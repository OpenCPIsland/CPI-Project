using ClubPenguin.Avatar;
using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(AvatarRenderTextureComponent))]
	public class LocalPlayerAvatarComponent : MonoBehaviour
	{
		private AvatarRenderTextureComponent avatarRenderTextureComponent;

		private void Start()
		{
			avatarRenderTextureComponent = GetComponent<AvatarRenderTextureComponent>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
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
	}
}
