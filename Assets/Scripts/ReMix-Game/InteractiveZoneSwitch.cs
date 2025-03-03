using ClubPenguin.Core;

public class InteractiveZoneSwitch : Switch
{
	public string ZoneId;

	public override object GetSwitchParameters()
	{
		return ZoneId;
	}

	public override string GetSwitchType()
	{
		return "interactiveZone";
	}
}
