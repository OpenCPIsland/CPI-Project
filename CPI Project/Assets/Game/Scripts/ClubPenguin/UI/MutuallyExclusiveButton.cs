using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class MutuallyExclusiveButton : MonoBehaviour
	{
		private EventDispatcher dispatcher;

		private InputButton inputButton;

		private TrayInputButton trayInputButton;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		[Tooltip("If true, any movement by the local player will remove the highlight from the button")]
		public bool ResetOnMove = true;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<InputEvents.ActionEvent>(onAction);
			inputButton = GetComponentInParent<InputButton>();
			trayInputButton = GetComponentInParent<TrayInputButton>();
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				locomotionEventBroadcaster = localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
				if (locomotionEventBroadcaster != null)
				{
					locomotionEventBroadcaster.OnStickDirectionEvent += OnStickDirection;
				}
			}
		}

		private void OnStickDirection(Vector2 steer)
		{
			if (steer != Vector2.zero && ResetOnMove && isButtonHighlighted())
			{
				GetTrayInputButton();
				if (trayInputButton != null)
				{
					trayInputButton.SetState(TrayInputButton.ButtonState.Default);
				}
			}
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<InputEvents.ActionEvent>(onAction);
			if (locomotionEventBroadcaster != null && !locomotionEventBroadcaster.gameObject.IsDestroyed())
			{
				locomotionEventBroadcaster.OnStickDirectionEvent -= OnStickDirection;
			}
		}

		private bool onAction(InputEvents.ActionEvent evt)
		{
			if (inputButton == null)
			{
				inputButton = GetComponentInParent<InputButton>();
			}
			GetTrayInputButton();
			if (inputButton != null && trayInputButton != null)
			{
				if (inputButton.Action == evt.Action)
				{
					trayInputButton.SetState(TrayInputButton.ButtonState.Highlighted);
				}
				else if (isButtonHighlighted())
				{
					trayInputButton.SetState(TrayInputButton.ButtonState.Default);
				}
			}
			return false;
		}

		private bool isButtonHighlighted()
		{
			return trayInputButton.CurrentState == TrayInputButton.ButtonState.Highlighted;
		}

		private void GetTrayInputButton()
		{
			if (trayInputButton == null)
			{
				trayInputButton = GetComponentInParent<TrayInputButton>();
			}
		}
	}
}
