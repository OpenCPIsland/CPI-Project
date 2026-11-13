using System;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class TestResult : ITestResult, IComparable<TestResult>
	{
		public enum ResultType
		{
			Success,
			Failed,
			Timeout,
			NotRun,
			FailedException,
			Ignored
		}

		private readonly GameObject m_Go;

		private string m_Name;

		public ResultType resultType = ResultType.NotRun;

		public double duration;

		public string messages;

		public string stacktrace;

		public string id;

		public bool dynamicTest;

		public TestComponent TestComponent;

		public GameObject GameObject
		{
			get
			{
				return m_Go;
			}
		}

		public TestResultState ResultState
		{
			get
			{
				switch (resultType)
				{
				case ResultType.Success:
					return TestResultState.Success;
				case ResultType.Failed:
					return TestResultState.Failure;
				case ResultType.FailedException:
					return TestResultState.Error;
				case ResultType.Ignored:
					return TestResultState.Ignored;
				case ResultType.NotRun:
					return TestResultState.Skipped;
				case ResultType.Timeout:
					return TestResultState.Cancelled;
				default:
					throw new Exception();
				}
			}
		}

		public string Message
		{
			get
			{
				return messages;
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
				return resultType != ResultType.NotRun;
			}
		}

		public string Name
		{
			get
			{
				if (m_Go != null)
				{
					m_Name = m_Go.name;
				}
				return m_Name;
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
				return resultType == ResultType.Success;
			}
		}

		public bool IsTimeout
		{
			get
			{
				return resultType == ResultType.Timeout;
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
				return stacktrace;
			}
		}

		public string FullName
		{
			get
			{
				string text = Name;
				if (m_Go != null)
				{
					Transform parent = m_Go.transform.parent;
					while (parent != null)
					{
						text = parent.name + "." + text;
						parent = parent.transform.parent;
					}
				}
				return text;
			}
		}

		public bool IsIgnored
		{
			get
			{
				return resultType == ResultType.Ignored;
			}
		}

		public bool IsFailure
		{
			get
			{
				return resultType == ResultType.Failed || resultType == ResultType.FailedException || resultType == ResultType.Timeout;
			}
		}

		public TestResult(TestComponent testComponent)
		{
			TestComponent = testComponent;
			m_Go = testComponent.gameObject;
			id = testComponent.gameObject.GetInstanceID().ToString();
			dynamicTest = testComponent.dynamic;
			if (m_Go != null)
			{
				m_Name = m_Go.name;
			}
			if (dynamicTest)
			{
				id = testComponent.dynamicTypeName;
			}
		}

		public void Update(TestResult oldResult)
		{
			resultType = oldResult.resultType;
			duration = oldResult.duration;
			messages = oldResult.messages;
			stacktrace = oldResult.stacktrace;
		}

		public void Reset()
		{
			resultType = ResultType.NotRun;
			duration = 0.0;
			messages = "";
			stacktrace = "";
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		public int CompareTo(TestResult other)
		{
			int num = Name.CompareTo(other.Name);
			if (num == 0)
			{
				num = m_Go.GetInstanceID().CompareTo(other.m_Go.GetInstanceID());
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (obj is TestResult)
			{
				return GetHashCode() == obj.GetHashCode();
			}
			return base.Equals(obj);
		}
	}
}
