using System;

namespace Disney.Mix.SDK.Internal
{
	public class SystemEnvironment : ISystemEnvironment
	{
		public int ProcessorCount
		{
			get
			{
				return Environment.ProcessorCount;
			}
		}
	}
}
