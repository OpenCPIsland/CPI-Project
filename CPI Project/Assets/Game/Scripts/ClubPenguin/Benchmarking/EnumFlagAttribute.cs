using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class EnumFlagAttribute : PropertyAttribute
	{
		public string EnumName;

		public EnumFlagAttribute()
		{
		}

		public EnumFlagAttribute(string name)
		{
			EnumName = name;
		}
	}
}
