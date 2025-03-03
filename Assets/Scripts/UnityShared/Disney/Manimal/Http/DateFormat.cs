using System.Runtime.InteropServices;

namespace Disney.Manimal.Http
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct DateFormat
	{
		public const string Iso8601 = "s";

		public const string RoundTrip = "u";
	}
}
