using System;

namespace Fabric
{
	[Serializable]
	public class InterpolatedParameter
	{
		private float startTimeMS;

		private float durationMS;

		private float curve = 0.05f;

		private float end;

		private float start;

		private float currentValue;

		public InterpolatedParameter()
		{
		}

		public InterpolatedParameter(float startValue)
		{
			Reset(startValue);
		}

		public void Reset(float value)
		{
			start = value;
			end = value;
			currentValue = value;
			startTimeMS = 0f;
			durationMS = 0f;
			curve = 0.5f;
		}

		public float GetTimeRemaining(float CurrentTimeMS)
		{
			float num = startTimeMS + durationMS;
			if (num <= CurrentTimeMS)
			{
				return 0f;
			}
			return num - CurrentTimeMS;
		}

		public void SetTarget(float CurrentTimeMS, float Target, float TimeToTargetMS, float Curve)
		{
			if (Target == end)
			{
				return;
			}
			if (TimeToTargetMS == 0f || Curve >= 1f)
			{
				end = Target;
				currentValue = end;
				return;
			}
			start = Get(CurrentTimeMS);
			durationMS = TimeToTargetMS;
			end = Target;
			if (Curve <= 1E-05f)
			{
				Curve = 1E-05f;
			}
			curve = 1f - 1f / Curve;
			if (durationMS == 0f)
			{
				start = Target;
			}
			startTimeMS = CurrentTimeMS;
		}

		public bool HasReachedTarget(float CurrentTimeMS)
		{
			return CurrentTimeMS >= startTimeMS + durationMS;
		}

		public bool HasReachedTarget()
		{
			return currentValue == end;
		}

		public float GetCurrentValue()
		{
			return currentValue;
		}

		public float GetTarget()
		{
			return end;
		}

		public float Get(float CurrentTimeMS)
		{
			float num = CurrentTimeMS - startTimeMS;
			float num2;
			if (num >= durationMS)
			{
				num2 = end;
			}
			else
			{
				float num3 = num / durationMS;
				num2 = start + (end - start) * (num3 / (num3 + curve * (num3 - 1f)));
			}
			currentValue = num2;
			return currentValue;
		}
	}
}
