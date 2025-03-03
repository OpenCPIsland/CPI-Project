using Sfs2X.Entities.Variables;

namespace ClubPenguin.Net.Client.Smartfox
{
	public enum SocketItemVars
	{
		[SocketItemVars("category", VariableType.INT)]
		CATEGORY,
		[SocketItemVars("template", VariableType.STRING)]
		TEMPLATE,
		[SocketItemVars("creator", VariableType.INT)]
		CREATOR,
		[SocketItemVars("type", VariableType.STRING)]
		TYPE,
		[SocketItemVars("ttl", VariableType.DOUBLE)]
		TIME_TO_LIVE,
		[SocketItemVars("actioncount", VariableType.INT)]
		ACTION_COUNT,
		[SocketItemVars("scheduledworldobjectstate", VariableType.INT)]
		SCHEDULED_WORLD_OBJECT_STATE,
		[SocketItemVars("statetimestamp", VariableType.STRING)]
		STATE_TIMESTAMP,
		[SocketItemVars("timestamp", VariableType.STRING)]
		TIMESTAMP,
		[SocketItemVars("path", VariableType.STRING)]
		GAME_OBJECT_PATH,
		[SocketItemVars("int_a", VariableType.INT)]
		INTEGER_A,
		[SocketItemVars("score", VariableType.STRING)]
		SCORE_DATA,
		[SocketItemVars("turn", VariableType.STRING)]
		TURN_DATA,
		[SocketItemVars("dancemoves", VariableType.STRING)]
		DANCE_MOVE_DATA
	}
}
