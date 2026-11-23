#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ClubPenguin.Net.Lan
{
	public sealed partial class LanServer
	{
		private const int MaxChatLength = 200;

		private const int MaxObjectives = 64;

		private const int MaxObjectiveBytes = 64;

		private const int MaxLocomotionObjectBytes = 1024;

		private static readonly Dictionary<int, KeyValuePair<string, string>> PropCatalog = new Dictionary<int, KeyValuePair<string, string>>
		{
			{ 11, P("GearDrums", "durable") },
			{ 12, P("GearFirstAidKit", "durable") },
			{ 13, P("GearFishDogMultiTool", "durable") },
			{ 14, P("GearGuitar", "durable") },
			{ 15, P("GearJackhammer", "durable") },
			{ 16, P("GearKeytar", "durable") },
			{ 17, P("GearPirateKit", "durable") },
			{ 18, P("GearScepter", "durable") },
			{ 19, P("GearShield", "durable") },
			{ 20, P("GearSword", "durable") },
			{ 66, P("GearTrafficSign", "durable") },
			{ 71, P("GearPomPomsBlue", "durable") },
			{ 72, P("GearPomPomsRed", "durable") },
			{ 73, P("GearGaryGear", "durable") },
			{ 77, P("GearCPSNCamera", "durable") },
			{ 86, P("GearDance", "durable") },
			{ 87, P("GearScroogeMcDuckCane", "durable") },
			{ 101, P("AcousticGuitarGear", "durable") },
			{ 103, P("MicrophoneGear", "durable") },
			{ 105, P("BongoDrumsGear", "durable") },
			{ 112, P("CellPhone", "durable") },
			{ 113, P("Map", "durable") },
			{ 114, P("DressingBooth", "durable") },
			{ 115, P("IglooPlans", "durable") },
			{ 129, P("GearFlowerWand", "durable") },
			{ 154, P("MedievalGear", "durable") },
			{ 4, P("Dance", "dispns") },
			{ 5, P("Drums", "dispns") },
			{ 7, P("FirstAidKit", "dispns") },
			{ 8, P("FishDogMultiTool", "dispns") },
			{ 9, P("FishingRod", "dispns") },
			{ 10, P("FishingRodGame", "dispns") },
			{ 21, P("Guitar", "dispns") },
			{ 26, P("Keytar", "dispns") },
			{ 31, P("PirateKit", "dispns") },
			{ 36, P("ScepterProp", "dispns") },
			{ 37, P("Shield", "dispns") },
			{ 43, P("Sword", "dispns") },
			{ 57, P("JackHammer", "dispns") },
			{ 70, P("CPSNCamera", "dispns") },
			{ 74, P("PomPomsRed", "dispns") },
			{ 75, P("PomPomsBlue", "dispns") },
			{ 76, P("GaryGear", "dispns") },
			{ 78, P("YetiRescueKit", "dispns") },
			{ 100, P("AcousticGuitar", "dispns") },
			{ 102, P("Microphone", "dispns") },
			{ 104, P("BongoDrums", "dispns") },
			{ 124, P("PizzaDispensible", "dispns") },
			{ 125, P("HotChocolateDispensible", "dispns") },
			{ 126, P("SquidStickDispensible", "dispns") },
			{ 127, P("FluffCakeDispensible", "dispns") },
			{ 128, P("CandyCaneDispensible", "dispns") },
			{ 146, P("BirthdayCakeSingleDispensible", "dispns") },
			{ 147, P("SilverAcousticGuitar", "dispns") }
		};

		private static KeyValuePair<string, string> P(string name, string kind)
		{
			return new KeyValuePair<string, string>(name, kind);
		}

		private void HandleExtension(LanPlayer player, string cmd, SfsObject p)
		{
			switch (cmd)
			{
			case "quest.set":
				HandleQuestSet(player, p);
				break;
			case "quest.c_obj":
				HandleQuestCompleteObjective(player, p);
				break;
			case "chat.msg":
				HandleChatMsg(player, p);
				break;
			case "chat.activity":
			case "chat.activity_cancel":
				SendToRoom(player.RoomKey, Ext(cmd, new SfsObject().PutInt("senderId", player.UserId)));
				break;
			case "l.a":
				HandleLocomotion(player, p);
				break;
			case "zone.logout":
				Transient.Send(player);
				player.Send(Sys(2, new SfsObject().PutString("zn", player.ZoneName)));
				break;
			case "dispensable.equip":
				HandleDispensableEquip(player, p);
				break;
			case "durable.equip":
				HandleDurableEquip(player, p);
				break;
			case "time.get":
				HandleTimeGet(player, p);
				break;
			case "encryption.get":
				HandleEncryptionGet(player, p);
				break;
			case "validate.state":
				HandleValidateState(player);
				break;
			case "zone.player_location":
				HandlePlayerLocation(player, p);
				break;
			case "task.p":
				HandlePickup(player, p);
				break;
			case "con.set":
				break;
			case "con.use":
				Consumables.HandleUse(player, p);
				break;
			case "con.reuse":
				Consumables.HandleReuse(player, p);
				break;
			case "pgame.message":
				PartyGames.HandleMessage(player, p);
				break;
			case "room.igloo_updated":
				HandleIglooUpdated(player, p);
				break;
			case "reward.levelup":
				HandleLevelUp(player, p);
				break;
			}
		}

		private static bool IsObjectiveName(string o)
		{
			return !string.IsNullOrEmpty(o) && FitsBytes(o, MaxObjectiveBytes);
		}

		private void HandleQuestSet(LanPlayer player, SfsObject p)
		{
			string json = p.GetString("data");
			object parsed;
			if (json != null && LanJson.TryParse(json, out parsed))
			{
				List<object> quests = LanJson.Get(parsed, "data") as List<object>;
				if (quests != null)
				{
					object active = quests.FirstOrDefault(delegate(object q)
					{
						object status = LanJson.Get(q, "status");
						long s;
						return (LanJson.TryLong(status, out s) && s == 1) || (status as string) == "ACTIVE";
					});
					List<object> objectives = LanJson.Get(active, "completedObjectives") as List<object>;
					player.CompletedObjectives.Clear();
					if (objectives != null)
					{
						foreach (object o in objectives)
						{
							string name = o as string;
							if (IsObjectiveName(name) && !player.CompletedObjectives.Contains(name) && player.CompletedObjectives.Count < MaxObjectives)
							{
								player.CompletedObjectives.Add(name);
							}
						}
					}
				}
			}
			player.Send(Ext("quest.set", new SfsObject()));
		}

		private void HandleQuestCompleteObjective(LanPlayer player, SfsObject p)
		{
			string sent = p.GetString("obj");
			if (IsObjectiveName(sent) && !player.CompletedObjectives.Contains(sent) && player.CompletedObjectives.Count < MaxObjectives)
			{
				player.CompletedObjectives.Add(sent);
			}
			string payload = "{\"goodUntil\":0,\"signature\":\"\",\"swid\":\"\",\"data\":" + LanJson.Serialize(player.CompletedObjectives) + "}";
			player.Send(Ext("quest.s_obj", new SfsObject().PutString("data", payload)));
		}

		private static string Decrypt(string raw, byte[] key)
		{
			try
			{
				byte[] combined = Convert.FromBase64String(raw);
				if (combined.Length < 17)
				{
					return null;
				}
				byte[] iv = new byte[16];
				Buffer.BlockCopy(combined, 0, iv, 0, 16);
				using (Aes aes = Aes.Create())
				{
					aes.Key = key;
					aes.IV = iv;
					aes.Mode = CipherMode.CBC;
					aes.Padding = PaddingMode.PKCS7;
					using (ICryptoTransform dec = aes.CreateDecryptor())
					{
						byte[] plain = dec.TransformFinalBlock(combined, 16, combined.Length - 16);
						return Encoding.UTF8.GetString(plain);
					}
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static string GetPossiblyEncrypted(SfsObject p, string key, LanPlayer player)
		{
			string ep = p.GetString("ep");
			string raw = p.GetString(key);
			if (ep != key || player.AesKey == null || raw == null)
			{
				return raw;
			}
			return Decrypt(raw, player.AesKey);
		}

		private void HandleChatMsg(LanPlayer player, SfsObject p)
		{
			string json = GetPossiblyEncrypted(p, "msg", player);
			object parsed;
			if (json == null || !LanJson.TryParse(json, out parsed))
			{
				return;
			}
			string message = LanJson.Path(parsed, "data", "message") as string;
			if (message == null)
			{
				return;
			}
			if (new StringInfo(message).LengthInTextElements > MaxChatLength)
			{
				return;
			}
			int emotion;
			if (!LanJson.TryInt(LanJson.Path(parsed, "data", "emotion"), out emotion))
			{
				emotion = 0;
			}
			message = message.TrimStart('​', '‌', '‍', '⁠', '﻿');
			if (HandleCommand(player, message))
			{
				return;
			}
			Bots.OnChat(player, message);
			string received = "{\"senderSessionId\":" + (player.PlayerSessionId ?? "0") + ",\"message\":" + LanJson.Quote(message) + ",\"emotion\":" + emotion.ToString(CultureInfo.InvariantCulture) + "}";
			SendToRoom(player.RoomKey, Ext("chat.msg", new SfsObject().PutString("msg", received).PutInt("senderId", player.UserId)));
		}

		private static bool IsVector3(float[] v)
		{
			return v != null && v.Length == 3 && v.All((float f) => !float.IsNaN(f) && !float.IsInfinity(f));
		}

		private void HandleLocomotion(LanPlayer player, SfsObject p)
		{
			float[] position = p.GetFloats("p");
			byte action;
			if (position == null || !p.TryGetByte("a", out action))
			{
				return;
			}
			float[] direction = p.GetFloats("d");
			float[] velocity = p.GetFloats("v");
			if (!IsVector3(position) || (direction != null && !IsVector3(direction)) || (velocity != null && !IsVector3(velocity)))
			{
				return;
			}
			string obj = p.GetString("o");
			if (obj != null && !FitsBytes(obj, MaxLocomotionObjectBytes))
			{
				obj = null;
			}
			float[] previous = player.LastPosition;
			player.LastPosition = position;
			Bots.Observe(player, action, position, obj);
			MarkPlaced(player);
			BroadcastNewJoiner(player);
			Targets.OnThrow(player, action, previous, position, velocity);
			PartyGames.OnAction(player, action, position);
			Consumables.OnInvitationAction(player, action);
			PartyGames.OnPlayerMoved(player, position);
			SfsObject r = new SfsObject().PutFloats("p", position).PutByte("a", action).PutInt("senderId", player.UserId)
				.PutLong("t", EpochMs);
			if (direction != null)
			{
				r.PutFloats("d", direction);
			}
			if (velocity != null)
			{
				r.PutFloats("v", velocity);
			}
			if (obj != null)
			{
				r.PutString("o", obj);
			}
			SendToRoom(player.RoomKey, Ext("l.a", r));
			Consumables.HandleInteraction(player, action, obj);
			HandleLocalInteraction(player, action, obj);
			Lobbies.OnThrowAction(player, action);
			Lobbies.HandleInteraction(player, action, obj);
		}

		private void SendEquippedObject(LanPlayer player, string kind, string nameOnServer)
		{
			player.EquippedItemName = nameOnServer;
			Consumables.EquipObject(player, new LanEquippedObject(kind, nameOnServer));
		}

		private void HandleDispensableEquip(LanPlayer player, SfsObject p)
		{
			long propId;
			KeyValuePair<string, string> prop;
			if (p.TryGetInteger("type", out propId) && propId >= int.MinValue && propId <= int.MaxValue && PropCatalog.TryGetValue((int)propId, out prop))
			{
				SendEquippedObject(player, prop.Value, prop.Key);
				Bots.ObserveDispenser(player, prop.Value, prop.Key);
			}
		}

		private void HandleDurableEquip(LanPlayer player, SfsObject p)
		{
			string json = p.GetString("type");
			object parsed;
			int propId;
			KeyValuePair<string, string> prop;
			if (json != null && LanJson.TryParse(json, out parsed) && LanJson.TryInt(LanJson.Path(parsed, "data", "propId"), out propId) && PropCatalog.TryGetValue(propId, out prop))
			{
				SendEquippedObject(player, prop.Value, prop.Key);
			}
		}

		private void HandleTimeGet(LanPlayer player, SfsObject p)
		{
			SfsValue ct = p.Get("ct");
			long clientTime = (ct != null && ct.Type == SfsType.Long) ? ((long)ct.Value) : 0;
			player.Send(Ext("time.get", new SfsObject().PutLong("ct", clientTime).PutLong("st", EpochMs)));
		}

		private void HandleEncryptionGet(LanPlayer player, SfsObject p)
		{
			string pkm = p.GetString("pkm");
			string pke = p.GetString("pke");
			if (pkm == null || pke == null)
			{
				return;
			}
			try
			{
				byte[] modulus = Convert.FromBase64String(pkm);
				if (modulus.Length > 0 && modulus[0] == 0)
				{
					byte[] trimmed = new byte[modulus.Length - 1];
					Buffer.BlockCopy(modulus, 1, trimmed, 0, trimmed.Length);
					modulus = trimmed;
				}
				byte[] exponent = Convert.FromBase64String(pke);
				if (modulus.Length > 512 || exponent.Length > 4 || modulus.Length == 0 || exponent.Length == 0)
				{
					throw new CryptographicException("unsupported key");
				}
				byte[] aesKey = new byte[32];
				using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
				{
					rng.GetBytes(aesKey);
				}
				byte[] encrypted;
				using (RSA rsa = RSA.Create())
				{
					RSAParameters parameters = default(RSAParameters);
					parameters.Modulus = modulus;
					parameters.Exponent = exponent;
					rsa.ImportParameters(parameters);
					encrypted = rsa.Encrypt(aesKey, RSAEncryptionPadding.Pkcs1);
				}
				player.AesKey = aesKey;
				player.Send(Ext("encryption.get", new SfsObject().PutString("ek", Convert.ToBase64String(encrypted))));
			}
			catch (Exception)
			{
				player.Send(Ext("encryption.error", new SfsObject()));
			}
		}

		private void HandleValidateState(LanPlayer player)
		{
			float[] pos = player.LastPosition ?? new float[3];
			Dictionary<string, object> state = new Dictionary<string, object>
			{
				{
					"location",
					new Dictionary<string, object>
					{
						{
							"world",
							""
						},
						{
							"language",
							0L
						},
						{
							"zoneId",
							new Dictionary<string, object>
							{
								{
									"name",
									player.RoomName
								},
								{
									"instanceId",
									""
								}
							}
						},
						{
							"contentIdentifier",
							""
						}
					}
				},
				{
					"position",
					new Dictionary<string, object>
					{
						{
							"x",
							(double)pos[0]
						},
						{
							"y",
							(double)pos[1]
						},
						{
							"z",
							(double)pos[2]
						}
					}
				},
				{
					"equippedItem",
					player.EquippedItemName ?? ""
				}
			};
			long now = EpochMs;
			Dictionary<string, object> signed = new Dictionary<string, object>
			{
				{
					"goodUntil",
					now + 300000
				},
				{
					"swid",
					player.Swid ?? "{SWID}"
				},
				{
					"data",
					state
				},
				{
					"signature",
					"server_signature_" + now.ToString(CultureInfo.InvariantCulture)
				}
			};
			player.Send(Ext("validate.state", new SfsObject().PutString("data", LanJson.Serialize(signed))));
		}

		private void HandlePlayerLocation(LanPlayer player, SfsObject p)
		{
			string swid = p.GetString("swid");
			LanPlayer target = (swid != null) ? BySwid(swid) : null;
			if (target == null || target.LastPosition == null)
			{
				player.Send(Ext("zone.player_notfound", new SfsObject()));
				return;
			}
			player.Send(Ext("zone.player_location", new SfsObject().PutFloats("pos", target.LastPosition)));
		}

		private void HandlePickup(LanPlayer player, SfsObject p)
		{
			string id = p.GetString("id");
			string room = ZoneOfRoom(player.RoomKey);
			if (string.IsNullOrEmpty(id) || id.Length > 512 || string.IsNullOrEmpty(room))
			{
				return;
			}
			RecordCollected(player, room, id);
		}

		private void HandleLocalInteraction(LanPlayer player, byte action, string obj)
		{
			if (action != 2 || obj == null)
			{
				return;
			}
			object parsed;
			int type;
			if (!LanJson.TryParse(obj, out parsed) || !LanJson.TryInt(LanJson.Get(parsed, "type"), out type) || type != 0)
			{
				return;
			}
			string id = LanJson.Get(parsed, "id") as string;
			string tag = LanJson.Get(parsed, "tag") as string;
			string room = ZoneOfRoom(player.RoomKey);
			if (string.IsNullOrEmpty(id) || id.Length > 512 || string.IsNullOrEmpty(tag) || tag == "Untagged" || string.IsNullOrEmpty(room))
			{
				return;
			}
			RecordCollected(player, room, id);
		}

		private void RecordCollected(LanPlayer player, string room, string id)
		{
			long now = EpochMs;
			if (!player.RoomCollected.ContainsKey(id) && player.RoomCollected.Count >= MaxCollectedPerRoom)
			{
				return;
			}
			player.RoomCollected[id] = now;
			Dictionary<string, long> saved = CollectedStore(player, room);
			if (saved != null && (saved.ContainsKey(id) || saved.Count < MaxCollectedPerRoom))
			{
				saved[id] = now;
			}
			SendRoomCollected(player, room);
		}

		private const int MaxCollectedPerRoom = 2000;

		private const int MaxCollectedPlayers = 500;

		private readonly Dictionary<string, Dictionary<string, Dictionary<string, long>>> collectedByPlayer = new Dictionary<string, Dictionary<string, Dictionary<string, long>>>();

		private Dictionary<string, long> CollectedStore(LanPlayer player, string room)
		{
			if (player.Swid == null || string.IsNullOrEmpty(room))
			{
				return null;
			}
			Dictionary<string, Dictionary<string, long>> rooms;
			if (!collectedByPlayer.TryGetValue(player.Swid, out rooms))
			{
				if (collectedByPlayer.Count >= MaxCollectedPlayers)
				{
					return null;
				}
				rooms = new Dictionary<string, Dictionary<string, long>>();
				collectedByPlayer[player.Swid] = rooms;
			}
			Dictionary<string, long> saved;
			if (!rooms.TryGetValue(room, out saved))
			{
				saved = new Dictionary<string, long>();
				rooms[room] = saved;
			}
			return saved;
		}

		private bool RestoreCollected(LanPlayer player)
		{
			Dictionary<string, long> saved = CollectedStore(player, ZoneOfRoom(player.RoomKey));
			if (saved == null)
			{
				return false;
			}
			foreach (KeyValuePair<string, long> kv in player.RoomCollected)
			{
				long at;
				if (saved.TryGetValue(kv.Key, out at) ? at < kv.Value : saved.Count < MaxCollectedPerRoom)
				{
					saved[kv.Key] = kv.Value;
				}
			}
			bool added = false;
			foreach (KeyValuePair<string, long> kv in saved)
			{
				long at;
				if (!player.RoomCollected.TryGetValue(kv.Key, out at) || at < kv.Value)
				{
					player.RoomCollected[kv.Key] = kv.Value;
					added = true;
				}
			}
			return added;
		}

		private void SendRoomCollected(LanPlayer player, string room)
		{
			Dictionary<string, object> collected = new Dictionary<string, object>();
			foreach (KeyValuePair<string, long> kv in player.RoomCollected)
			{
				collected[kv.Key] = kv.Value;
			}
			Dictionary<string, object> reward = new Dictionary<string, object>
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
					""
				},
				{
					"data",
					new Dictionary<string, object>
					{
						{
							"room",
							room
						},
						{
							"collected",
							collected
						}
					}
				}
			};
			player.Send(Ext("reward.room", new SfsObject().PutString("reward", LanJson.Serialize(reward))));
		}

		private void HandleIglooUpdated(LanPlayer player, SfsObject p)
		{
			if (p.GetString("igloo_data") == null || player.Swid == null)
			{
				return;
			}
			string[] parts = player.RoomName.Split(':');
			if (parts.Length < 4 || parts[0] != "Igloo" || parts[3] != player.Swid)
			{
				return;
			}
			SendToRoomExcept(player.RoomKey, player, Ext("room.igloo_updated", new SfsObject().PutString("igloo_id", player.Swid)));
		}

		private void HandleLevelUp(LanPlayer player, SfsObject p)
		{
			long level;
			if (!p.TryGetInteger("level", out level) || level < 1 || level > int.MaxValue)
			{
				return;
			}
			SendToRoomExcept(player.RoomKey, player, Ext("reward.levelup", new SfsObject().PutInt("level", (int)level).PutInt("senderId", player.UserId)));
		}

		public void SendRewardAdd(LanPlayer player, string command, int source, string sourceId, string rewardJson)
		{
			if (player.PlayerSessionId == null)
			{
				return;
			}
			string payload = "{\"goodUntil\":0,\"signature\":\"\",\"swid\":\"\",\"data\":{\"source\":" + source.ToString(CultureInfo.InvariantCulture) + ",\"sourceId\":" + LanJson.Quote(sourceId) + ",\"rewards\":{\"" + player.PlayerSessionId + "\":" + rewardJson + "}}}";
			player.Send(Ext(command, new SfsObject().PutString("reward", payload)));
		}
	}
}
#endif
