using System;

namespace Tweaker.Core
{
	public interface IStepTweakable
	{
		object StepSize
		{
			get;
		}

		Type TweakableType
		{
			get;
		}

		object StepNext();

		object StepPrevious();
	}
}
