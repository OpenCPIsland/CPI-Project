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

		private const string RememberPref = "LAN.Remember";

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
			server.DisconnectRemote();
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
