namespace Disney.Mix.SDK.Internal
{
	public static class AgeBandTypeConverter
	{
		public static AgeBandType Convert(string ageBand)
		{
			switch (ageBand)
			{
			case "ADULT":
				return AgeBandType.Adult;
			case "TEEN":
				return AgeBandType.Teen;
			case "CHILD":
				return AgeBandType.Child;
			default:
				return AgeBandType.Unknown;
			}
		}

		public static string Convert(AgeBandType ageBand)
		{
			switch (ageBand)
			{
			case AgeBandType.Adult:
				return "ADULT";
			case AgeBandType.Teen:
				return "TEEN";
			case AgeBandType.Child:
				return "CHILD";
			default:
				return "UNKNOWN";
			}
		}
	}
}
