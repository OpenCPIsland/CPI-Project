using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace AssemblyCSharp
{
	[RequireComponent(typeof(Collider))]
	public class DynamicQuestArrow : MonoBehaviour
	{
		public Vector3 Offset = default(Vector3);

		public string IndicatorContentKey = "Prefabs/ActionIndicators/ActionIndicatorArrow";

		private DActionIndicator indicatorData;

		public void Start()
		{
			string indicatorId = GetInstanceID().ToString();
			indicatorData = new DActionIndicator();
			indicatorData.IndicatorId = indicatorId;
			indicatorData.TargetTransform = base.transform;
			indicatorData.IndicatorContentKey = IndicatorContentKey;
			indicatorData.TargetOffset = Offset;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ActionIndicatorEvents.AddActionIndicator(indicatorData));
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ActionIndicatorEvents.RemoveActionIndicator(indicatorData));
			}
		}
	}
}
