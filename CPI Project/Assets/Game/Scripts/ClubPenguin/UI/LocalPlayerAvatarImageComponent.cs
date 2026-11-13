using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(AvatarImageComponent))]
	public class LocalPlayerAvatarImageComponent : MonoBehaviour
	{
		public RawImage AvatarIcon;

		public string Context;

		public string AnimationStateName = "Base Layer.Idle";

		public float AnimationNormalizedTime = 0.5f;

		private AvatarImageComponent avatarImageComponent;

		private void Start()
		{
			avatarImageComponent = GetComponent<AvatarImageComponent>();
			AvatarImageComponent obj = avatarImageComponent;
			obj.OnImageReady = (Action<DataEntityHandle, Texture2D>)Delegate.Combine(obj.OnImageReady, new Action<DataEntityHandle, Texture2D>(onImageReady));
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				AvatarAnimationFrame avatarAnimationFrame = new AvatarAnimationFrame(AnimationStateName, AnimationNormalizedTime);
				avatarImageComponent.RequestImage(localPlayerHandle, avatarAnimationFrame, Context);
			}
			else
			{
				Log.LogError(this, "Local player handle was null");
			}
		}

		private void onImageReady(DataEntityHandle handle, Texture2D icon)
		{
			AvatarIcon.texture = icon;
		}
	}
}
