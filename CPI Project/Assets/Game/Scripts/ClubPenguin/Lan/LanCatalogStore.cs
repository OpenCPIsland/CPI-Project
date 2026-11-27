#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanCatalogStore
	{
		private sealed class Item
		{
			public long Id;

			public long Challenge;

			public string Swid;

			public string Json;

			public long Sold;

			public long Created;
		}

		private const int MaxItems = 500;

		private const int MaxItemBytes = 16384;

		private const int MaxDays = 60;

		private const double SaveDelayMs = 2000.0;

		private readonly LanServer server;

		private readonly string path;

		private readonly Dictionary<int, long> schedule = new Dictionary<int, long>();

		private readonly List<Item> items = new List<Item>();

		private bool saveQueued;

		public LanCatalogStore(LanServer server, string path)
		{
			this.server = server;
			this.path = path;
			Load();
		}

		public void SendState(LanServiceConnection conn)
		{
			conn.Send(ScheduleMessage(schedule));
			foreach (Item item in items)
			{
				conn.Send(ItemMessage(item));
			}
			conn.Send(LanServices.Message("cat_ready"));
		}

		public void Handle(LanServiceConnection conn, string cmd, object msg, Action<string> broadcast)
		{
			switch (cmd)
			{
			case "cat_theme":
				HandleTheme(msg, broadcast);
				break;
			case "cat_submit":
				HandleSubmit(conn, msg, broadcast);
				break;
			case "cat_buy":
				HandleBuy(msg, broadcast);
				break;
			}
		}

		private void HandleTheme(object msg, Action<string> broadcast)
		{
			int day;
			long id;
			if (!LanJson.TryInt(LanJson.Get(msg, "day"), out day) || !LanJson.TryLong(LanJson.Get(msg, "id"), out id) || day < 20000101 || day > 99991231 || id <= 0 || schedule.ContainsKey(day))
			{
				return;
			}
			schedule[day] = id;
			foreach (int old in schedule.Keys.OrderByDescending((int d) => d).Skip(MaxDays).ToList())
			{
				schedule.Remove(old);
			}
			Dictionary<int, long> one = new Dictionary<int, long>();
			one[day] = id;
			broadcast(ScheduleMessage(one));
			QueueSave();
		}

		private void HandleSubmit(LanServiceConnection conn, object msg, Action<string> broadcast)
		{
			long id;
			long challenge;
			string json = LanJson.Get(msg, "json") as string;
			object parsed;
			if (!LanJson.TryLong(LanJson.Get(msg, "id"), out id) || !LanJson.TryLong(LanJson.Get(msg, "challenge"), out challenge) || id <= 0 || challenge <= 0 || json == null || Encoding.UTF8.GetByteCount(json) > MaxItemBytes || !LanJson.TryParse(json, out parsed) || !(parsed is Dictionary<string, object>) || items.Any((Item x) => x.Id == id))
			{
				return;
			}
			Item item = new Item
			{
				Id = id,
				Challenge = challenge,
				Swid = conn.Swid,
				Json = json,
				Sold = 0L,
				Created = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
			};
			items.Add(item);
			while (items.Count > MaxItems)
			{
				items.RemoveAt(0);
			}
			broadcast(ItemMessage(item));
			QueueSave();
		}

		private void HandleBuy(object msg, Action<string> broadcast)
		{
			long id;
			if (!LanJson.TryLong(LanJson.Get(msg, "id"), out id))
			{
				return;
			}
			Item item = items.FirstOrDefault((Item x) => x.Id == id);
			if (item == null)
			{
				return;
			}
			item.Sold++;
			broadcast(LanServices.Message("cat_sold", "id", item.Id, "sold", item.Sold));
			QueueSave();
		}

		private static string ScheduleMessage(Dictionary<int, long> days)
		{
			Dictionary<string, object> map = new Dictionary<string, object>();
			foreach (KeyValuePair<int, long> kv in days)
			{
				map[kv.Key.ToString()] = kv.Value;
			}
			return LanServices.Message("cat_schedule", "days", map);
		}

		private static string ItemMessage(Item item)
		{
			return LanServices.Message("cat_item", "id", item.Id, "challenge", item.Challenge, "swid", item.Swid, "sold", item.Sold, "created", item.Created, "json", item.Json);
		}

		private void QueueSave()
		{
			if (saveQueued || string.IsNullOrEmpty(path))
			{
				return;
			}
			saveQueued = true;
			server.Loop.After(SaveDelayMs, Save);
		}

		public void Save()
		{
			saveQueued = false;
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			try
			{
				Dictionary<string, object> days = new Dictionary<string, object>();
				foreach (KeyValuePair<int, long> kv in schedule)
				{
					days[kv.Key.ToString()] = kv.Value;
				}
				List<object> list = new List<object>();
				foreach (Item item in items)
				{
					Dictionary<string, object> e = new Dictionary<string, object>();
					e["id"] = item.Id;
					e["challenge"] = item.Challenge;
					e["swid"] = item.Swid;
					e["json"] = item.Json;
					e["sold"] = item.Sold;
					e["created"] = item.Created;
					list.Add(e);
				}
				Dictionary<string, object> root = new Dictionary<string, object>();
				root["schedule"] = days;
				root["items"] = list;
				LanStore.Write(path, LanJson.Serialize(root));
			}
			catch (Exception)
			{
			}
		}

		private void Load()
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			try
			{
				string text = LanStore.Read(path);
				if (text == null)
				{
					return;
				}
				object root;
				if (!LanJson.TryParse(text, out root))
				{
					return;
				}
				Dictionary<string, object> days = LanJson.Get(root, "schedule") as Dictionary<string, object>;
				if (days != null)
				{
					foreach (KeyValuePair<string, object> kv in days)
					{
						int day;
						long id;
						if (int.TryParse(kv.Key, out day) && LanJson.TryLong(kv.Value, out id) && id > 0)
						{
							schedule[day] = id;
						}
					}
				}
				List<object> list = LanJson.Get(root, "items") as List<object>;
				if (list != null)
				{
					foreach (object e in list)
					{
						Item item = new Item();
						item.Swid = LanJson.Get(e, "swid") as string;
						item.Json = LanJson.Get(e, "json") as string;
						if (!LanJson.TryLong(LanJson.Get(e, "id"), out item.Id) || !LanJson.TryLong(LanJson.Get(e, "challenge"), out item.Challenge) || item.Json == null || item.Swid == null)
						{
							continue;
						}
						LanJson.TryLong(LanJson.Get(e, "sold"), out item.Sold);
						LanJson.TryLong(LanJson.Get(e, "created"), out item.Created);
						items.Add(item);
					}
					while (items.Count > MaxItems)
					{
						items.RemoveAt(0);
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
#endif
