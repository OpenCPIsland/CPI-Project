using ClubPenguin.BlobShadows;
using ClubPenguin.Cinematography;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class RemotePlayerVisibilityState
	{
		public static void HideRemotePlayers()
		{
			CameraCullingMaskHelper.HideLayer(Camera.main, "RemotePlayer");
			CameraCullingMaskHelper.HideLayer(Camera.main, "AllPlayerInteractibles");
			Service.Get<EventDispatcher>().DispatchEvent(new BlobShadowEvents.DisableBlobShadows(false));
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.HidePlayerNames));
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerIndicatorEvents.HidePlayerIndicators));
			if (SceneRefs.UiChatRoot != null)
			{
				WorldChatController componentInChildren = SceneRefs.UiChatRoot.GetComponentInChildren<WorldChatController>();
				if (componentInChildren != null)
				{
					componentInChildren.IgnoreRemoteChat = true;
				}
			}
		}

		public static void ShowRemotePlayers()
		{
			CameraCullingMaskHelper.ShowLayer(Camera.main, "RemotePlayer");
			CameraCullingMaskHelper.ShowLayer(Camera.main, "AllPlayerInteractibles");
			Service.Get<EventDispatcher>().DispatchEvent(default(BlobShadowEvents.EnableBlobShadows));
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.ShowPlayerNames));
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerIndicatorEvents.ShowPlayerIndicators));
			if (SceneRefs.UiChatRoot != null)
			{
				WorldChatController componentInChildren = SceneRefs.UiChatRoot.GetComponentInChildren<WorldChatController>();
				if (componentInChildren != null)
				{
					componentInChildren.IgnoreRemoteChat = false;
				}
			}
		}
	}
}
