using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Tags;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native.iOS;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Props
{
	public class RetrievePropCMD
	{
		private const float MAX_MESH_COMBINE_WAIT_TIME_SEC = 15f;

		private string propId;

		private PrefabContentKey propContentKey;

		private PropUser propUser;

		private long ownerId;

		private bool isOwnerLocalPlayer;

		private Action<Prop> onPropInstantiatedHandler;

		private System.Action onCompleteHandler;

		private AvatarView avatarView;

		private float meshCombineWaitTimeSec;

		public RetrievePropCMD(string propId, PrefabContentKey propContentKey, PropUser propUser, long ownerId, bool isOwnerLocalPlayer, Action<Prop> onPropInstantiatedHandler = null, System.Action onCompleteHandler = null)
		{
			this.propId = propId;
			this.propContentKey = propContentKey;
			this.propUser = propUser;
			this.ownerId = ownerId;
			this.isOwnerLocalPlayer = isOwnerLocalPlayer;
			this.onPropInstantiatedHandler = onPropInstantiatedHandler;
			this.onCompleteHandler = onCompleteHandler;
			avatarView = propUser.GetComponent<AvatarView>();
		}

		public void Execute()
		{
			CoroutineRunner.Start(loadPropAssetAndPlayRetrieveAnim(), propUser, "loadPropAssetAndPlayRetrieveAnim");
		}

		private IEnumerator loadPropAssetAndPlayRetrieveAnim()
		{
			while (!avatarView.IsReady)
			{
				meshCombineWaitTimeSec += Time.deltaTime;
				if (meshCombineWaitTimeSec > 15f)
				{
					throw new Exception("RetrievePropCMD timed out while waiting for avatar mesh to combine. Was the Avatar GameObject Destroyed?");
				}
				yield return null;
			}
			AssetRequest<GameObject> assetRequest = null;
			try
			{
				assetRequest = Content.LoadAsync(propContentKey);
			}
			catch (Exception ex)
			{
				Log.LogErrorFormatted(this, "Unable to load Prop id {0}. Path was {1}. Message: {2}", propId, propContentKey, ex.Message);
			}
			if (assetRequest != null)
			{
				yield return assetRequest;
				try
				{
					if (isOwnerLocalPlayer)
					{
						Service.Get<iOSHapticFeedback>().TriggerImpactFeedback(iOSHapticFeedback.ImpactFeedbackStyle.Light);
					}
					Prop propPrefab = assetRequest.Asset.GetComponent<Prop>();
					Prop propInstance = UnityEngine.Object.Instantiate(propPrefab);
					propInstance.gameObject.SetActive(false);
					propInstance.OwnerId = ownerId;
					propInstance.PropId = propId;
					propInstance.IsOwnerLocalPlayer = isOwnerLocalPlayer;
					propInstance.PropDef = Service.Get<PropService>().GetPropDefinition(propId);
					Service.Get<TagsManager>().MakeTagsData(propInstance.gameObject, new TagDefinition[1][]
					{
						propInstance.PropDef.Tags
					});
					if (onPropInstantiatedHandler != null)
					{
						onPropInstantiatedHandler(propInstance);
					}
					yield return CoroutineRunner.Start(propUser.RetrieveProp(propInstance), propUser, "propUser.RetrieveProp");
				}
				finally
				{
					if (onCompleteHandler != null)
					{
						onCompleteHandler();
					}
				}
			}
		}
	}
}
