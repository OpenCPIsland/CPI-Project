namespace NUnit.Framework.Api
{
	public class TestOutput
	{
		private string text;

		private TestOutputType type;

		public string Text
		{
			get
			{
				return text;
			}
		}

		public TestOutputType Type
		{
			get
			{
				return type;
			}
		}

		public TestOutput(string text, TestOutputType type)
		{
			this.text = text;
			this.type = type;
		}

		public override string ToString()
		{
			return string.Concat(type, ": ", text);
		}
	}
}
