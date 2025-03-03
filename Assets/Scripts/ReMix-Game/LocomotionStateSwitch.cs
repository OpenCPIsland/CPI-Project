using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using System.Collections.Generic;

public class LocomotionStateSwitch : Switch
{
	public List<LocomotionState> states = new List<LocomotionState>();

	public override object GetSwitchParameters()
	{
		return states;
	}

	public override string GetSwitchType()
	{
		return "locomotionState";
	}
}
