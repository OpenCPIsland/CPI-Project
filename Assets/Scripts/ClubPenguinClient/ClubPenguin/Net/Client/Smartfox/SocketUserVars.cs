using Sfs2X.Entities.Variables;

namespace ClubPenguin.Net.Client.Smartfox
{
	public enum SocketUserVars
	{
		[SocketUserVar("outfit", VariableType.STRING)]
		OUTFIT,
		[SocketUserVar("proto", VariableType.STRING)]
		PROTOTYPE,
		[SocketUserVar("con", VariableType.OBJECT)]
		EQUIPPED_OBJECT,
		[SocketUserVar("bubble", VariableType.OBJECT)]
		AIR_BUBBLE,
		[SocketUserVar("con_prop", VariableType.STRING)]
		EQUIPPED_OBJECT_PROPERTIES,
		[SocketUserVar("sess", VariableType.STRING)]
		SESSION_ID,
		[SocketUserVar("loc", VariableType.INT)]
		LOCOMOTION_STATE,
		[SocketUserVar("colour", VariableType.STRING)]
		PROFILE,
		[SocketUserVar("on_quest", VariableType.STRING)]
		ON_QUEST,
		[SocketUserVar("awayfromkeyboard", VariableType.INT)]
		AWAY_FROM_KEYBOARD,
		[SocketUserVar("tube", VariableType.STRING)]
		SELECTED_TUBE,
		[SocketUserVar("tempheadstatus", VariableType.INT)]
		TEMPORARY_HEAD_STATUS
	}
}
