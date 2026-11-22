#if !UNITY_WEBGL
using System.Collections.Generic;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanTransientData
	{
		private sealed class Saved
		{
			public string EquippedConsumable;

			public int? ActionCount;

			public long SavedAt;
		}

		private const long TtlMs = 60000;

		private readonly LanServer server;

		private readonly Dictionary<string, Saved> saved = new Dictionary<string, Saved>();

		public LanTransientData(LanServer server)
		{
			this.server = server;
		}

		public void Send(LanPlayer player)
		{
			Saved s = new Saved();
			s.EquippedConsumable = player.EquippedConsumable;
			s.ActionCount = (player.EquippedConsumable != null) ? player.HeldActionCount : null;
			s.SavedAt = server.Loop.NowMs;
			if (player.Swid != null)
			{
				saved[player.Swid] = s;
			}
			player.HeldActionCount = null;
			player.EquippedConsumable = null;
			Dictionary<string, object> data = new Dictionary<string, object>();
			if (s.EquippedConsumable != null)
			{
				data["equippedConsumable"] = s.EquippedConsumable;
			}
			if (s.EquippedConsumable != null && s.ActionCount.HasValue)
			{
				data["equippedConsumableProperties"] = "{\"actionCount\":" + s.ActionCount.Value + "}";
			}
			Dictionary<string, object> payload = new Dictionary<string, object>
			{
				{
					"goodUntil",
					0L
				},
				{
					"signature",
					""
				},
				{
					"swid",
					player.Swid ?? ""
				},
				{
					"data",
					data
				}
			};
			player.Send(LanServer.Ext("zone.transfer", new SfsObject().PutString("d", LanJson.Serialize(payload))));
		}

		public void Restore(LanPlayer player)
		{
			if (player.Swid == null)
			{
				return;
			}
			Saved s;
			if (!saved.TryGetValue(player.Swid, out s))
			{
				return;
			}
			saved.Remove(player.Swid);
			if (server.Loop.NowMs - s.SavedAt > TtlMs)
			{
				return;
			}
			player.EquippedConsumable = s.EquippedConsumable;
			player.HeldObject = s.EquippedConsumable;
			if (player.EquippedConsumable != null && s.ActionCount.HasValue)
			{
				player.HeldActionCount = s.ActionCount;
			}
		}
	}
}
#endif
