namespace Tweaker.Core
{
	public class TweakableInfo<T> : TweakerObjectInfo
	{
		public TweakableRange<T> Range;

		public TweakableStepSize<T> StepSize;

		public TweakableNamedToggleValue<T>[] ToggleValues;

		public TweakableInfo(string name, TweakableRange<T> range, TweakableStepSize<T> stepSize, TweakableNamedToggleValue<T>[] toggleValues, uint instanceId = 0u, ICustomTweakerAttribute[] customAttributes = null, string description = "")
			: base(name, instanceId, customAttributes, description)
		{
			Range = range;
			StepSize = stepSize;
			ToggleValues = toggleValues;
		}

		public TweakableInfo(string name, ICustomTweakerAttribute[] customAttributes = null, string description = "")
			: base(name, 0u, customAttributes, description)
		{
			Range = null;
			StepSize = null;
			ToggleValues = null;
		}
	}
}
