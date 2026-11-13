using System;

namespace ClubPenguin.Benchmarking
{
	public struct BenchmarkLogger : IDisposable
	{
		public readonly string Name;

		public BenchmarkLogger(string name)
		{
			Name = name;
			Print("<" + Name + ">");
		}

		public void Log(string txt)
		{
			Print("<" + Name + "> " + txt);
		}

		public void Print(string txt)
		{
			Console.WriteLine(txt);
		}

		public void Dispose()
		{
			Print("</" + Name + ">");
		}
	}
}
