using System;

namespace Tweaker.Core
{
	public interface ITweakable : ITweakerObject
	{
		Type TweakableType
		{
			get;
		}

		Type DeclaringType
		{
			get;
		}

		ITweakableManager Manager
		{
			get;
			set;
		}

		bool HasStep
		{
			get;
		}

		bool HasToggle
		{
			get;
		}

		bool HasRange
		{
			get;
		}

		IStepTweakable Step
		{
			get;
		}

		IToggleTweakable Toggle
		{
			get;
		}

		object MinValue
		{
			get;
		}

		object MaxValue
		{
			get;
		}

		event Action<object, object> ValueChanged;

		void SetValue(object value);

		object GetValue();
	}
}
