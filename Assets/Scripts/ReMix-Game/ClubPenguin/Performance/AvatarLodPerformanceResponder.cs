using UnityEngine;

namespace ClubPenguin.Performance
{
	[RequireComponent(typeof(LODGroup))]
	public class AvatarLodPerformanceResponder : PerformanceResponder
	{
		private LODGroup lodGroup;

		private float previousDetailLevel = 1f;

		public override PerformanceResponderType GetPerformanceResponderType()
		{
			return PerformanceResponderType.AvatarLod;
		}

		protected override void Awake()
		{
			lodGroup = GetComponent<LODGroup>();
			base.Awake();
		}

		protected override void onDetailLevelChanged(float newDetailLevel)
		{
			if (newDetailLevel == 0f)
			{
				disableAllLODs();
			}
			else if (newDetailLevel > 0f && previousDetailLevel == 0f)
			{
				enableAllLODs();
			}
			previousDetailLevel = newDetailLevel;
		}

		private void disableAllLODs()
		{
			lodGroup.enabled = false;
		}

		private void enableAllLODs()
		{
			lodGroup.enabled = true;
		}
	}
}
