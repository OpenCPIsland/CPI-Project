using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class InactivityService : MonoBehaviour
	{
		public int InactivityTimeoutSeconds;

		private DateTime futureTimeoutTime;

		private bool isActive;

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
					futureTimeoutTime = DateTime.Now.AddSeconds(InactivityTimeoutSeconds);
				}
			}
		}

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReady);
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
		}

		private bool onLocalPlayerDataReady(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			IsActive = true;
			return false;
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			IsActive = false;
			return false;
		}

		private void Update()
		{
			if (isActive)
			{
				if (UnityEngine.Input.anyKeyDown)
				{
					futureTimeoutTime = DateTime.Now.AddSeconds(InactivityTimeoutSeconds);
				}
				else if (futureTimeoutTime < DateTime.Now)
				{
					Service.Get<GameStateController>().ExitWorld();
					isActive = false;
				}
			}
		}
	}
}
