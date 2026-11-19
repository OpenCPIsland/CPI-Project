#if !UNITY_WEBGL
using System;
using System.Linq;

namespace ClubPenguin.Net.Lan
{
	public sealed partial class LanServer
	{
		private const int MaxAnnouncementLength = 500;

		private const string SystemPrefix = "​";

		private const string SystemErrorPrefix = "​‌";

		private bool HandleCommand(LanPlayer player, string message)
		{
			if (!message.StartsWith("!", StringComparison.Ordinal))
			{
				return false;
			}
			string text = message.Substring(1);
			int space = text.IndexOf(' ');
			string name = ((space < 0) ? text : text.Substring(0, space)).ToLowerInvariant();
			string args = (space < 0) ? "" : text.Substring(space + 1).Trim();
			if (name.Length == 0 || !name.All(char.IsLetter))
			{
				return false;
			}
			switch (name)
			{
			case "announce":
				CommandAnnounce(player, args);
				break;
			default:
				SendSystemChat(player, "Unknown command: !" + name, true);
				break;
			}
			return true;
		}

		private void CommandAnnounce(LanPlayer player, string args)
		{
			if (args.Length == 0)
			{
				SendSystemChat(player, "Usage: !announce {message}", true);
				return;
			}
			if (args.Length > MaxAnnouncementLength)
			{
				args = args.Substring(0, MaxAnnouncementLength);
			}
			byte[] packet = Ext("server.alert", new SfsObject().PutString("message", args));
			foreach (LanPlayer target in players.ToArray())
			{
				target.Send(packet);
			}
		}

		private void SendSystemChat(LanPlayer player, string text, bool error)
		{
			string received = "{\"senderSessionId\":" + (player.PlayerSessionId ?? "0") + ",\"message\":" + LanJson.Quote((error ? SystemErrorPrefix : SystemPrefix) + text) + ",\"emotion\":0}";
			player.Send(Ext("chat.msg", new SfsObject().PutString("msg", received).PutInt("senderId", player.UserId)));
		}
	}
}
#endif
