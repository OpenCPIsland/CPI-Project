using System;
using System.IO;

namespace NUnit.Framework.Internal
{
	public class StackFilter
	{
		public static string Filter(string rawTrace)
		{
			if (rawTrace == null)
			{
				return null;
			}
			StringReader stringReader = new StringReader(rawTrace);
			StringWriter stringWriter = new StringWriter();
			try
			{
				string text;
				while ((text = stringReader.ReadLine()) != null && text.IndexOf("NUnit.Framework.Assert") >= 0)
				{
				}
				while (text != null)
				{
					stringWriter.WriteLine(text);
					text = stringReader.ReadLine();
				}
			}
			catch (Exception)
			{
				return rawTrace;
			}
			return stringWriter.ToString();
		}
	}
}
