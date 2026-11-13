using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkUIInteractionStage : BenchmarkTestStage
	{
		[Header("UI Interaction Settings")]
		public List<ExternalEvent> InteractionEvents;

		protected override void performBenchmark()
		{
			StateMachineContext component = GameObject.FindGameObjectWithTag(UIConstants.Tags.UI_Tray_Root).GetComponent<StateMachineContext>();
			foreach (ExternalEvent interactionEvent in InteractionEvents)
			{
				try
				{
					component.SendEvent(interactionEvent);
				}
				catch (Exception)
				{
				}
			}
			onFinish();
		}
	}
}
