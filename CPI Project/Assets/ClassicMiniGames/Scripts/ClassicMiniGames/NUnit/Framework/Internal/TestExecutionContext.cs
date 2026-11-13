using NUnit.Framework.Api;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace NUnit.Framework.Internal
{
	public class TestExecutionContext
	{
		public TestExecutionContext prior;

		private Test currentTest;

		private TestResult currentResult;

		private string workDirectory;

		private object testObject;

		private ITestListener listener = TestListener.NULL;

		private int assertCount;

		private bool stopOnError;

		private TextWriter outWriter;

		private TextWriter errorWriter;

		private bool tracing;

		private TextWriter traceWriter;

		private CultureInfo currentCulture;

		private CultureInfo currentUICulture;

		private static TestExecutionContext current = new TestExecutionContext();

		public static TestExecutionContext CurrentContext
		{
			get
			{
				return current;
			}
		}

		public Test CurrentTest
		{
			get
			{
				return currentTest;
			}
			set
			{
				currentTest = value;
			}
		}

		public TestResult CurrentResult
		{
			get
			{
				return currentResult;
			}
			set
			{
				currentResult = value;
			}
		}

		public object TestObject
		{
			get
			{
				return testObject;
			}
			set
			{
				testObject = value;
			}
		}

		public string WorkDirectory
		{
			get
			{
				return workDirectory;
			}
			set
			{
				workDirectory = value;
			}
		}

		public bool StopOnError
		{
			get
			{
				return stopOnError;
			}
			set
			{
				stopOnError = value;
			}
		}

		public ITestListener Listener
		{
			get
			{
				return listener;
			}
			set
			{
				listener = value;
			}
		}

		public int AssertCount
		{
			get
			{
				return assertCount;
			}
		}

		public TextWriter Out
		{
			get
			{
				return outWriter;
			}
			set
			{
				if (outWriter != value)
				{
					outWriter = value;
					Console.Out.Flush();
					Console.SetOut(outWriter);
				}
			}
		}

		public TextWriter Error
		{
			get
			{
				return errorWriter;
			}
			set
			{
				if (errorWriter != value)
				{
					errorWriter = value;
					Console.Error.Flush();
					Console.SetError(errorWriter);
				}
			}
		}

		public bool Tracing
		{
			get
			{
				return tracing;
			}
			set
			{
				if (tracing != value)
				{
					if (traceWriter != null && tracing)
					{
						StopTracing();
					}
					tracing = value;
					if (traceWriter != null && tracing)
					{
						StartTracing();
					}
				}
			}
		}

		public TextWriter TraceWriter
		{
			get
			{
				return traceWriter;
			}
			set
			{
				if (traceWriter != value)
				{
					if (traceWriter != null && tracing)
					{
						StopTracing();
					}
					traceWriter = value;
					if (traceWriter != null && tracing)
					{
						StartTracing();
					}
				}
			}
		}

		public CultureInfo CurrentCulture
		{
			get
			{
				return currentCulture;
			}
			set
			{
				currentCulture = value;
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		public CultureInfo CurrentUICulture
		{
			get
			{
				return currentUICulture;
			}
			set
			{
				currentUICulture = value;
				Thread.CurrentThread.CurrentUICulture = currentUICulture;
			}
		}

		public TestExecutionContext()
		{
			prior = null;
			outWriter = Console.Out;
			errorWriter = Console.Error;
			traceWriter = null;
			tracing = false;
			currentCulture = CultureInfo.CurrentCulture;
			currentUICulture = CultureInfo.CurrentUICulture;
		}

		public TestExecutionContext(TestExecutionContext other)
		{
			prior = other;
			currentTest = other.currentTest;
			currentResult = other.currentResult;
			testObject = other.testObject;
			workDirectory = other.workDirectory;
			listener = other.listener;
			stopOnError = other.stopOnError;
			outWriter = other.outWriter;
			errorWriter = other.errorWriter;
			traceWriter = other.traceWriter;
			tracing = other.tracing;
			currentCulture = CultureInfo.CurrentCulture;
			currentUICulture = CultureInfo.CurrentUICulture;
		}

		public static void Save()
		{
			current = new TestExecutionContext(current);
		}

		public static void Restore()
		{
			current.ReverseChanges();
			int num = current.AssertCount;
			current = current.prior;
			current.assertCount += num;
		}

		private void StopTracing()
		{
			traceWriter.Close();
			Trace.Listeners.Remove("NUnit");
		}

		private void StartTracing()
		{
			Trace.Listeners.Add(new TextWriterTraceListener(traceWriter, "NUnit"));
		}

		public void ReverseChanges()
		{
			if (prior == null)
			{
				throw new InvalidOperationException("TestContext: too many Restores");
			}
			Out = prior.Out;
			Error = prior.Error;
			Tracing = prior.Tracing;
			CurrentCulture = prior.CurrentCulture;
			CurrentUICulture = prior.CurrentUICulture;
		}

		public void Update()
		{
			currentCulture = CultureInfo.CurrentCulture;
			currentUICulture = CultureInfo.CurrentUICulture;
		}

		public void IncrementAssertCount()
		{
			Interlocked.Increment(ref assertCount);
		}
	}
}
