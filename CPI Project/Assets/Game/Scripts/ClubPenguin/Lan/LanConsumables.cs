#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanConsumableDef
	{
		public string Experience;

		public double TimeToLive;

		public int ActionCount;

		public double RewardRadius;

		public long TotalCoins;

		public long MinCoins;

		public bool QuestOnly;

		public bool ServerAdded;

		public bool Shareable;

		public int? TotalItemQuantity;

		public string Recipient;
	}

	public sealed class LanConsumables
	{
		private sealed class LiveConsumable
		{
			public int ItemId;

			public string RoomKey;

			public string Type;

			public int CreatorUserId;

			public float[] Position;

			public LanConsumableDef Def;

			public double TimeToLive;

			public int? ActionCount;
		}

		private const int ItemCategoryConsumable = 1;

		private const int RewardSourceWorldObject = 1;

		private const int LocomotionInteract = 2;

		private const long TakeCooldownMs = 2000;

		private const double EatenItemRemoveDelayMs = 4000;

		private const double AutoStartSeconds = 30;

		private const double InvitationHandoffMs = 1000;

		private static readonly Dictionary<string, LanConsumableDef> Catalog = BuildCatalog();

		private readonly LanServer server;

		private readonly Dictionary<int, LiveConsumable> liveItems = new Dictionary<int, LiveConsumable>();

		private readonly Dictionary<int, List<LanPlayer>> invitationGuests = new Dictionary<int, List<LanPlayer>>();

		private readonly Dictionary<int, LanTimer> autoStartTimers = new Dictionary<int, LanTimer>();

		private readonly Dictionary<int, bool> handoffCancelled = new Dictionary<int, bool>();

		public LanConsumables(LanServer server)
		{
			this.server = server;
		}

		private static LanConsumableDef OneShot(bool questOnly = false, bool serverAdded = false)
		{
			LanConsumableDef d = new LanConsumableDef();
			d.Experience = "oneShot";
			d.QuestOnly = questOnly;
			d.ServerAdded = serverAdded;
			return d;
		}

		private static LanConsumableDef Held(bool shareable, int? quantity, string recipient, bool questOnly = false)
		{
			LanConsumableDef d = new LanConsumableDef();
			d.Experience = "playerHeld";
			d.Shareable = shareable;
			d.TotalItemQuantity = quantity;
			d.Recipient = recipient;
			d.QuestOnly = questOnly;
			return d;
		}

		private static LanConsumableDef ServerAddedHeld()
		{
			LanConsumableDef d = new LanConsumableDef();
			d.Experience = "playerHeld";
			d.ServerAdded = true;
			return d;
		}

		private static LanConsumableDef World(double ttl, int actionCount, double radius, long total, long min)
		{
			LanConsumableDef d = new LanConsumableDef();
			d.Experience = "world";
			d.TimeToLive = ttl;
			d.ActionCount = actionCount;
			d.RewardRadius = radius;
			d.TotalCoins = total;
			d.MinCoins = min;
			return d;
		}

		private static LanConsumableDef Lobby(int? quantity)
		{
			LanConsumableDef d = new LanConsumableDef();
			d.Experience = "partyGameLobby";
			d.TotalItemQuantity = quantity;
			return d;
		}

		private static Dictionary<string, LanConsumableDef> BuildCatalog()
		{
			Dictionary<string, LanConsumableDef> c = new Dictionary<string, LanConsumableDef>();
			c["BirthdayCakeGroup"] = Held(true, 6, "BirthdayCakeServerAdded");
			c["BirthdayCakeServerAdded"] = ServerAddedHeld();
			c["BirthdayCakeSingle"] = OneShot();
			c["Firework"] = OneShot();
			c["HotChocolateSingle"] = OneShot();
			c["HotChocolateServerAdded"] = ServerAddedHeld();
			c["HotChocolateGroup"] = Held(true, 6, "HotChocolateServerAdded");
			c["PizzaGroup"] = Held(true, 6, "PizzaServerAdded");
			c["PartyBlaster"] = World(5, 0, 2, 25, 1);
			c["Pinata"] = World(30, 5, 3.5, 50, 1);
			c["PizzaServerAdded"] = ServerAddedHeld();
			c["PizzaSingle"] = OneShot();
			c["QuestFirework"] = OneShot(true);
			c["QuestStinkyCheeseTray"] = Held(true, 6, "StinkyCheeseSlice", true);
			c["SquidStickServerAdded"] = ServerAddedHeld();
			c["SquidStickSingle"] = OneShot();
			c["SquidStickGroup"] = Held(true, 6, "SquidStickServerAdded");
			c["StinkyCheeseServerAdded"] = ServerAddedHeld();
			c["StinkyCheeseTray"] = Held(true, 6, "StinkyCheeseServerAdded");
			c["SeaweedSmoothieSingle"] = OneShot();
			c["MegaPartyBlaster"] = World(5, 0, 3.5, 100, 1);
			c["SuperPartyBlaster"] = World(5, 0, 2.5, 50, 1);
			c["GlowStickYellow"] = OneShot();
			c["GlowStickPink"] = OneShot();
			c["GlowStickRainbow"] = OneShot();
			c["FishdogSingle"] = OneShot();
			c["FishdogGroup"] = Held(true, 5, "FishdogServerAdded");
			c["FishdogServerAdded"] = ServerAddedHeld();
			c["GlowstickPack"] = Held(true, 6, "GlowStickServerAdded");
			c["GlowStickServerAdded"] = ServerAddedHeld();
			c["StinkyCheeseSingle"] = OneShot();
			c["PizzaPlanetGroup"] = Held(true, 6, "PizzaPlanetServerAdded");
			c["PizzaPlanetServerAdded"] = ServerAddedHeld();
			c["ZootopiaPawpsicle"] = OneShot();
			c["FindingDoryBubbleWand"] = OneShot();
			c["MonstersScareCanister"] = OneShot();
			c["FrozenSnowflake"] = OneShot();
			c["SuperFirework"] = OneShot();
			c["FireHydrant"] = OneShot();
			c["SeaweedSmoothieTray"] = Held(true, 4, "SeaweedSmoothieServerAdded");
			c["SeaweedSmoothieServerAdded"] = ServerAddedHeld();
			c["ScavengerHuntGroup"] = Lobby(null);
			c["FishBucketGroup"] = Lobby(3);
			c["FindFourGroup"] = Lobby(null);
			c["SpicyPizzaGroup"] = Held(true, 6, "SpicyPizzaServerAdded");
			c["SpicyPizzaServerAdded"] = ServerAddedHeld();
			c["SpicyPizzaSingle"] = OneShot();
			c["HalloweenBasketTray"] = Held(true, 3, "HalloweenBasketServerAdded");
			c["HalloweenBasketServerAdded"] = ServerAddedHeld();
			c["HalloweenBasketOrange"] = OneShot();
			c["HalloweenBasketPurple"] = OneShot();
			c["HalloweenBasketGreen"] = OneShot();
			c["GlowStickRainbowNonMember"] = OneShot();
			c["HotChocolateGroupNonMember"] = Held(true, 6, "HotChocolateServerAdded");
			c["PizzaGroupNonMember"] = Held(true, 6, "PizzaServerAdded");
			c["SquidStickGroupNonMember"] = Held(true, 6, "SquidStickServerAdded");
			c["SuperFireworkNonMember"] = OneShot();
			c["HolidayPartyBlaster"] = World(5, 0, 2, 25, 1);
			c["HolidayFirework"] = OneShot();
			c["CandyCaneSingle"] = OneShot();
			c["CandyCane"] = Held(true, 4, "CandyCaneServerAdded");
			c["CandyCaneServerAdded"] = ServerAddedHeld();
			c["FluffyCakeSingle"] = OneShot();
			c["FluffyCakeTray"] = Held(true, 6, "FluffyCakeServerAdded");
			c["FluffyCakeServerAdded"] = ServerAddedHeld();
			c["RainbowSmoothieSingle"] = OneShot();
			c["RainbowSmoothieTray"] = Held(true, 4, "RainbowSmoothieServerAdded");
			c["RainbowSmoothieServerAdded"] = ServerAddedHeld();
			c["Science_BeakerSingle"] = OneShot();
			c["HotChocolateSingleNonMember"] = OneShot();
			c["BirthdayCakeSingleNonMember"] = OneShot();
			c["PizzaSingleNonMember"] = OneShot();
			c["FireworkNonMember"] = OneShot();
			c["FishdogSingleNonMember"] = OneShot();
			c["BirthdayCakeGroupNonMember"] = Held(true, 6, "BirthdayCakeServerAdded");
			c["PinataNonMember"] = World(30, 5, 3.5, 50, 1);
			c["PartyBlasterNonMember"] = World(5, 0, 2, 25, 1);
			c["MegaPartyBlasterNonMember"] = World(5, 0, 3.5, 100, 1);
			c["FireHydrantNonMember"] = OneShot();
			c["Science_BeakerSingle_NonMember"] = OneShot();
			c["IceCreamTray"] = Held(true, 3, "IceCreamServerAdded");
			c["IceCreamServerAdded"] = ServerAddedHeld();
			c["IceCreamSingle"] = OneShot();
			c["Popscorn"] = OneShot();
			c["PopscornServerAdded"] = ServerAddedHeld();
			c["PopscornTray"] = Held(true, 3, "PopscornServerAdded");
			c["20thBirthdayCakeSingle"] = OneShot();
			c["20thBirthdayCakeGroup"] = Held(true, 6, "20thBirthdayCakeServerAdded");
			c["GlowStickMint"] = OneShot();
			c["Science_BeakerGreenSingle"] = OneShot();
			c["PizzaPlanetSingle"] = Held(false, 1, "PizzaPlanetSingleServerAdded");
			c["HalloweenPartyBlaster"] = World(5, 0, 2.5, 50, 1);
			c["Science_BeakerServerAdded"] = OneShot(false, true);
			c["Science_BeakerGroup"] = Held(true, 4, "Science_BeakerServerAdded");
			c["Science_BeakerGreenServerAdded"] = OneShot(false, true);
			c["Science_BeakerGreenGroup"] = Held(true, 4, "Science_BeakerGreenServerAdded");
			c["20thBirthdayCakeServerAdded"] = ServerAddedHeld();
			return c;
		}

		public static LanConsumableDef Get(string name)
		{
			LanConsumableDef d;
			return (name != null && Catalog.TryGetValue(name, out d)) ? d : null;
		}

		private static float[] ParsePosition(string propJson, LanPlayer player)
		{
			object prop;
			if (propJson != null && LanJson.TryParse(propJson, out prop))
			{
				double x;
				double y;
				double z;
				if (LanJson.TryNumber(LanJson.Get(prop, "x") ?? LanJson.Get(prop, "X"), out x) && LanJson.TryNumber(LanJson.Get(prop, "y") ?? LanJson.Get(prop, "Y"), out y) && LanJson.TryNumber(LanJson.Get(prop, "z") ?? LanJson.Get(prop, "Z"), out z))
				{
					return new float[3]
					{
						(float)x,
						(float)y,
						(float)z
					};
				}
			}
			float[] last = player.LastPosition;
			return (last != null) ? new float[3]
			{
				last[0],
				last[1],
				last[2]
			} : new float[3];
		}

		private void BroadcastDequip(LanPlayer player)
		{
			server.BroadcastUserVars(player, LanServer.NullVar("con"));
			player.EquippedItemName = null;
			player.EquippedConsumable = null;
			player.HeldActionCount = null;
			player.HeldObject = null;
		}

		private static SfsArray ItemVar(string name, byte kind, SfsValue v)
		{
			return new SfsArray().AddString(name).AddByte(kind).Add(v);
		}

		private static SfsArray EncodeItem(LiveConsumable item)
		{
			SfsArray vars = new SfsArray().AddArray(ItemVar("category", SfsVar.Int, new SfsValue(SfsType.Int, ItemCategoryConsumable))).AddArray(ItemVar("template", SfsVar.String, new SfsValue(SfsType.UtfString, item.ActionCount.HasValue ? "actioned" : "timed"))).AddArray(ItemVar("creator", SfsVar.Int, new SfsValue(SfsType.Int, item.CreatorUserId)))
				.AddArray(ItemVar("type", SfsVar.String, new SfsValue(SfsType.UtfString, item.Type)))
				.AddArray(ItemVar("ttl", SfsVar.Double, new SfsValue(SfsType.Double, item.TimeToLive)));
			if (item.ActionCount.HasValue)
			{
				vars.AddArray(ItemVar("actioncount", SfsVar.Int, new SfsValue(SfsType.Int, item.ActionCount.Value)));
			}
			return new SfsArray().AddInt(item.ItemId).AddArray(vars).AddFloats(item.Position);
		}

		private void BroadcastItemVariable(LiveConsumable item, SfsArray variable)
		{
			server.SendToRoom(item.RoomKey, LanServer.Sys(1007, new SfsObject().PutInt("r", LanServer.RoomId).PutInt("i", item.ItemId).PutArray("v", new SfsArray().AddArray(variable))));
		}

		public void SendSnapshot(LanPlayer player)
		{
			List<LiveConsumable> items = liveItems.Values.Where((LiveConsumable i) => i.RoomKey == player.RoomKey).ToList();
			if (items.Count == 0)
			{
				return;
			}
			SfsArray q = new SfsArray();
			foreach (LiveConsumable item in items)
			{
				q.AddArray(EncodeItem(item));
			}
			player.Send(LanServer.Sys(30, new SfsObject().PutInt("r", LanServer.RoomId).PutArray("q", q)));
		}

		private static double Distance(float[] a, float[] b)
		{
			double dx = a[0] - b[0];
			double dy = a[1] - b[1];
			double dz = a[2] - b[2];
			return Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}

		private void SendExpiryRewards(LiveConsumable item)
		{
			LanConsumableDef def = item.Def;
			if (def.TotalCoins <= 0)
			{
				return;
			}
			List<LanPlayer> recipients = server.InRoom(item.RoomKey).Where((LanPlayer p) => p.UserId == item.CreatorUserId || (p.LastPosition != null && Distance(p.LastPosition, item.Position) <= def.RewardRadius)).ToList();
			if (recipients.Count == 0)
			{
				return;
			}
			long coins = Math.Max(def.MinCoins, def.TotalCoins / recipients.Count);
			foreach (LanPlayer p in recipients)
			{
				server.SendRewardAdd(p, "reward.add", RewardSourceWorldObject, item.ItemId.ToString(), "{\"coins\":" + coins + "}");
			}
		}

		private void Tick(LiveConsumable item)
		{
			item.TimeToLive -= 1.0;
			if (item.TimeToLive > 0.0)
			{
				BroadcastItemVariable(item, ItemVar("ttl", SfsVar.Double, new SfsValue(SfsType.Double, item.TimeToLive)));
				server.Loop.After(1000, delegate
				{
					Tick(item);
				});
				return;
			}
			liveItems.Remove(item.ItemId);
			SendExpiryRewards(item);
			server.SendToRoom(item.RoomKey, LanServer.Sys(30, new SfsObject().PutInt("r", LanServer.RoomId).PutInts("n", new int[1]
			{
				item.ItemId
			})));
		}

		private void DeployWorld(LanPlayer player, string type, LanConsumableDef def, int partialCount, string propJson)
		{
			bool actioned = def.ActionCount > 0;
			LiveConsumable item = new LiveConsumable();
			item.ItemId = server.AllocateItemId();
			item.RoomKey = player.RoomKey;
			item.Type = type;
			item.CreatorUserId = player.UserId;
			item.Position = ParsePosition(propJson, player);
			item.Def = def;
			item.TimeToLive = def.TimeToLive;
			item.ActionCount = actioned ? new int?((partialCount > 0) ? partialCount : def.ActionCount) : null;
			liveItems[item.ItemId] = item;
			server.SendToRoom(item.RoomKey, LanServer.Ext("con.mmoitemdeployed", new SfsObject().PutInt("ownerId", player.UserId).PutInt("senderId", item.ItemId)));
			BroadcastDequip(player);
			server.SendToRoom(item.RoomKey, LanServer.Sys(30, new SfsObject().PutInt("r", LanServer.RoomId).PutArray("q", new SfsArray().AddArray(EncodeItem(item)))));
			server.Loop.After(1000, delegate
			{
				Tick(item);
			});
		}

		private void UseOneShot(LanPlayer player)
		{
			server.SendToRoomExcept(player.RoomKey, player, LanServer.Ext("con.use", new SfsObject().PutInt("senderId", player.UserId).PutString("type", player.PlayerSessionId ?? "0")));
			BroadcastDequip(player);
		}

		private void Use(LanPlayer player, string type, int partialCount, string propJson)
		{
			LanConsumableDef def = Get(type);
			if (def == null || def.ServerAdded)
			{
				return;
			}
			switch (def.Experience)
			{
			case "oneShot":
				UseOneShot(player);
				break;
			case "world":
				DeployWorld(player, type, def, partialCount, propJson);
				break;
			case "playerHeld":
				SetHeldItem(player, type, (partialCount > 0) ? partialCount : def.TotalItemQuantity.GetValueOrDefault(1));
				break;
			case "partyGameLobby":
				UseInvitation(player, type, def);
				break;
			}
		}

		public void UseAsBot(LanPlayer player, string type)
		{
			Use(player, type, 0, null);
		}

		public void HandleUse(LanPlayer player, SfsObject p)
		{
			string json = p.GetString("type");
			object signed;
			if (json == null || !LanJson.TryParse(json, out signed))
			{
				return;
			}
			string type = LanJson.Path(signed, "data", "type") as string;
			int partialCount;
			if (!LanJson.TryInt(LanJson.Path(signed, "data", "partialCount"), out partialCount) || partialCount < 0)
			{
				partialCount = 0;
			}
			if (type == null)
			{
				return;
			}
			Use(player, type, partialCount, p.GetString("prop"));
		}

		public void HandleReuse(LanPlayer player, SfsObject p)
		{
			string type = p.GetString("type") ?? "";
			string propJson = p.GetString("prop");
			LanConsumableDef def = Get(type);
			if (def == null || !def.QuestOnly)
			{
				player.Send(LanServer.Ext("con.reuse_error", new SfsObject().PutString("type", type).PutString("prop", propJson ?? "{}")));
				return;
			}
			Use(player, type, 0, propJson);
		}

		public void HandleInteraction(LanPlayer player, int action, string objectJson)
		{
			if (action != LocomotionInteract || objectJson == null)
			{
				return;
			}
			object obj;
			if (!LanJson.TryParse(objectJson, out obj))
			{
				return;
			}
			string id = LanJson.Get(obj, "id") as string;
			if (id == null)
			{
				return;
			}
			object type = LanJson.Get(obj, "type");
			long typeNum;
			bool numeric = LanJson.TryLong(type, out typeNum);
			if ((numeric && typeNum == 2) || (type as string) == "PLAYER")
			{
				HandlePlayerInteraction(player, id);
			}
			else if ((numeric && typeNum == 1) || (type as string) == "SERVER")
			{
				int itemId;
				if (int.TryParse(id, out itemId))
				{
					HandleWorldItemInteraction(itemId);
				}
			}
		}

		private void HandleWorldItemInteraction(int itemId)
		{
			LiveConsumable item;
			if (!liveItems.TryGetValue(itemId, out item) || !item.ActionCount.HasValue || item.ActionCount.Value <= 0)
			{
				return;
			}
			item.ActionCount = item.ActionCount.Value - 1;
			BroadcastItemVariable(item, ItemVar("actioncount", SfsVar.Int, new SfsValue(SfsType.Int, item.ActionCount.Value)));
		}

		private void SetHeldItem(LanPlayer player, string name, int actionCount)
		{
			player.EquippedConsumable = name;
			player.HeldObject = name;
			player.HeldActionCount = actionCount;
			server.BroadcastUserVars(player, LanServer.ConVar("consum", name), LanServer.ConPropVar(actionCount));
		}

		private void SetHeldActionCount(LanPlayer player, int actionCount)
		{
			player.HeldActionCount = actionCount;
			server.BroadcastUserVars(player, LanServer.ConPropVar(actionCount));
		}

		private void RemoveHeldItem(LanPlayer player)
		{
			player.EquippedConsumable = null;
			player.HeldObject = null;
			player.HeldActionCount = null;
			server.BroadcastUserVars(player, LanServer.NullVar("con"), LanServer.NullVar("con_prop"));
		}

		private void DropInvitation(int userId)
		{
			invitationGuests.Remove(userId);
			LanTimer t;
			if (autoStartTimers.TryGetValue(userId, out t))
			{
				t.Cancel();
				autoStartTimers.Remove(userId);
			}
		}

		private List<LanPlayer> LiveGuests(LanPlayer holder)
		{
			HashSet<LanPlayer> here = new HashSet<LanPlayer>(server.InRoom(holder.RoomKey));
			List<LanPlayer> guests;
			if (!invitationGuests.TryGetValue(holder.UserId, out guests))
			{
				guests = new List<LanPlayer>();
			}
			List<LanPlayer> kept = guests.Where((LanPlayer g) => here.Contains(g) && server.PartyGames.SessionOf(g) == null).ToList();
			invitationGuests[holder.UserId] = kept;
			LanConsumableDef def = Get(holder.EquippedConsumable);
			if (def != null && def.TotalItemQuantity.HasValue && holder.HeldActionCount.HasValue && holder.HeldActionCount.Value > 0 && holder.HeldActionCount.Value != def.TotalItemQuantity.Value - kept.Count)
			{
				SetHeldActionCount(holder, def.TotalItemQuantity.Value - kept.Count);
			}
			return kept.Where((LanPlayer g) => g.HeldObject == null && g.EquippedConsumable == null).ToList();
		}

		private void UseInvitation(LanPlayer player, string type, LanConsumableDef def)
		{
			int? template = server.PartyGames.Definitions.TemplateForTrigger(type);
			if (!template.HasValue || !server.PartyGames.CanStart(template.Value))
			{
				return;
			}
			if (server.PartyGames.SessionOf(player) != null)
			{
				return;
			}
			DropInvitation(player.UserId);
			SetHeldItem(player, type, def.TotalItemQuantity.GetValueOrDefault(1));
		}

		private bool JoinInvitation(LanPlayer player, LanPlayer holder)
		{
			string name = holder.EquippedConsumable;
			LanConsumableDef def = Get(name);
			if (def == null || def.Experience != "partyGameLobby")
			{
				return false;
			}
			int? template = server.PartyGames.Definitions.TemplateForTrigger(name);
			if (!template.HasValue || !server.PartyGames.CanStart(template.Value) || !holder.HeldActionCount.HasValue || holder.HeldActionCount.Value == 0)
			{
				return true;
			}
			if (player.EquippedConsumable != null || player.HeldObject != null || server.PartyGames.SessionOf(player) != null || server.PartyGames.SessionOf(holder) != null)
			{
				return true;
			}
			List<LanPlayer> guests = LiveGuests(holder);
			if (guests.Contains(player))
			{
				return true;
			}
			if (!server.InRoom(player.RoomKey).Contains(player))
			{
				return true;
			}
			guests.Add(player);
			int left = def.TotalItemQuantity.GetValueOrDefault(1) - guests.Count;
			if (left > 0)
			{
				invitationGuests[holder.UserId] = guests;
				SetHeldActionCount(holder, left);
				ArmAutoStart(holder, name, template.Value);
				return true;
			}
			List<LanPlayer> players = new List<LanPlayer>();
			players.Add(holder);
			players.AddRange(guests);
			if (!server.PartyGames.CanStartWith(template.Value, players))
			{
				guests.RemoveAt(guests.Count - 1);
				invitationGuests[holder.UserId] = guests;
				return true;
			}
			StartInvitation(holder, name, template.Value, players);
			return true;
		}

		private void StartInvitation(LanPlayer holder, string name, int template, List<LanPlayer> players)
		{
			DropInvitation(holder.UserId);
			SetHeldActionCount(holder, 0);
			RemoveHeldItem(holder);
			handoffCancelled[holder.UserId] = false;
			server.Loop.After(InvitationHandoffMs, delegate
			{
				bool cancelled;
				handoffCancelled.TryGetValue(holder.UserId, out cancelled);
				handoffCancelled.Remove(holder.UserId);
				if (cancelled)
				{
					return;
				}
				HashSet<LanPlayer> connected = new HashSet<LanPlayer>(server.All());
				List<LanPlayer> here = players.Where((LanPlayer p) => connected.Contains(p) && server.PartyGames.SessionOf(p) == null).ToList();
				if (here.Contains(holder) && server.PartyGames.Start(template, here, null) != null)
				{
					return;
				}
				if (!connected.Contains(holder) || holder.EquippedConsumable != null || holder.HeldObject != null || server.PartyGames.SessionOf(holder) != null)
				{
					return;
				}
				List<LanPlayer> guests = here.Where((LanPlayer p) => p != holder).ToList();
				invitationGuests[holder.UserId] = guests;
				LanConsumableDef def = Get(name);
				SetHeldItem(holder, name, ((def != null) ? def.TotalItemQuantity.GetValueOrDefault(1) : 1) - guests.Count);
				ArmAutoStart(holder, name, template);
			});
		}

		public void OnInvitationAction(LanPlayer holder, int action)
		{
			string name = holder.EquippedConsumable;
			LanConsumableDef def = Get(name);
			if (def == null || def.Experience != "partyGameLobby")
			{
				return;
			}
			int? template = server.PartyGames.Definitions.TemplateForTrigger(name);
			LanPartyGameDef gdef = template.HasValue ? server.PartyGames.Definitions.Get(template.Value) : null;
			if (gdef == null || gdef.LobbyType != 1 || !holder.HeldActionCount.HasValue || holder.HeldActionCount.Value == 0)
			{
				return;
			}
			if (action != LanFishBucket.ActionStart)
			{
				return;
			}
			if (server.InRoom(holder.RoomKey).Contains(holder))
			{
				TryOwnerStart(holder, name, template.Value);
			}
		}

		private void TryOwnerStart(LanPlayer holder, string name, int template)
		{
			LanPartyGameDef def = server.PartyGames.Definitions.Get(template);
			if (def == null || !server.PartyGames.CanStart(template) || server.PartyGames.SessionOf(holder) != null || !holder.HeldActionCount.HasValue || holder.HeldActionCount.Value == 0 || holder.EquippedConsumable != name)
			{
				return;
			}
			List<LanPlayer> players = new List<LanPlayer>();
			players.Add(holder);
			players.AddRange(LiveGuests(holder));
			if (players.Count < def.MinPlayers || !server.PartyGames.CanStartWith(template, players))
			{
				return;
			}
			StartInvitation(holder, name, template, players);
		}

		private void ArmAutoStart(LanPlayer holder, string name, int template)
		{
			LanPartyGameDef def = server.PartyGames.Definitions.Get(template);
			LanTimer old;
			if (autoStartTimers.TryGetValue(holder.UserId, out old))
			{
				old.Cancel();
				autoStartTimers.Remove(holder.UserId);
			}
			List<LanPlayer> guests;
			int count = invitationGuests.TryGetValue(holder.UserId, out guests) ? guests.Count : 0;
			if (def == null || def.LobbyType != 1 || 1 + count < def.MinPlayers)
			{
				return;
			}
			autoStartTimers[holder.UserId] = server.Loop.After(AutoStartSeconds * 1000.0, delegate
			{
				autoStartTimers.Remove(holder.UserId);
				TryOwnerStart(holder, name, template);
			});
		}

		public void ForgetInvitation(LanPlayer player)
		{
			DropInvitation(player.UserId);
			if (handoffCancelled.ContainsKey(player.UserId))
			{
				handoffCancelled[player.UserId] = true;
			}
		}

		private void HandlePlayerInteraction(LanPlayer player, string targetSessionId)
		{
			if (targetSessionId == player.PlayerSessionId)
			{
				string name = player.EquippedConsumable;
				LanConsumableDef held = Get(name);
				bool edible = held != null && (held.ServerAdded || (held.Experience == "playerHeld" && !held.Shareable));
				if (name == null || !edible || !player.HeldActionCount.HasValue || player.HeldActionCount.Value == 0)
				{
					return;
				}
				SetHeldActionCount(player, 0);
				server.Loop.After(EatenItemRemoveDelayMs, delegate
				{
					if (player.EquippedConsumable == name && player.HeldActionCount == 0)
					{
						RemoveHeldItem(player);
					}
				});
				return;
			}
			LanPlayer holder = server.InRoomExcept(player.RoomKey, player).FirstOrDefault((LanPlayer e) => e.PlayerSessionId == targetSessionId);
			if (holder != null && JoinInvitation(player, holder))
			{
				return;
			}
			if (holder == null)
			{
				return;
			}
			string trayName = holder.EquippedConsumable;
			LanConsumableDef tray = Get(trayName);
			if (tray == null || tray.Experience != "playerHeld" || !tray.Shareable || !holder.HeldActionCount.HasValue || holder.HeldActionCount.Value == 0)
			{
				return;
			}
			LanConsumableDef recipient = Get(tray.Recipient);
			if (player.EquippedConsumable != null || recipient == null || !recipient.ServerAdded)
			{
				return;
			}
			long now = server.Loop.NowMs;
			if (now - player.LastTakeAt < TakeCooldownMs)
			{
				return;
			}
			player.LastTakeAt = now;
			int left = holder.HeldActionCount.Value - 1;
			if (left > 0)
			{
				SetHeldActionCount(holder, left);
			}
			else
			{
				SetHeldActionCount(holder, 0);
				RemoveHeldItem(holder);
			}
			SetHeldItem(player, tray.Recipient, 1);
		}

		private bool ReleaseHeldItem(LanPlayer player, string previous)
		{
			int? count = player.HeldActionCount;
			if (!count.HasValue)
			{
				return false;
			}
			DropInvitation(player.UserId);
			player.HeldActionCount = null;
			LanConsumableDef def = Get(previous);
			if (def != null && def.Experience == "playerHeld" && !def.ServerAdded && count.Value > 0 && player.Swid != null)
			{
				Dictionary<string, object> payload = new Dictionary<string, object>
				{
					{
						"goodUntil",
						LanServer.EpochMs + 300000
					},
					{
						"signature",
						""
					},
					{
						"swid",
						player.Swid
					},
					{
						"data",
						new Dictionary<string, object>
						{
							{
								"type",
								previous
							},
							{
								"partialCount",
								(long)count.Value
							}
						}
					}
				};
				player.Send(LanServer.Ext("con.partial", new SfsObject().PutString("data", LanJson.Serialize(payload))));
			}
			return true;
		}

		private void ReplaceHeldItem(LanPlayer player, LanEquippedObject next)
		{
			if (next == null)
			{
				server.BroadcastUserVars(player, LanServer.NullVar("con"), LanServer.NullVar("con_prop"));
				return;
			}
			player.Send(LanServer.UserVarsPacket(player, LanServer.ConVar(next.Type, next.Name), LanServer.NullVar("con_prop")));
			byte[] clear = LanServer.UserVarsPacket(player, LanServer.NullVar("con"), LanServer.NullVar("con_prop"));
			byte[] set = LanServer.UserVarsPacket(player, LanServer.ConVar(next.Type, next.Name));
			foreach (LanPlayer other in server.InRoomExcept(player.RoomKey, player))
			{
				other.Send(clear);
				other.Send(set);
			}
		}

		public void HandleClientConChanged(LanPlayer player, LanEquippedObject next)
		{
			LanPartyGameSession game = server.PartyGames.SessionOf(player);
			LanPartyGamePlayer me = (game != null) ? game.Players.FirstOrDefault((LanPartyGamePlayer p) => p.Player == player && !p.Left) : null;
			string gameProp = (game != null && me != null) ? game.GamePropFor(me) : null;
			if (gameProp != null && player.HeldObject == gameProp && (next == null || next.Name != gameProp))
			{
				player.HeldObject = null;
				server.PartyGames.OnPlayerQuit(player);
			}
			server.Lobbies.OnLobbyPropPutAway(player, (next != null) ? next.Name : null);
			if (handoffCancelled.ContainsKey(player.UserId))
			{
				handoffCancelled[player.UserId] = true;
			}
			string previous = player.EquippedConsumable;
			string name = (next != null && LanServer.FitsBytes(next.Name, 255)) ? next.Name : null;
			string nextConsumable = (next != null && next.Type == "consum") ? name : null;
			player.EquippedConsumable = nextConsumable;
			player.HeldObject = name;
			if (nextConsumable != null && nextConsumable == previous)
			{
				return;
			}
			if (ReleaseHeldItem(player, previous))
			{
				ReplaceHeldItem(player, next);
			}
		}

		public void EquipObject(LanPlayer player, LanEquippedObject next)
		{
			string previous = player.EquippedConsumable;
			player.EquippedConsumable = null;
			player.HeldObject = next.Name;
			if (ReleaseHeldItem(player, previous))
			{
				ReplaceHeldItem(player, next);
			}
			else
			{
				server.BroadcastUserVars(player, LanServer.ConVar(next.Type, next.Name));
			}
		}
	}
}
#endif
