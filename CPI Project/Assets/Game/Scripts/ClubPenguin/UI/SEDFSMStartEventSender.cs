using ClubPenguin.Core;
using ClubPenguin.Data;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SEDFSMStartEventSender : MonoBehaviour
	{
		private StateMachine stateMachine;

		private void Start()
		{
			stateMachine = GetComponent<StateMachine>();
			if (stateMachine == null)
			{
				Log.LogErrorFormatted(this, "Ensure this component is added to gameobject that will contain a StateMachine component. Currently its on {0} gameobject", base.gameObject.name);
				return;
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			SEDFSMStartEventData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				stateMachine.SendEvent(component.EventName);
				cPDataEntityCollection.RemoveComponent<SEDFSMStartEventData>(cPDataEntityCollection.LocalPlayerHandle);
			}
		}
	}
}
