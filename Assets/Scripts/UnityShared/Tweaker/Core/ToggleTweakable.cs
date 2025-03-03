using System;

namespace Tweaker.Core
{
	public class ToggleTweakable<T> : IToggleTweakable, IStepTweakable
	{
		private readonly BaseTweakable<T> baseTweakable;

		private int currentIndex = -1;

		private TweakableInfo<T> tweakableInfo;

		public BaseTweakable<T> BaseTweakable
		{
			get
			{
				return baseTweakable;
			}
		}

		public object StepSize
		{
			get
			{
				return 1;
			}
		}

		public int CurrentIndex
		{
			get
			{
				return currentIndex;
			}
		}

		public int ToggleCount
		{
			get
			{
				return tweakableInfo.ToggleValues.Length;
			}
		}

		public Type TweakableType
		{
			get
			{
				return typeof(T);
			}
		}

		public ToggleTweakable(BaseTweakable<T> baseTweakable)
		{
			this.baseTweakable = baseTweakable;
			tweakableInfo = baseTweakable.TweakableInfo;
			currentIndex = GetIndexOfValue(baseTweakable.GetValue());
		}

		public int GetIndexOfValue(object value)
		{
			for (int i = 0; i < tweakableInfo.ToggleValues.Length; i++)
			{
				TweakableNamedToggleValue<T> tweakableNamedToggleValue = tweakableInfo.ToggleValues[i];
				if (tweakableNamedToggleValue.Value.Equals(value))
				{
					return i;
				}
			}
			return -1;
		}

		public string GetNameByIndex(int index)
		{
			if (index >= 0 && tweakableInfo.ToggleValues.Length > index)
			{
				return tweakableInfo.ToggleValues[index].Name;
			}
			return null;
		}

		public string GetNameByValue(object value)
		{
			return GetNameByIndex(GetIndexOfValue(value));
		}

		public object SetValueByName(string valueName)
		{
			for (int i = 0; i < tweakableInfo.ToggleValues.Length; i++)
			{
				if (tweakableInfo.ToggleValues[i].Name == valueName)
				{
					currentIndex = i;
					baseTweakable.SetValue(tweakableInfo.ToggleValues[i].Value);
					return baseTweakable.GetValue();
				}
			}
			throw new TweakableSetException(baseTweakable.Name, "Invalid toggle value name: '" + valueName + "'");
		}

		public string GetValueName()
		{
			if (currentIndex >= 0 && currentIndex < tweakableInfo.ToggleValues.Length)
			{
				return tweakableInfo.ToggleValues[currentIndex].Name;
			}
			return null;
		}

		public object StepNext()
		{
			currentIndex++;
			if (currentIndex >= tweakableInfo.ToggleValues.Length)
			{
				currentIndex = 0;
			}
			T value = tweakableInfo.ToggleValues[currentIndex].Value;
			baseTweakable.SetValue(value);
			return baseTweakable.GetValue();
		}

		public object StepPrevious()
		{
			currentIndex--;
			if (currentIndex < 0)
			{
				currentIndex = tweakableInfo.ToggleValues.Length - 1;
			}
			T value = tweakableInfo.ToggleValues[currentIndex].Value;
			baseTweakable.SetValue(value);
			return baseTweakable.GetValue();
		}
	}
}
