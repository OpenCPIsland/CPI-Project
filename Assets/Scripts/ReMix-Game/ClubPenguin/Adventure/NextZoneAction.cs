using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class NextZoneAction : FsmStateAction
	{
		[RequiredField]
		public string Zone;

		[RequiredField]
		public string IndicatorContentKey = "Prefabs/ActionIndicators/ActionIndicatorArrow";

		public Vector3 Offset = new Vector3(0f, 1f, 0f);

		private EventDispatcher dispatcher;

		private GameObject transition;

		private string[] zoneMap = new string[3]
		{
			"Dock",
			"Beach",
			"Diving"
		};

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			transition = GameObject.Find(Zone + "Transition");
			ZoneDefinition currentZone = Service.Get<ZoneTransitionService>().CurrentZone;
			if (currentZone != null && currentZone.SceneName == Zone)
			{
				Finish();
				return;
			}
			if (transition != null)
			{
				dispatcher.DispatchEvent(new HudEvents.SetNavigationTarget(transition.transform));
				return;
			}
			ZoneDefinition currentZone2 = Service.Get<ZoneTransitionService>().CurrentZone;
			string value = (currentZone2 != null) ? currentZone2.SceneName : "";
			int num = Array.IndexOf(zoneMap, Zone);
			int num2 = Array.IndexOf(zoneMap, value);
			int num3 = -1;
			string text = "";
			if (num != -1 && num != num2)
			{
				num3 = ((num > num2) ? (num2 + 1) : (num2 - 1));
				text = zoneMap[num3];
				GameObject gameObject = GameObject.Find(text + "Transition");
				if (gameObject != null)
				{
					dispatcher.DispatchEvent(new HudEvents.SetNavigationTarget(gameObject.transform));
				}
			}
		}

		public override void OnExit()
		{
			ZoneDefinition currentZone = Service.Get<ZoneTransitionService>().CurrentZone;
			if ((currentZone == null || currentZone.SceneName != Zone) && transition != null)
			{
				dispatcher.DispatchEvent(default(HudEvents.SetNavigationTarget));
			}
		}
	}
}
