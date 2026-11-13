using UnityEngine;

namespace ClubPenguin.UI
{
	public class DActionIndicator
	{
		public string IndicatorId
		{
			get;
			set;
		}

		public string IndicatorContentKey
		{
			get;
			set;
		}

		public Transform TargetTransform
		{
			get;
			set;
		}

		public Vector3 TargetOffset
		{
			get;
			set;
		}

		public bool IsVisible
		{
			get;
			set;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is DActionIndicator))
			{
				return false;
			}
			DActionIndicator dActionIndicator = obj as DActionIndicator;
			if (string.IsNullOrEmpty(dActionIndicator.IndicatorId))
			{
				return false;
			}
			return IndicatorId.Equals(dActionIndicator.IndicatorId);
		}

		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(IndicatorId))
			{
				return IndicatorId.GetHashCode();
			}
			return 0;
		}
	}
}
