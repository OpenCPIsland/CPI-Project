using System;
using System.Globalization;
using System.Text;

namespace NUnit.Framework.Internal
{
	public class ExceptionHelper
	{
		public static string BuildMessage(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0} : {1}", exception.GetType().ToString(), exception.Message);
			for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
			{
				stringBuilder.Append(Env.NewLine);
				stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "  ----> {0} : {1}", innerException.GetType().ToString(), innerException.Message);
			}
			return stringBuilder.ToString();
		}

		public static string BuildStackTrace(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(GetStackTrace(exception));
			for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
			{
				stringBuilder.Append(Env.NewLine);
				stringBuilder.Append("--");
				stringBuilder.Append(innerException.GetType().Name);
				stringBuilder.Append(Env.NewLine);
				stringBuilder.Append(GetStackTrace(innerException));
			}
			return stringBuilder.ToString();
		}

		private static string GetStackTrace(Exception exception)
		{
			try
			{
				return exception.StackTrace;
			}
			catch (Exception)
			{
				return "No stack trace available";
			}
		}
	}
}
