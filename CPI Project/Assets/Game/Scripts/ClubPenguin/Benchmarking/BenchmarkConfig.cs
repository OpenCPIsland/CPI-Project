using Disney.Kelowna.Common.Environment;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkConfig : ScriptableObject
	{
		public string UserName;

		public string Password;

		public Environment ServerEnvironment = Environment.QA;

		public BenchmarkTest[] Tests;
	}
}
