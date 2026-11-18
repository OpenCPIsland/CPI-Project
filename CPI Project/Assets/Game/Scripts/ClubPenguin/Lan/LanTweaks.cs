#if !UNITY_WEBGL
using ClubPenguin.Net.Client;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Net;
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

		private static readonly object owner = new object();

		private static LanServer server;

		private static LanBeacon beacon;

		private static LanFinder finder;

		private static string joinedAddress;

		private static string joinedName;

		private static bool joinEnabled;

		private static bool watching;

		private static bool quitHooked;

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
					return "Hosting on " + LanBeacon.LocalAddress() + " (" + server.PlayerCount + " connected)";
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

		private static void SavePrefs()
		{
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
			StopJoining();
			LanServerData data = new LanServerData();
			data.PartyGames = LoadText("Lan/party-games");
			data.TubeRaces = LoadText("Lan/tube-races");
			data.WorldObjects = LoadText("Lan/world-objects");
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
			CoroutineRunner.StartPersistent(stopServerLater(server), owner, "LanStopServer");
			server = null;
		}

		private static void StartJoining()
		{
			if (LanGameServerClient.Instance == null || server != null || joinEnabled)
			{
				return;
			}
			HookQuit();
			joinEnabled = true;
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
