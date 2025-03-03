using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client.Smartfox
{
	public enum SmartfoxCommand
	{
		[SmartfoxCommands("chat.activity")]
		CHAT_ACTIVITY,
		[SmartfoxCommands("chat.activity_cancel")]
		CHAT_ACTIVITY_CANCEL,
		[SmartfoxCommands("chat.msg")]
		CHAT,
		[SmartfoxCommands("time.get")]
		GET_SERVER_TIME,
		[SmartfoxCommands("time.error")]
		SERVER_TIME_ERROR,
		[SmartfoxCommands("encryption.get")]
		GET_ROOM_ENCRYPTION_KEY,
		[SmartfoxCommands("encryption.error")]
		ENCRYPTION_KEY_ERROR,
		[SmartfoxCommands("proto")]
		PROTOTYPE,
		[SmartfoxCommands("quest.c_obj")]
		COMPLETE_OBJECTIVE,
		[SmartfoxCommands("quest.set")]
		SET_QUEST_STATES,
		[SmartfoxCommands("quest.s_obj")]
		SERVER_OBJECTIVE_COMPLETED,
		[SmartfoxCommands("quest.error")]
		SERVER_QUEST_ERROR,
		[SmartfoxCommands("con.set")]
		SET_CONSUMABLE_INVENTORY,
		[SmartfoxCommands("con.use")]
		USE_CONSUMABLE,
		[SmartfoxCommands("con.reuse")]
		REUSE_CONSUMABLE,
		[SmartfoxCommands("con.reuse_error")]
		REUSE_FAILED,
		[SmartfoxCommands("con.mmoitemdeployed")]
		CONSUMABLE_MMO_DEPLOYED,
		[SmartfoxCommands("con.partial")]
		SET_CONSUMABLE_PARTIAL_COUNT,
		[SmartfoxCommands("reward.add")]
		RECEIVED_REWARDS,
		[SmartfoxCommands("reward.add_delayed")]
		RECEIVED_REWARDS_DELAYED,
		[SmartfoxCommands("reward.room")]
		RECEIVED_ROOOM_REWARDS,
		[SmartfoxCommands("reward.levelup")]
		PROGRESSION_LEVELUP,
		[SmartfoxCommands("reward.broadcast")]
		SEND_EARNED_REWARDS,
		[SmartfoxCommands("room.igloo_updated")]
		IGLOO_UPDATED,
		[SmartfoxCommands("room.force_leave")]
		FORCE_LEAVE,
		[SmartfoxCommands("zone.logout")]
		LEAVE_ROOM,
		[SmartfoxCommands("zone.transfer")]
		ROOM_TRANSIENT_DATA,
		[SmartfoxCommands("zone.player_location")]
		GET_PLAYER_LOCATION,
		[SmartfoxCommands("zone.player_notfound")]
		PLAYER_NOT_FOUND,
		[SmartfoxCommands("l.a")]
		LOCOMOTION_ACTION,
		[SmartfoxCommands("task.p")]
		PICKUP,
		[SmartfoxCommands("task.c")]
		TASK_COUNT,
		[SmartfoxCommands("task.r")]
		TASK_PROGRESS,
		[SmartfoxCommands("membership.refresh")]
		MEMBERSHIP_REFRESH,
		[SmartfoxCommands("validate.state")]
		GET_SIGNED_STATE,
		[SmartfoxCommands("durable.equip")]
		EQUIP_DURABLE,
		[SmartfoxCommands("dispensable.equip")]
		EQUIP_DISPENSABLE,
		[SmartfoxCommands("wsevent.{0}")]
		WEBSERVICE_EVENT,
		[SmartfoxCommands("pgame.start")]
		PARTY_GAME_START,
		[SmartfoxCommands("pgame.start.v2")]
		PARTY_GAME_START_V2,
		[SmartfoxCommands("pgame.end")]
		PARTY_GAME_END,
		[SmartfoxCommands("pgame.message")]
		PARTY_GAME_MESSAGE
	}
	public static class SmartFoxCommand
	{
		private static Dictionary<string, SmartfoxCommand> enumMap;

		private static List<SmartfoxCommand> enumList;

		static SmartFoxCommand()
		{
			enumMap = new Dictionary<string, SmartfoxCommand>();
			enumList = new List<SmartfoxCommand>();
			foreach (SmartfoxCommand value in Enum.GetValues(typeof(SmartfoxCommand)))
			{
				enumMap.Add(value.GetCommand(), value);
				enumList.Add(value);
			}
		}

		public static SmartfoxCommand? FromString(string str)
		{
			if (!enumMap.ContainsKey(str))
			{
				return null;
			}
			return enumMap[str];
		}

		public static List<SmartfoxCommand> Values()
		{
			return enumList;
		}
	}
}
