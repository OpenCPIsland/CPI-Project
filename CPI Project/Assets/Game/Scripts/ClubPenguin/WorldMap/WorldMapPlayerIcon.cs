using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.WorldMap
{
	public class WorldMapPlayerIcon : MonoBehaviour
	{
		private const float ANIMATION_TIME = 0.5f;

		public RawImage PlayerAvatarIcon;

		public RuntimeAnimatorController PenguinAnimatorController;

		public string AnimationState;

		private AvatarImageComponent avatarRenderer;

		private void Start()
		{
			avatarRenderer = GetComponent<AvatarImageComponent>();
			AvatarImageComponent avatarImageComponent = avatarRenderer;
			avatarImageComponent.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Combine(avatarImageComponent.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			AvatarAnimationFrame avatarAnimationFrame = new AvatarAnimationFrame(AnimationState, 0.5f);
			avatarRenderer.RequestImage(localPlayerHandle, avatarAnimationFrame);
		}

		private void OnDestroy()
		{
			if (avatarRenderer != null)
			{
				AvatarImageComponent avatarImageComponent = avatarRenderer;
				avatarImageComponent.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Remove(avatarImageComponent.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			}
		}

		private void onImageReady(DataEntityHandle handle, Texture2D icon)
		{
			PlayerAvatarIcon.texture = icon;
		}
	}
}
