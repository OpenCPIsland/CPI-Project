using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest (Advanced)")]
	public class SetQuestMarkerAction : FsmStateAction
	{
		[RequiredField]
		public string MarkerName;

		public string MarkerID;

		[RequiredField]
		public string IndicatorContentKey = "Prefabs/ActionIndicators/ActionIndicatorArrow";

		public Vector3 Offset = new Vector3(0f, 1f, 0f);

		public bool DestroyOnExit = true;

		private DActionIndicator indicatorData;

		private GameObject marker;

		public override void OnEnter()
		{
			marker = GameObject.Find(MarkerName);
			if (marker != null)
			{
				indicatorData = new DActionIndicator();
				indicatorData.IndicatorId = MarkerID;
				indicatorData.TargetTransform = marker.transform;
				indicatorData.IndicatorContentKey = IndicatorContentKey;
				indicatorData.TargetOffset = Offset;
				Service.Get<EventDispatcher>().DispatchEvent(new ActionIndicatorEvents.AddActionIndicator(indicatorData));
			}
			Finish();
		}

		public override void OnExit()
		{
			if (marker != null && DestroyOnExit)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ActionIndicatorEvents.RemoveActionIndicator(indicatorData));
			}
		}
	}
}
