using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClubPenguin.Net.Client
{
	public static class LanCatalog
	{
		private sealed class Item
		{
			public long Id;

			public long Challenge;

			public string Swid;

			public long Sold;

			public long Created;

			public CatalogItemData Data;
		}

		private static readonly object sync = new object();

		private static readonly Dictionary<int, long> schedule = new Dictionary<int, long>();

		private static readonly Dictionary<long, Item> items = new Dictionary<long, Item>();

		private static readonly Random random = new Random();

		private static bool ready;

		public static Action<string, Dictionary<string, object>> Send;

		public static bool Active
		{
			get
			{
				return ready && Send != null && LanDirectory.Active;
			}
		}

		public static void Reset()
		{
			lock (sync)
			{
				ready = false;
				schedule.Clear();
				items.Clear();
			}
		}

		public static void SetReady()
		{
			lock (sync)
			{
				ready = true;
			}
		}

		public static void SetScheduleDay(int day, long id)
		{
			lock (sync)
			{
				schedule[day] = id;
			}
		}

		public static void AddItem(long id, long challenge, string swid, long sold, long created, string json)
		{
			CatalogItemData data;
			try
			{
				data = Service.Get<JsonService>().Deserialize<CatalogItemData>(json);
			}
			catch (Exception)
			{
				return;
			}
			data.clothingCatalogItemId = id;
			lock (sync)
			{
				items[id] = new Item
				{
					Id = id,
					Challenge = challenge,
					Swid = swid,
					Sold = sold,
					Created = created,
					Data = data
				};
			}
		}

		public static void SetSold(long id, long sold)
		{
			lock (sync)
			{
				Item item;
				if (items.TryGetValue(id, out item))
				{
					item.Sold = sold;
				}
			}
		}

		public static long[] Schedule(int[] dayStamps, long[] computed)
		{
			long[] result = new long[dayStamps.Length];
			for (int i = 0; i < dayStamps.Length; i++)
			{
				long shared;
				bool known;
				lock (sync)
				{
					known = schedule.TryGetValue(dayStamps[i], out shared);
				}
				if (known)
				{
					result[i] = shared;
					continue;
				}
				result[i] = computed[i];
				if (computed[i] > 0)
				{
					SetScheduleDay(dayStamps[i], computed[i]);
					Dictionary<string, object> args = new Dictionary<string, object>();
					args["day"] = dayStamps[i];
					args["id"] = computed[i];
					Send("cat_theme", args);
				}
			}
			return result;
		}

		private static CatalogItemData ToData(Item item)
		{
			CatalogItemData data = item.Data;
			data.clothingCatalogItemId = item.Id;
			data.numberSold = item.Sold;
			return data;
		}

		private static List<Item> Snapshot()
		{
			lock (sync)
			{
				return items.Values.ToList();
			}
		}

		public static List<CatalogItemData> ItemsFor(long challenge, bool friendsOnly)
		{
			return (from x in Snapshot()
				where x.Challenge == challenge && (!friendsOnly || LanDirectory.IsFriend(x.Swid))
				select ToData(x)).ToList();
		}

		public static CatalogItemData? MostPopular(long challenge)
		{
			Item best = null;
			foreach (Item item in Snapshot())
			{
				if ((challenge <= 0 || item.Challenge == challenge) && (best == null || item.Sold > best.Sold))
				{
					best = item;
				}
			}
			if (best == null)
			{
				return null;
			}
			return ToData(best);
		}

		public static long UserSubmission(long challenge, string swid)
		{
			Item latest = null;
			foreach (Item item in Snapshot())
			{
				if (item.Challenge == challenge && item.Swid == swid && (latest == null || item.Created > latest.Created))
				{
					latest = item;
				}
			}
			return (latest != null) ? latest.Id : 0L;
		}

		public static long NewItemId()
		{
			byte[] bytes = new byte[8];
			lock (sync)
			{
				random.NextBytes(bytes);
			}
			long id = BitConverter.ToInt64(bytes, 0) & long.MaxValue;
			return (id == 0) ? 1 : id;
		}

		public static void Submit(long challenge, string swid, CatalogItemData data)
		{
			long created = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			lock (sync)
			{
				items[data.clothingCatalogItemId] = new Item
				{
					Id = data.clothingCatalogItemId,
					Challenge = challenge,
					Swid = swid,
					Sold = 0L,
					Created = created,
					Data = data
				};
			}
			Dictionary<string, object> args = new Dictionary<string, object>();
			args["id"] = data.clothingCatalogItemId;
			args["challenge"] = challenge;
			args["json"] = Service.Get<JsonService>().Serialize(data);
			Send("cat_submit", args);
		}

		public static CatalogItemData? Find(long id)
		{
			lock (sync)
			{
				Item item;
				if (items.TryGetValue(id, out item))
				{
					return ToData(item);
				}
			}
			return null;
		}

		public static void Purchase(long id)
		{
			lock (sync)
			{
				Item item;
				if (items.TryGetValue(id, out item))
				{
					item.Sold++;
				}
			}
			Dictionary<string, object> args = new Dictionary<string, object>();
			args["id"] = id;
			Send("cat_buy", args);
		}

		public static long SoldBy(string swid)
		{
			long total = 0L;
			foreach (Item item in Snapshot())
			{
				if (item.Swid == swid)
				{
					total += item.Sold;
				}
			}
			return total;
		}

		public static List<CatalogItemData> ItemsBy(string swid, long sinceMs)
		{
			return (from x in Snapshot()
				where x.Swid == swid && x.Created >= sinceMs
				orderby x.Created descending
				select ToData(x)).ToList();
		}
	}
}
