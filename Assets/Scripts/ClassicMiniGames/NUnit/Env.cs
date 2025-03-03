using System;

namespace NUnit
{
	public class Env
	{
		public static readonly string NewLine = Environment.NewLine;

		public static string DocumentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
	}
}
