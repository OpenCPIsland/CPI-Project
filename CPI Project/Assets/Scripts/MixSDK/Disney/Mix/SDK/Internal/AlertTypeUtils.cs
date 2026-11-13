namespace Disney.Mix.SDK.Internal
{
	public static class AlertTypeUtils
	{
		public static AlertType FromString(string str)
		{
			switch (str)
			{
			case "CYBERCRIMINAL":
				return AlertType.Cybercriminal;
			case "CYBERBULLY":
				return AlertType.Cyberbully;
			case "DATING_TEENS":
				return AlertType.DatingTeens;
			case "OFFENSIVE_USER":
				return AlertType.OffensiveUser;
			case "OVERSHARING_CHILD":
				return AlertType.OversharingChild;
			case "POTENTIAL_PAEDOPHILE":
				return AlertType.PotentialPaedophile;
			case "SEX_PEST":
				return AlertType.SexPest;
			case "SPAMMER":
				return AlertType.Spammer;
			case "TERRORIST":
				return AlertType.Terrorist;
			case "TROLL":
				return AlertType.Troll;
			case "VULNERABLE_PERSON":
				return AlertType.VulnerablePerson;
			default:
				return AlertType.Unknown;
			}
		}

		public static string ToString(AlertType type)
		{
			switch (type)
			{
			case AlertType.Cybercriminal:
				return "CYBERCRIMINAL";
			case AlertType.Cyberbully:
				return "CYBERBULLY";
			case AlertType.DatingTeens:
				return "DATING_TEENS";
			case AlertType.OffensiveUser:
				return "OFFENSIVE_USER";
			case AlertType.OversharingChild:
				return "OVERSHARING_CHILD";
			case AlertType.PotentialPaedophile:
				return "POTENTIAL_PAEDOPHILE";
			case AlertType.SexPest:
				return "SEX_PEST";
			case AlertType.Spammer:
				return "SPAMMER";
			case AlertType.Terrorist:
				return "TERRORIST";
			case AlertType.Troll:
				return "TROLL";
			case AlertType.VulnerablePerson:
				return "VULNERABLE_PERSON";
			default:
				return "Unknown";
			}
		}
	}
}
