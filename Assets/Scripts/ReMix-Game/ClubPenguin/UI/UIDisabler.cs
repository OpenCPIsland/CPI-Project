using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class UIDisabler : MonoBehaviour
	{
		[SerializeField]
		private const float CAMERA_CULL_DELAY = 1f;

		[SerializeField]
		private bool isDisablePlayerCard = false;

		[SerializeField]
		private bool isDisableTray = true;

		[SerializeField]
		private bool isDisableTrayInstant = false;

		[SerializeField]
		private bool isCullCamera = true;

		[SerializeField]
		private bool isDisableChatUI = true;

		[SerializeField]
		private bool isDisablePlayerNames = true;

		[SerializeField]
		private bool isDisableQuestNavigation = true;

		[SerializeField]
		private bool isDisableCellPhoneHud = true;

		[SerializeField]
		private bool isDisableIglooEditButton = false;

		[SerializeField]
		private bool isDisableQuestHud = true;

		[SerializeField]
		private bool isDisableAllAccessTicker = true;

		[SerializeField]
		private bool resetOnDestroy = true;

		private int previousCameraCullingMask;

		public bool ResetOnDestroy
		{
			get
			{
				return resetOnDestroy;
			}
			set
			{
				resetOnDestroy = value;
			}
		}

		private void Start()
		{
			if (isDisablePlayerCard)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
			}
			if (isDisableTray || isDisableTrayInstant)
			{
				disableTray(isDisableTrayInstant);
			}
			if (isCullCamera)
			{
				CoroutineRunner.Start(cullCamera(), this, "");
			}
			if (isDisableChatUI)
			{
				disableChatUI();
			}
			if (isDisablePlayerNames)
			{
				disablePlayerNames();
			}
			if (isDisableQuestNavigation)
			{
				disableQuestNavigation();
			}
			if (isDisableCellPhoneHud)
			{
				disableCellPhoneHud();
			}
			if (isDisableQuestHud)
			{
				disableQuestHud();
			}
			if (isDisableAllAccessTicker)
			{
				disableAllAccessTicker();
			}
			if (isDisableIglooEditButton)
			{
				disableIglooEditButton();
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (!Service.Get<SceneTransitionService>().IsTransitioning && resetOnDestroy)
			{
				if (isDisablePlayerCard)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
				}
				if (isDisableTray)
				{
					enableTray();
				}
				if (isCullCamera)
				{
					restoreCamera();
				}
				if (isDisableChatUI)
				{
					enableChatUI();
				}
				if (isDisablePlayerNames)
				{
					enablePlayerNames();
				}
				if (isDisableQuestNavigation)
				{
					enableQuestNavigation();
				}
				if (isDisableCellPhoneHud)
				{
					enableCellPhoneHud();
				}
				if (isDisableQuestHud)
				{
					enableQuestHud();
				}
				if (isDisableAllAccessTicker)
				{
					enableAllAccessTicker();
				}
				if (isDisableIglooEditButton)
				{
					enableIglooEditButton();
				}
			}
		}

		private void disablePlayerNames()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.HidePlayerNames));
		}

		private void enablePlayerNames()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerNameEvents.ShowPlayerNames));
		}

		private void disableQuestNavigation()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideQuestNavigation));
		}

		private void enableQuestNavigation()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ShowQuestNavigation));
		}

		private void disableCellPhoneHud()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideCellPhoneHud));
		}

		private void enableCellPhoneHud()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ShowCellPhoneHud));
		}

		private void disableQuestHud()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_HUD);
			if (gameObject != null)
			{
				QuestHud componentInChildren = gameObject.GetComponentInChildren<QuestHud>(true);
				if (componentInChildren != null)
				{
					componentInChildren.gameObject.SetActive(false);
				}
			}
		}

		private void enableQuestHud()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_HUD);
			if (gameObject != null)
			{
				QuestHud componentInChildren = gameObject.GetComponentInChildren<QuestHud>(true);
				if (componentInChildren != null)
				{
					componentInChildren.gameObject.SetActive(true);
				}
			}
		}

		private void disableAllAccessTicker()
		{
			AllAccessCountdownBarController allAccessCountdownBarController = Object.FindObjectOfType<AllAccessCountdownBarController>();
			if (allAccessCountdownBarController != null)
			{
				allAccessCountdownBarController.Hide();
			}
		}

		private void enableAllAccessTicker()
		{
			AllAccessCountdownBarController allAccessCountdownBarController = Object.FindObjectOfType<AllAccessCountdownBarController>();
			if (allAccessCountdownBarController != null)
			{
				allAccessCountdownBarController.Show();
			}
		}

		private void disableIglooEditButton()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideIglooEditButton));
		}

		private void enableIglooEditButton()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ShowIglooEditButton));
		}

		private void disableTray(bool instant)
		{
			if (SceneRefs.UiTrayRoot != null)
			{
				if (instant)
				{
					SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "noUIInstant"));
				}
				else
				{
					SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "noUI"));
				}
			}
		}

		private void enableTray()
		{
			if (SceneRefs.UiTrayRoot != null && SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>() != null)
			{
				SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "restoreUI"));
			}
		}

		private void disableChatUI()
		{
			GameObject uiChatRoot = SceneRefs.UiChatRoot;
			if (!(uiChatRoot != null))
			{
				return;
			}
			Canvas[] componentsInChildren = uiChatRoot.GetComponentsInChildren<Canvas>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].GetComponent<CinematicSpeechController>() == null)
				{
					componentsInChildren[i].enabled = false;
				}
			}
		}

		private void enableChatUI()
		{
			GameObject uiChatRoot = SceneRefs.UiChatRoot;
			if (!(uiChatRoot != null))
			{
				return;
			}
			Canvas[] componentsInChildren = uiChatRoot.GetComponentsInChildren<Canvas>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].GetComponent<CinematicSpeechController>() == null)
				{
					componentsInChildren[i].enabled = true;
				}
			}
		}

		private IEnumerator cullCamera()
		{
			previousCameraCullingMask = Camera.main.cullingMask;
			yield return new WaitForSeconds(1f);
			CameraCullingMaskHelper.HideAllLayers(Camera.main);
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.CameraCullingStateChanged(false));
		}

		private void restoreCamera()
		{
			Camera.main.cullingMask = previousCameraCullingMask;
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.CameraCullingStateChanged(true));
		}
	}
}
