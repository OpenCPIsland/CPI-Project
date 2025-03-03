namespace UnityTest
{
	public interface IAssertionComponentConfigurator
	{
		int UpdateCheckStartOnFrame
		{
			set;
		}

		int UpdateCheckRepeatFrequency
		{
			set;
		}

		bool UpdateCheckRepeat
		{
			set;
		}

		float TimeCheckStartAfter
		{
			set;
		}

		float TimeCheckRepeatFrequency
		{
			set;
		}

		bool TimeCheckRepeat
		{
			set;
		}

		AssertionComponent Component
		{
			get;
		}
	}
}
