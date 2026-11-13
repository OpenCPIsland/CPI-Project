using NUnit.Framework.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml;

namespace NUnit.Framework.Internal
{
	public abstract class TestResult : NUnit.Framework.Api.ITestResult, IXmlNodeBuilder
	{
		protected ResultState resultState;

		private double time = 0.0;

		protected readonly ITest test;

		private string stackTrace;

		protected string message;

		protected int assertCount = 0;

		private List<NUnit.Framework.Api.ITestResult> children;

		public ITest Test
		{
			get
			{
				return test;
			}
		}

		public ResultState ResultState
		{
			get
			{
				return resultState;
			}
		}

		public virtual string Name
		{
			get
			{
				return test.Name;
			}
		}

		public virtual string FullName
		{
			get
			{
				return test.FullName;
			}
		}

		public double Time
		{
			get
			{
				return time;
			}
			set
			{
				time = value;
			}
		}

		public string Message
		{
			get
			{
				return message;
			}
		}

		public virtual string StackTrace
		{
			get
			{
				return stackTrace;
			}
		}

		public int AssertCount
		{
			get
			{
				return assertCount;
			}
			set
			{
				assertCount = value;
			}
		}

		public abstract int FailCount
		{
			get;
		}

		public abstract int PassCount
		{
			get;
		}

		public abstract int SkipCount
		{
			get;
		}

		public abstract int InconclusiveCount
		{
			get;
		}

		public bool HasChildren
		{
			get
			{
				return children != null && children.Count > 0;
			}
		}

		public IList<NUnit.Framework.Api.ITestResult> Children
		{
			get
			{
				if (children == null)
				{
					children = new List<NUnit.Framework.Api.ITestResult>();
				}
				return children;
			}
		}

		public TestResult(ITest test)
		{
			this.test = test;
			resultState = ResultState.Inconclusive;
		}

		public XmlNode ToXml(bool recursive)
		{
			XmlNode xmlNode = XmlHelper.CreateTopLevelElement("dummy");
			AddToXml(xmlNode, recursive);
			return xmlNode.FirstChild;
		}

		public virtual XmlNode AddToXml(XmlNode parentNode, bool recursive)
		{
			XmlNode xmlNode = test.AddToXml(parentNode, false);
			XmlHelper.AddAttribute(xmlNode, "result", ResultState.Status.ToString());
			if (ResultState.Label != string.Empty)
			{
				XmlHelper.AddAttribute(xmlNode, "label", ResultState.Label);
			}
			XmlHelper.AddAttribute(xmlNode, "time", Time.ToString("0.000", CultureInfo.InvariantCulture));
			if (test is TestSuite)
			{
				XmlHelper.AddAttribute(xmlNode, "total", (PassCount + FailCount + SkipCount + InconclusiveCount).ToString());
				XmlHelper.AddAttribute(xmlNode, "passed", PassCount.ToString());
				XmlHelper.AddAttribute(xmlNode, "failed", FailCount.ToString());
				XmlHelper.AddAttribute(xmlNode, "inconclusive", InconclusiveCount.ToString());
				XmlHelper.AddAttribute(xmlNode, "skipped", SkipCount.ToString());
			}
			XmlHelper.AddAttribute(xmlNode, "asserts", AssertCount.ToString());
			switch (ResultState.Status)
			{
			case TestStatus.Failed:
				AddFailureElement(xmlNode);
				break;
			case TestStatus.Skipped:
				AddReasonElement(xmlNode);
				break;
			}
			if (recursive && HasChildren)
			{
				foreach (TestResult child in Children)
				{
					child.AddToXml(xmlNode, recursive);
				}
			}
			return xmlNode;
		}

		public void AddResult(NUnit.Framework.Api.ITestResult result)
		{
			if (children == null)
			{
				children = new List<NUnit.Framework.Api.ITestResult>();
			}
			children.Add(result);
			TestStatus status = result.ResultState.Status;
			if (status == TestStatus.Failed)
			{
				SetResult(ResultState.Failure, "Component test failure");
			}
		}

		public void SetResult(ResultState resultState)
		{
			SetResult(resultState, null, null);
		}

		public void SetResult(ResultState resultState, string message)
		{
			SetResult(resultState, message, null);
		}

		public void SetResult(ResultState resultState, string message, string stackTrace)
		{
			this.resultState = resultState;
			this.message = message;
			this.stackTrace = stackTrace;
		}

		public void RecordException(Exception ex)
		{
			if (ex is NUnitException)
			{
				ex = ex.InnerException;
			}
			if (ex is ThreadAbortException)
			{
				SetResult(ResultState.Cancelled, "Test cancelled by user", ex.StackTrace);
			}
			else if (ex is AssertionException)
			{
				SetResult(ResultState.Failure, ex.Message, StackFilter.Filter(ex.StackTrace));
			}
			else if (ex is IgnoreException)
			{
				SetResult(ResultState.Ignored, ex.Message, StackFilter.Filter(ex.StackTrace));
			}
			else if (ex is InconclusiveException)
			{
				SetResult(ResultState.Inconclusive, ex.Message, StackFilter.Filter(ex.StackTrace));
			}
			else if (ex is SuccessException)
			{
				SetResult(ResultState.Success, ex.Message, StackFilter.Filter(ex.StackTrace));
			}
			else
			{
				SetResult(ResultState.Error, ExceptionHelper.BuildMessage(ex), ExceptionHelper.BuildStackTrace(ex));
			}
		}

		private XmlNode AddReasonElement(XmlNode targetNode)
		{
			XmlNode xmlNode = XmlHelper.AddElement(targetNode, "reason");
			XmlHelper.AddElementWithCDataSection(xmlNode, "message", Message);
			return xmlNode;
		}

		private XmlNode AddFailureElement(XmlNode targetNode)
		{
			XmlNode xmlNode = XmlHelper.AddElement(targetNode, "failure");
			if (Message != null)
			{
				XmlHelper.AddElementWithCDataSection(xmlNode, "message", Message);
			}
			if (StackTrace != null)
			{
				XmlHelper.AddElementWithCDataSection(xmlNode, "stack-trace", StackTrace);
			}
			return xmlNode;
		}
	}
}
