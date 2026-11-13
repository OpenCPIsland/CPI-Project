using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class ConsumingPropSwitch : Switch
	{
		[Tooltip("When enabled, switch persists after consuming a prop for as long as user is sitting.")]
		public bool PersistWhenSitting = true;

		public float PersistForTime = 0f;

		[Tooltip("Prop type to consider.")]
		public PropDefinition.PropTypes PropType = PropDefinition.PropTypes.Consumable;

		[Tooltip("Prop visual treatment type to consider.")]
		public Prop.VisualTreatmentType VisualTreatmentType = Prop.VisualTreatmentType.Solo;

		private PropService propService;

		private PropUser propUser;

		private LocomotionEventBroadcaster locoEventBroadcaster;

		private bool consuming;

		private float elapsedTime;

		private bool timerCoroutineIsRunning;

		public void Start()
		{
			if (!SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.IsDestroyed())
			{
				propUser = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PropUser>();
				if (PersistWhenSitting)
				{
					locoEventBroadcaster = propUser.GetComponent<LocomotionEventBroadcaster>();
					locoEventBroadcaster.OnControllerChangedEvent += onControllerChangedEvent;
				}
				propUser.EPropUseStarted += onPropUseStarted;
			}
		}

		public void OnDestroy()
		{
			if (propUser != null)
			{
				propUser.EPropUseStarted -= onPropUseStarted;
				if (PersistWhenSitting && locoEventBroadcaster != null)
				{
					locoEventBroadcaster.OnControllerChangedEvent -= onControllerChangedEvent;
				}
			}
			if (base.OnOff)
			{
				propUser.EPropUserEnteredIdle -= onPropUserEnteredIdle;
				propUser.EPropStored -= onPropStored;
			}
		}

		private void onControllerChangedEvent(LocomotionController newController)
		{
			if (!(newController is SitController) && !consuming)
			{
				Change(false);
			}
		}

		private void onPropUseStarted(Prop prop)
		{
			if (!base.OnOff && prop.PropDef != null && prop.PropDef.PropType == PropType && prop.VisualTreatment == VisualTreatmentType && prop.CustomCamera == null)
			{
				propUser.EPropUserEnteredIdle += onPropUserEnteredIdle;
				propUser.EPropStored += onPropStored;
				consuming = true;
				elapsedTime = 0f;
				Change(true);
				if (PersistForTime > 0f && !timerCoroutineIsRunning)
				{
					CoroutineRunner.Start(ticSwitch(), this, "ticSwitch");
				}
			}
		}

		private void onPropUserEnteredIdle()
		{
			if (base.OnOff && consuming)
			{
				propUser.EPropUserEnteredIdle -= onPropUserEnteredIdle;
				propUser.EPropStored -= onPropStored;
				consuming = false;
				if ((!PersistWhenSitting || !LocomotionHelper.IsCurrentControllerOfType<SitController>(propUser.gameObject)) && !timerCoroutineIsRunning)
				{
					Change(false);
				}
			}
		}

		private void onPropStored(Prop prop)
		{
			onPropUserEnteredIdle();
		}

		private IEnumerator ticSwitch()
		{
			for (timerCoroutineIsRunning = true; elapsedTime < PersistForTime; elapsedTime += Time.deltaTime)
			{
				yield return null;
			}
			if (!PersistWhenSitting || !LocomotionHelper.IsCurrentControllerOfType<SitController>(propUser.gameObject))
			{
				Change(false);
			}
			timerCoroutineIsRunning = false;
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
