using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	public class CameraRenderingControl : MonoBehaviour
	{
		private Camera mainCamera;

		private int cullMaskState = 0;

		private bool renderingState = true;

		private bool chatWasIncluded = false;

		private bool uiWasCulled = false;

		public void Awake()
		{
			SceneRefs.Set(this);
			renderingState = true;
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			mainCamera = gameObject.GetComponent<Camera>();
		}

		public void DisableRendering(bool includeChat, bool cullUILayer)
		{
			SetRenderingState(false, includeChat, cullUILayer);
		}

		public void EnableRendering()
		{
			SetRenderingState(true, chatWasIncluded, uiWasCulled);
		}

		public void SetRenderingState(bool enabled, bool includeChat, bool cullUILayer)
		{
			if (renderingState == enabled)
			{
				return;
			}
			renderingState = enabled;
			chatWasIncluded = includeChat;
			uiWasCulled = cullUILayer;
			if (enabled)
			{
				mainCamera.cullingMask = cullMaskState;
			}
			else
			{
				cullMaskState = mainCamera.cullingMask;
				mainCamera.cullingMask = 0;
				if (!cullUILayer)
				{
					mainCamera.cullingMask |= LayerMask.GetMask("UI");
				}
			}
			if (enabled)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.ShowPlayerNames));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.HidePlayerNames));
			}
			if (includeChat)
			{
				SetChatRenderingState(enabled);
			}
			Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.RenderingStateChanged(enabled));
		}

		public void SetChatRenderingState(bool enabled)
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Chat);
			if (gameObject != null)
			{
				Canvas[] componentsInChildren = gameObject.GetComponentsInChildren<Canvas>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = enabled;
				}
			}
		}
	}
}
