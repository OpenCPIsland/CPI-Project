using System;
using System.Reflection;

namespace NUnit.Framework.Api
{
	public struct ExpectedExceptionData
	{
		private Type expectedExceptionType;

		private string expectedExceptionName;

		private string expectedMessage;

		private MessageMatch matchType;

		private string userMessage;

		private string handlerName;

		private MethodInfo exceptionHandler;

		public Type ExpectedExceptionType
		{
			get
			{
				return expectedExceptionType;
			}
			set
			{
				expectedExceptionType = value;
				expectedExceptionName = value.FullName;
			}
		}

		public string ExpectedExceptionName
		{
			get
			{
				return expectedExceptionName;
			}
			set
			{
				expectedExceptionName = value;
				expectedExceptionType = null;
			}
		}

		public string ExpectedMessage
		{
			get
			{
				return expectedMessage;
			}
			set
			{
				expectedMessage = value;
			}
		}

		public MessageMatch MatchType
		{
			get
			{
				return matchType;
			}
			set
			{
				matchType = value;
			}
		}

		public string UserMessage
		{
			get
			{
				return userMessage;
			}
			set
			{
				userMessage = value;
			}
		}

		public string HandlerName
		{
			get
			{
				return handlerName;
			}
			set
			{
				handlerName = value;
				exceptionHandler = null;
			}
		}

		public MethodInfo GetExceptionHandler(Type fixtureType)
		{
			if (exceptionHandler == null && handlerName != null)
			{
				exceptionHandler = fixtureType.GetMethod(handlerName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1]
				{
					typeof(Exception)
				}, null);
			}
			return exceptionHandler;
		}
	}
}
