using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Switches
{
	public class ActionGraphSwitch : Switch
	{
		public float SwitchTimer = 0f;

		public string InteractableTag;

		public float PersistWhileInRadius = -1f;

		private EventDispatcher dispatcher;

		private float elapsedTime;

		private bool waitingForRadius;

		public void Start()
		{
			waitingForRadius = false;
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<PenguinInteraction.InteractionStartedEvent>(onInteractionStarted);
		}

		public void OnDestroy()
		{
			if (dispatcher != null)
			{
				dispatcher.RemoveListener<PenguinInteraction.InteractionStartedEvent>(onInteractionStarted);
			}
		}

		private bool onInteractionStarted(PenguinInteraction.InteractionStartedEvent evt)
		{
			if (Service.Get<CPDataEntityCollection>().IsLocalPlayer(evt.InteractingPlayerId) && evt.ObjectInteractedWith != null && evt.ObjectInteractedWith.CompareTag(InteractableTag))
			{
				elapsedTime = 0f;
				if (!base.OnOff)
				{
					if (SwitchTimer > 0f)
					{
						CoroutineRunner.Start(waitForTimer(), this, "waitForTimer");
					}
					Change(true);
				}
			}
			return false;
		}

		private IEnumerator waitForTimer()
		{
			while (elapsedTime < SwitchTimer)
			{
				yield return null;
				elapsedTime += Time.deltaTime;
			}
			if (!waitingForRadius)
			{
				Change(false);
			}
		}

		public override string GetSwitchType()
		{
			throw new NotImplementedException();
		}

		public override object GetSwitchParameters()
		{
			throw new NotImplementedException();
		}
	}
}
