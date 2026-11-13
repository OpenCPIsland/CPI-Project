using NUnit.Framework.Api;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class ExpectedExceptionAttribute : NUnitAttribute
	{
		private ExpectedExceptionData exceptionData = default(ExpectedExceptionData);

		public Type ExpectedException
		{
			get
			{
				return exceptionData.ExpectedExceptionType;
			}
			set
			{
				exceptionData.ExpectedExceptionType = value;
			}
		}

		public string ExpectedExceptionName
		{
			get
			{
				return exceptionData.ExpectedExceptionName;
			}
			set
			{
				exceptionData.ExpectedExceptionName = value;
			}
		}

		public string ExpectedMessage
		{
			get
			{
				return exceptionData.ExpectedMessage;
			}
			set
			{
				exceptionData.ExpectedMessage = value;
			}
		}

		public string UserMessage
		{
			get
			{
				return exceptionData.UserMessage;
			}
			set
			{
				exceptionData.UserMessage = value;
			}
		}

		public MessageMatch MatchType
		{
			get
			{
				return exceptionData.MatchType;
			}
			set
			{
				exceptionData.MatchType = value;
			}
		}

		public string Handler
		{
			get
			{
				return exceptionData.HandlerName;
			}
			set
			{
				exceptionData.HandlerName = value;
			}
		}

		public ExpectedExceptionData ExceptionData
		{
			get
			{
				return exceptionData;
			}
		}

		public ExpectedExceptionAttribute()
		{
		}

		public ExpectedExceptionAttribute(Type exceptionType)
		{
			exceptionData.ExpectedExceptionType = exceptionType;
		}

		public ExpectedExceptionAttribute(string exceptionName)
		{
			exceptionData.ExpectedExceptionName = exceptionName;
		}
	}
}
