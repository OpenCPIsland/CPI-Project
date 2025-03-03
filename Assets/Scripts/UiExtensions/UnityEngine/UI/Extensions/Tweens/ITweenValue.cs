namespace UnityEngine.UI.Extensions.Tweens
{
	internal interface ITweenValue
	{
		bool ignoreTimeScale
		{
			get;
		}

		float duration
		{
			get;
		}

		void TweenValue(float floatPercentage);

		bool ValidTarget();

		void Finished();
	}
}
