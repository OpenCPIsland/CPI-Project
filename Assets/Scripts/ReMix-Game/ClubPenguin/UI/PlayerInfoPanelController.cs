using ClubPenguin.Avatar;
using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(AvatarRenderTextureComponent))]
	public class PlayerInfoPanelController : MonoBehaviour
	{
		private const int LOD_INDEX = 1;

		public Text DisplayNameText;

		private AvatarRenderTextureComponent avatarRenderTextureComponent;

		private DisplayNameData displayNameData;

		private void Start()
		{
			avatarRenderTextureComponent = GetComponent<AvatarRenderTextureComponent>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out displayNameData))
				{
					DisplayNameText.text = displayNameData.DisplayName;
					displayNameData.OnDisplayNameChanged += onDisplayNameChanged;
				}
				else
				{
					Log.LogError(this, "Local player handle did not have display name data");
				}
				AvatarDetailsData component;
				if (cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component))
				{
					avatarRenderTextureComponent.RenderAvatar(component);
					return;
				}
				avatarRenderTextureComponent.RenderAvatar(new DCustomEquipment[0]);
				Log.LogError(this, "Local player handle did not have avatar details data");
			}
			else
			{
				Log.LogError(this, "Local player handle was null");
			}
		}

		private void onDisplayNameChanged(string displayName)
		{
			DisplayNameText.text = displayName;
		}

		private void OnDestroy()
		{
			if (displayNameData != null)
			{
				displayNameData.OnDisplayNameChanged -= onDisplayNameChanged;
			}
		}
	}
}
