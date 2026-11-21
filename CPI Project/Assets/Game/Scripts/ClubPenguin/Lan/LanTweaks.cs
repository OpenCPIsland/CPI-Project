#if !UNITY_WEBGL
using ClubPenguin.Chat;
using ClubPenguin.Net.Client;
using ClubPenguin.Tubes;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Net.Lan
{
	public static class LanTweaks
	{
		private const double HostTimeoutSeconds = 6.0;

		private const float ServerStopDelaySeconds = 3f;

		private const string HostPref = "LAN.Host";

		private const string JoinPref = "LAN.Join";

		private const string RememberPref = "LAN.Remember";

		private const string BotsPref = "LAN.Bots";

		private const string BotChatPref = "LAN.BotChat";

		private const string BotEmotesPref = "LAN.BotEmotes";

		private const int MaxBots = 30;

		private static readonly object owner = new object();

		private static LanServer server;

		private static LanBeacon beacon;

		private static LanFinder finder;

		private static string joinedAddress;

		private static string joinedName;

		private static bool joinEnabled;

		private static bool watching;

		private static bool quitHooked;

		private static AndroidJavaObject multicastLock;

		[Tweakable("LAN.Host", Description = "Hosts a LAN game on this device. Devices on the same network with LAN.Join on connect by themselves.")]
		[PublicTweak]
		public static bool Host
		{
			get
			{
				return server != null;
			}
			set
			{
				if (value)
				{
					StartHosting();
				}
				else
				{
					StopHosting();
				}
				SavePrefs();
			}
		}

		[Tweakable("LAN.Join", Description = "Looks for a LAN game on this network and connects to it automatically. Turn off to go back to Offline Mode.")]
		[PublicTweak]
		public static bool Join
		{
			get
			{
				return joinEnabled;
			}
			set
			{
				if (value)
				{
					StartJoining();
				}
				else
				{
					StopJoining();
				}
				SavePrefs();
			}
		}

		[Tweakable("LAN.Remember", Description = "Saves LAN.Host and LAN.Join so they turn back on the next time the game starts. Turn off to start in Offline Mode every time.")]
		[PublicTweak]
		public static bool Remember
		{
			get
			{
				return PlayerPrefs.GetInt(RememberPref, 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt(RememberPref, value ? 1 : 0);
				if (value)
				{
					SavePrefs();
				}
				else
				{
					PlayerPrefs.DeleteKey(HostPref);
					PlayerPrefs.DeleteKey(JoinPref);
					PlayerPrefs.Save();
				}
			}
		}

		[Tweakable("LAN.Experimental.Bots", Description = "EXPERIMENTAL (more bots there are, the buggier they are): Number of bot penguins the LAN host adds (0-30). Bots walk around, follow players between rooms and play Crate Co, Tilt-o-Tubes, dance battles and party games. Only the device with LAN.Host on runs them. It's not recommended to use this experimental feature.")]
		[PublicTweak]
		public static int Bots
		{
			get
			{
				return PlayerPrefs.GetInt(BotsPref, 0);
			}
			set
			{
				PlayerPrefs.SetInt(BotsPref, Mathf.Clamp(value, 0, MaxBots));
				PlayerPrefs.Save();
				ApplyBots();
			}
		}

		[Tweakable("LAN.Experimental.BotChat", Description = "Lets LAN bots use the chat service.")]
		[PublicTweak]
		public static bool BotChat
		{
			get
			{
				return PlayerPrefs.GetInt(BotChatPref, 1) != 0;
			}
			set
			{
				PlayerPrefs.SetInt(BotChatPref, value ? 1 : 0);
				PlayerPrefs.Save();
				ApplyBotTalk();
			}
		}

		[Tweakable("LAN.Experimental.BotEmotes", Description = "Lets LAN bots send emotes and sizzle clips.")]
		[PublicTweak]
		public static bool BotEmotes
		{
			get
			{
				return PlayerPrefs.GetInt(BotEmotesPref, 1) != 0;
			}
			set
			{
				PlayerPrefs.SetInt(BotEmotesPref, value ? 1 : 0);
				PlayerPrefs.Save();
				ApplyBotTalk();
			}
		}

		[Tweakable("LAN.Status", Description = "Current LAN state.")]
		[PublicTweak]
		public static string Status
		{
			get
			{
				if (LanGameServerClient.Instance == null)
				{
					return "LAN requires Offline Mode";
				}
				if (server != null)
				{
					return "Hosting on " + LanBeacon.LocalAddress() + " (" + (server.PlayerCount - server.Bots.Count) + " connected, " + server.Bots.Count + " bots)";
				}
				if (joinedAddress != null)
				{
					return "Connected to " + (string.IsNullOrEmpty(joinedName) ? joinedAddress : joinedName);
				}
				if (joinEnabled)
				{
					return "Searching for a LAN game";
				}
				return "Offline";
			}
			set
			{
			}
		}

		[Invokable("LAN.Help", Description = "Lists the chat commands that start with ! that are in the game.")]
		[PublicTweak]
		public static void Help()
		{
			DPrompt prompt = new DPrompt("LAN Commands", string.Join("\n", LanServer.CommandHelp), DPrompt.ButtonFlags.OK, null, true, true, true);
			PromptManager promptManager = Service.Get<PromptManager>();
			promptManager.ShowPrompt(prompt, delegate
			{
			}, promptManager.ErrorPrefab);
		}

		private static void SavePrefs()
		{
			if (!Remember)
			{
				return;
			}
			PlayerPrefs.SetInt(HostPref, (server != null) ? 1 : 0);
			PlayerPrefs.SetInt(JoinPref, joinEnabled ? 1 : 0);
			PlayerPrefs.Save();
		}

		public static bool RestorePrefs()
		{
			if (LanGameServerClient.Instance == null)
			{
				return false;
			}
			if (!Remember)
			{
				return true;
			}
			if (PlayerPrefs.GetInt(HostPref, 0) == 1)
			{
				StartHosting();
			}
			else if (PlayerPrefs.GetInt(JoinPref, 0) == 1)
			{
				StartJoining();
			}
			return true;
		}

		private static void StartHosting()
		{
			LanGameServerClient client = LanGameServerClient.Instance;
			if (client == null || server != null)
			{
				return;
			}
			HookQuit();
			LanServicesClient.Ensure();
			StopJoining();
			LanServerData data = new LanServerData();
			data.PartyGames = LoadText("Lan/party-games");
			data.TubeRaces = LoadText("Lan/tube-races");
			data.WorldObjects = LoadText("Lan/world-objects");
			data.CatalogPath = System.IO.Path.Combine(Application.persistentDataPath, "LanCatalog.json");
			data.BotsPath = System.IO.Path.Combine(Application.persistentDataPath, "LanBots.json");
			LanServer created = new LanServer(data);
			if (!created.Start())
			{
				return;
			}
			server = created;
			beacon = new LanBeacon(SystemInfo.deviceName);
			beacon.Start();
			client.SetHost(IPAddress.Loopback.ToString());
			ReloadZone();
			ApplyBots();
		}

		private static void ApplyBots()
		{
			if (server == null)
			{
				return;
			}
			ApplyBotTalk();
			LanBotGridBaker.Attach(server);
			server.Bots.SetTarget(Mathf.Clamp(PlayerPrefs.GetInt(BotsPref, 0), 0, MaxBots), BotColours(), BotTubes(), BotOutfits(), BotEmoteCodes(), BotSizzleClips());
		}

		private static void ApplyBotTalk()
		{
			if (server == null)
			{
				return;
			}
			server.Bots.ChatEnabled = PlayerPrefs.GetInt(BotChatPref, 1) != 0;
			server.Bots.EmotesEnabled = PlayerPrefs.GetInt(BotEmotesPref, 1) != 0;
		}

		private static int[] BotColours()
		{
			try
			{
				return Service.Get<GameData>().Get<Dictionary<int, AvatarColorDefinition>>().Keys.ToArray();
			}
			catch (System.Exception)
			{
				return new int[0];
			}
		}

		private static string CategoryOf(TemplateDefinition template)
		{
			return (template.CategoryKey != null && template.CategoryKey.Key != null) ? template.CategoryKey.Key.ToLowerInvariant() : "";
		}

		private static string[] BotOutfits()
		{
			try
			{
				List<TemplateDefinition> all = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>().Values.Where((TemplateDefinition t) => t != null && !t.WorkInProgress && CategoryOf(t).Length > 0).ToList();
				List<TemplateDefinition> designed = all.Where((TemplateDefinition t) => !t.IsEditable).ToList();
				List<TemplateDefinition> pool = (designed.Count >= 8) ? designed : all;
				Dictionary<string, List<TemplateDefinition>> byCategory = pool.GroupBy(CategoryOf).ToDictionary((IGrouping<string, TemplateDefinition> g) => g.Key, (IGrouping<string, TemplateDefinition> g) => g.ToList());
				List<string> categories = byCategory.Keys.ToList();
				System.Random random = new System.Random();
				List<string> outfits = new List<string>();
				for (int i = 0; i < 80 && categories.Count > 0; i++)
				{
					List<string> picked = categories.OrderBy((string c) => random.Next()).Take(random.Next(2, 5)).ToList();
					if (picked.Any((string c) => c.EndsWith("fulllength")))
					{
						picked.RemoveAll((string c) => c.EndsWith("/tops") || c.EndsWith("/bottoms"));
					}
					StringBuilder sb = new StringBuilder("{\"parts\":[");
					for (int j = 0; j < picked.Count; j++)
					{
						List<TemplateDefinition> choices = byCategory[picked[j]];
						TemplateDefinition template = choices[random.Next(choices.Count)];
						if (j > 0)
						{
							sb.Append(',');
						}
						sb.Append("{\"equipmentId\":").Append(random.Next(1, int.MaxValue)).Append(",\"dateTimeCreated\":0,\"definitionId\":").Append(template.Id).Append(",\"parts\":[]}");
					}
					sb.Append("]}");
					outfits.Add(sb.ToString());
				}
				return outfits.ToArray();
			}
			catch (System.Exception)
			{
				return new string[0];
			}
		}

		private static int[] BotEmoteCodes()
		{
			try
			{
				return Service.Get<GameData>().Get<Dictionary<string, EmoteDefinition>>().Values.Where((EmoteDefinition e) => e != null && e.CharacterCode > 0 && e.CharacterCode <= char.MaxValue).Select((EmoteDefinition e) => e.CharacterCode).ToArray();
			}
			catch (System.Exception)
			{
				return new int[0];
			}
		}

		private static int[] BotSizzleClips()
		{
			try
			{
				return Service.Get<GameData>().Get<Dictionary<int, SizzleClipDefinition>>().Keys.Where((int id) => id > 0).ToArray();
			}
			catch (System.Exception)
			{
				return new int[0];
			}
		}

		private static int[] BotTubes()
		{
			HashSet<int> ids = new HashSet<int>();
			try
			{
				foreach (int id in Service.Get<GameData>().Get<Dictionary<int, TubeDefinition>>().Keys)
				{
					ids.Add(id);
				}
			}
			catch (System.Exception)
			{
			}
			if (ids.Count == 0)
			{
				try
				{
					foreach (TubeDefinition def in Resources.FindObjectsOfTypeAll<TubeDefinition>())
					{
						if (def != null)
						{
							ids.Add(def.Id);
						}
					}
				}
				catch (System.Exception)
				{
				}
			}
			return ids.ToArray();
		}

		public static void RefreshBotLooks()
		{
			if (server == null)
			{
				return;
			}
			int[] tubes = BotTubes();
			if (tubes.Length > 0)
			{
				server.Bots.SetTarget(Mathf.Clamp(PlayerPrefs.GetInt(BotsPref, 0), 0, MaxBots), BotColours(), tubes, BotOutfits(), BotEmoteCodes(), BotSizzleClips());
			}
		}

		private static void StopHosting()
		{
			if (server == null)
			{
				return;
			}
			LanGameServerClient client = LanGameServerClient.Instance;
			if (client != null)
			{
				client.SetHost(null);
				ReloadZone();
			}
			if (beacon != null)
			{
				beacon.Stop();
				beacon = null;
			}
			server.DisconnectRemote();
			CoroutineRunner.StartPersistent(stopServerLater(server), owner, "LanStopServer");
			LanBotGridBaker.Detach(server);
			server = null;
		}

		private static void StartJoining()
		{
			if (LanGameServerClient.Instance == null || server != null || joinEnabled)
			{
				return;
			}
			HookQuit();
			LanServicesClient.Ensure();
			joinEnabled = true;
			AcquireMulticast();
			finder = new LanFinder();
			finder.Start();
			if (!watching)
			{
				watching = true;
				CoroutineRunner.StartPersistent(watchHosts(), owner, "LanJoin");
			}
		}

		private static void StopJoining()
		{
			if (!joinEnabled)
			{
				return;
			}
			joinEnabled = false;
			if (finder != null)
			{
				finder.Stop();
				finder = null;
			}
			ReleaseMulticast();
			Disconnect();
		}

		private static void Disconnect()
		{
			if (joinedAddress == null)
			{
				return;
			}
			joinedAddress = null;
			joinedName = null;
			LanGameServerClient client = LanGameServerClient.Instance;
			if (client != null)
			{
				client.SetHost(null);
				ReloadZone();
			}
		}

		private static IEnumerator watchHosts()
		{
			while (joinEnabled)
			{
				LanFinder current = finder;
				LanGameServerClient client = LanGameServerClient.Instance;
				if (current != null && client != null)
				{
					if (joinedAddress == null)
					{
						string address;
						string name;
						if (current.TryGetHost(HostTimeoutSeconds, out address, out name) && CanTransition())
						{
							joinedAddress = address;
							joinedName = name;
							client.SetHost(address);
							ReloadZone();
						}
					}
					else if (!current.IsFresh(joinedAddress, HostTimeoutSeconds) && CanTransition())
					{
						Disconnect();
					}
				}
				yield return new WaitForSeconds(1f);
			}
			watching = false;
		}

		private static IEnumerator stopServerLater(LanServer stopping)
		{
			yield return new WaitForSeconds(ServerStopDelaySeconds);
			stopping.Stop();
		}

		private static string LoadText(string path)
		{
			TextAsset asset = Resources.Load<TextAsset>(path);
			return (asset != null) ? asset.text : null;
		}

		private static void HookQuit()
		{
			if (quitHooked)
			{
				return;
			}
			quitHooked = true;
			Application.quitting += delegate
			{
				joinEnabled = false;
				if (finder != null)
				{
					finder.Stop();
				}
				if (beacon != null)
				{
					beacon.Stop();
				}
				if (server != null)
				{
					server.Stop();
				}
			};
		}

		private static void AcquireMulticast()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			if (multicastLock != null)
			{
				return;
			}
			try
			{
				using (AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				using (AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
				using (AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext"))
				using (AndroidJavaObject wifi = context.Call<AndroidJavaObject>("getSystemService", "wifi"))
				{
					if (wifi == null)
					{
						return;
					}
					AndroidJavaObject created = wifi.Call<AndroidJavaObject>("createMulticastLock", "CPI LAN");
					created.Call("setReferenceCounted", false);
					created.Call("acquire");
					multicastLock = created;
				}
			}
			catch (System.Exception)
			{
				multicastLock = null;
			}
#endif
		}

		private static void ReleaseMulticast()
		{
			if (multicastLock == null)
			{
				return;
			}
			try
			{
				multicastLock.Call("release");
			}
			catch (System.Exception)
			{
			}
			multicastLock.Dispose();
			multicastLock = null;
		}

		private static bool CanTransition()
		{
			if (!Service.IsSet<ZoneTransitionService>())
			{
				return true;
			}
			return !Service.Get<ZoneTransitionService>().IsTransitioning;
		}

		private static void ReloadZone()
		{
			if (!Service.IsSet<ZoneTransitionService>())
			{
				return;
			}
			ZoneTransitionService zones = Service.Get<ZoneTransitionService>();
			if (zones.IsTransitioning || zones.CurrentZone == null || string.IsNullOrEmpty(zones.CurrentZone.SceneName))
			{
				return;
			}
			zones.LoadZone(zones.CurrentZone);
		}
	}
}
#endif
