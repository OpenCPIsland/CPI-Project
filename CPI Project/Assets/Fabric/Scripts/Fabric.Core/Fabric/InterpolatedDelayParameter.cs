using System;

namespace Fabric
{
	[Serializable]
	public class InterpolatedDelayParameter
	{
		private float startTimeMS;

		private float durationMS;

		private float curve = 0.05f;

		private float end;

		private float start;

		private float currentValue;

		private float _delay;

		public InterpolatedDelayParameter()
		{
		}

		public InterpolatedDelayParameter(float startValue)
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
			_delay = 0f;
		}

		public float GetTimeRemaining(float CurrentTimeMS)
		{
			float num = startTimeMS + durationMS + _delay;
			if (num <= CurrentTimeMS)
			{
				return 0f;
			}
			return num - CurrentTimeMS;
		}

		public void SetTarget(float CurrentTimeMS, float Target, float TimeToTargetMS, float Curve, float delay = 0f)
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
			_delay = delay;
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
			return CurrentTimeMS >= startTimeMS + durationMS + _delay;
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
			if (CurrentTimeMS < startTimeMS + _delay)
			{
				return currentValue;
			}
			float num = CurrentTimeMS - startTimeMS - _delay;
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
