using System.IO;
using System.Text;

namespace NUnitLite.Runner
{
	internal class TcpWriter : TextWriter
	{
		public override Encoding Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		public TcpWriter(string hostName, int port)
		{
		}

		public override void Write(char value)
		{
		}

		public override void Write(string value)
		{
		}

		public override void WriteLine(string value)
		{
		}
	}
}
