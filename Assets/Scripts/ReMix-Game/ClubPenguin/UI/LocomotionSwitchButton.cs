using ClubPenguin.Core;
using ClubPenguin.Input;
using ClubPenguin.Locomotion;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class LocomotionSwitchButton : MonoBehaviour, IRestartable
	{
		private const float COOLDOWN = 0.75f;

		public InputEvents.Switches Switch;

		private TrayInputButton trayInputButton;

		private ButtonClickListener clickListener;

		private EventDispatcher dispatcher;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		private bool isSliding;

		private bool isCoolingDown;

		private void Start()
		{
			trayInputButton = GetComponentInParent<TrayInputButton>();
			if (trayInputButton != null)
			{
				clickListener = trayInputButton.GetComponent<ButtonClickListener>();
				clickListener.OnClick.AddListener(OnClicked);
			}
			else
			{
				Log.LogError(this, "TrayInputButton not found");
			}
			dispatcher = Service.Get<EventDispatcher>();
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				locomotionEventBroadcaster = localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
				SlideController component = localPlayerGameObject.GetComponent<SlideController>();
				if (component != null && component.enabled)
				{
					changeState(true);
				}
			}
			if (locomotionEventBroadcaster != null)
			{
				locomotionEventBroadcaster.OnControllerChangedEvent += onControllerChanged;
			}
			else
			{
				Log.LogError(this, "LocomotionEventBroadcaster was not found in the scene");
			}
		}

		public void Restart()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				locomotionEventBroadcaster = localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
				SlideController component = localPlayerGameObject.GetComponent<SlideController>();
				if (component != null && component.enabled)
				{
					changeState(true);
				}
			}
		}

		private void OnClicked(ButtonClickListener.ClickType interactedType)
		{
			if (!isCoolingDown)
			{
				isCoolingDown = true;
				dispatcher.DispatchEvent(new InputEvents.SwitchChangeEvent(Switch, isSliding));
				CoroutineRunner.Start(cooldownFinished(), this, "cooldownFinished");
			}
		}

		private IEnumerator cooldownFinished()
		{
			yield return new WaitForSeconds(0.75f);
			isCoolingDown = false;
		}

		private void onControllerChanged(LocomotionController controller)
		{
			if (controller is SlideController)
			{
				changeState(true);
			}
			else
			{
				changeState(false);
			}
		}

		private void changeState(bool isSliding)
		{
			this.isSliding = isSliding;
			trayInputButton.SetState((!isSliding) ? TrayInputButton.ButtonState.Default : TrayInputButton.ButtonState.Highlighted);
		}

		private void OnDestroy()
		{
			if (clickListener != null)
			{
				clickListener.OnClick.RemoveListener(OnClicked);
			}
			if (locomotionEventBroadcaster != null)
			{
				locomotionEventBroadcaster.OnControllerChangedEvent -= onControllerChanged;
			}
		}
	}
}
