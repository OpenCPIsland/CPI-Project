using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClubPenguin
{
	public class InactivityService : MonoBehaviour
	{
		public int InactivityTimeoutSeconds;

		private DateTime futureTimeoutTime;

		private bool isSessionActive;

		private bool isActive;

		private bool isTrackingEnabled = true;

		public bool IsTrackingEnabled
		{
			get
			{
				return isTrackingEnabled;
			}
		}

		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				isActive = value;
				if (isActive)
				{
					futureTimeoutTime = DateTime.Now.AddSeconds(getTimeoutSeconds());
				}
			}
		}

		public void SetTrackingEnabled(bool enabled)
		{
			isTrackingEnabled = enabled;
			updateActivityState();
		}

		private void Start()
		{
			if (Service.IsSet<EventDispatcher>())
			{
				Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
				Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
			}
			else
			{
				Debug.LogWarning("EventDispatcher service is not set.");
			}
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			isSessionActive = true;
			updateActivityState();
			return false;
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			isSessionActive = false;
			updateActivityState();
			return false;
		}

		private void Update()
		{
			if (isActive)
			{
				bool anyKeyDown = false;

				if (Keyboard.current != null)
				{
					foreach (var keyControl in Keyboard.current.allKeys)
					{
						if (keyControl != null && keyControl.wasPressedThisFrame)
						{
							anyKeyDown = true;
							break;
						}
					}
				}

				if (Mouse.current != null)
				{
					if (Mouse.current.leftButton.wasPressedThisFrame ||
						Mouse.current.rightButton.wasPressedThisFrame ||
						Mouse.current.middleButton.wasPressedThisFrame ||
						Mouse.current.forwardButton.wasPressedThisFrame ||
						Mouse.current.backButton.wasPressedThisFrame ||
						Mouse.current.scroll.ReadValue() != Vector2.zero)
					{
						anyKeyDown = true;
					}
				}

				if (Touchscreen.current != null)
				{
					foreach (var touch in Touchscreen.current.touches)
					{
						if (touch != null && touch.press.wasPressedThisFrame)
						{
							anyKeyDown = true;
							break;
						}
					}
				}

				if (anyKeyDown)
				{
					futureTimeoutTime = DateTime.Now.AddSeconds(getTimeoutSeconds());
				}
				else if (futureTimeoutTime < DateTime.Now)
				{
					if (Service.IsSet<GameStateController>())
					{
						Service.Get<GameStateController>().ExitWorld();
					}
					else
					{
						Debug.LogWarning("GameStateController service is not set.");
					}
					isActive = false;
				}
			}
		}

		private void updateActivityState()
		{
			IsActive = (isTrackingEnabled && isSessionActive);
		}

		private int getTimeoutSeconds()
		{
			return Mathf.Max(1, InactivityTimeoutSeconds);
		}
	}
}
