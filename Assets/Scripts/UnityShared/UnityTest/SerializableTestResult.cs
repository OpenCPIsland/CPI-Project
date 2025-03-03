using System;

namespace UnityTest
{
	[Serializable]
	internal class SerializableTestResult : ITestResult
	{
		public TestResultState resultState;

		public string message;

		public bool executed;

		public string name;

		public string fullName;

		public string id;

		public bool isSuccess;

		public double duration;

		public string stackTrace;

		public bool isIgnored;

		public TestResultState ResultState
		{
			get
			{
				return resultState;
			}
		}

		public string Message
		{
			get
			{
				return message;
			}
		}

		public string Logs
		{
			get
			{
				return null;
			}
		}

		public bool Executed
		{
			get
			{
				return executed;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string FullName
		{
			get
			{
				return fullName;
			}
		}

		public string Id
		{
			get
			{
				return id;
			}
		}

		public bool IsSuccess
		{
			get
			{
				return isSuccess;
			}
		}

		public double Duration
		{
			get
			{
				return duration;
			}
		}

		public string StackTrace
		{
			get
			{
				return stackTrace;
			}
		}

		public bool IsIgnored
		{
			get
			{
				return isIgnored;
			}
		}
	}
}
