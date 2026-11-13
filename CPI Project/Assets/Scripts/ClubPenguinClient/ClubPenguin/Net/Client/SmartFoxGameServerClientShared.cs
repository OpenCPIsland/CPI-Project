#if UNITY_WEBGL
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Client.Smartfox;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Utils;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Org.BouncyCastle.Crypto.Generators;
using Sfs3.Client;
using Sfs3.Client.BitSwarm;
using Sfs3.Client.Entities;
using Sfs3.Client.Entities.Data;
using Sfs3.Client.Entities.Variables;
using Sfs3.Client.Requests;
using Sfs3.Client.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public class SmartFoxGameServerClientShared
	{
		private readonly string zone;

		private readonly bool sfsDebugLogging;

		private readonly bool enableLagMonitor;

		internal readonly bool UseEncryption;

		internal SmartFoxEncryptor SmartFoxEncryptor;

		internal readonly JsonService JsonService;

		internal readonly ClubPenguinClient ClubPenguinClient;

		internal bool TriggerForceTeardownAfterDelay = false;

		internal bool WasTornDownImmediately = false;

		private object smartFoxLock = new object();

		public SmartFox smartFox;

		private object serverTimeLock = new object();

		private long pendingServerTime = 0L;

		private Stopwatch pendingTimer = new Stopwatch();

		internal ConcurrentQueue<KeyValuePair<GameServerEvent, object>> TriggeredEvents = new ConcurrentQueue<KeyValuePair<GameServerEvent, object>>();

		private object joinRoomDataLock = new object();

		private SignedResponse<JoinRoomData> signedJoinRoomData;

		private object roomToLeaveLock = new object();

		private Room sfsRoomToLeave = null;

		private object transientDataLock = new object();

		private SignedResponse<TransientData> lastRoomTransientData;

		internal int ConnectionAttempts;

		private SmartFoxGameServerClientSFSThread sfsThread;

		private string clientRoomName;

		internal object smartFoxRef
		{
			get
			{
				return smartFox;
			}
		}

		internal ConfigData smartFoxConfiguration
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox.Config;
				}
			}
		}

		internal bool isConnected
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox != null && smartFox.IsConnected;
				}
			}
		}

		internal bool isLoggedIn
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox != null && smartFox.MySelf != null;
				}
			}
		}

		internal int UserCount
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox.UserManager.UserCount;
				}
			}
		}

		internal SignedResponse<JoinRoomData> SignedJoinRoomData
		{
			set
			{
				lock (joinRoomDataLock)
				{
					signedJoinRoomData = value;
				}
			}
		}

		internal string JoinRoomDataRoom
		{
			get
			{
				lock (joinRoomDataLock)
				{
					if (signedJoinRoomData != null)
					{
						return signedJoinRoomData.Data.room;
					}
					return null;
				}
			}
		}

		internal string JoinRoomDataUserName
		{
			get
			{
				lock (joinRoomDataLock)
				{
					if (signedJoinRoomData != null)
					{
						return signedJoinRoomData.Data.userName;
					}
					return null;
				}
			}
		}

		internal long JoinRoomDataSessionId
		{
			get
			{
				lock (joinRoomDataLock)
				{
					if (signedJoinRoomData != null)
					{
						return signedJoinRoomData.Data.sessionId;
					}
					return 0L;
				}
			}
		}

		internal Room SfsRoomToLeave
		{
			set
			{
				lock (roomToLeaveLock)
				{
					sfsRoomToLeave = value;
				}
			}
		}

		internal SignedResponse<TransientData> LastRoomTransientData
		{
			get
			{
				lock (transientDataLock)
				{
					return lastRoomTransientData;
				}
			}
			set
			{
				lock (transientDataLock)
				{
					lastRoomTransientData = value;
				}
			}
		}

		internal string ClientRoomName
		{
			get
			{
				if (smartFox != null)
				{
					lock (smartFox)
					{
						return clientRoomName;
					}
				}
				return null;
			}
			set
			{
				if (smartFox != null)
				{
					lock (smartFox)
					{
						clientRoomName = value;
					}
				}
			}
		}

		internal bool TryGetUserVariable(string key, out UserVariable var)
		{
			bool result = false;
			var = null;
			lock (smartFoxLock)
			{
				if (smartFox != null && smartFox.MySelf != null && smartFox.MySelf.ContainsVariable(key))
				{
					var = smartFox.MySelf.GetVariable(key);
					result = true;
				}
			}
			return result;
		}

		internal void Disconnect()
		{
			lock (smartFoxLock)
			{
				if (smartFox != null)
				{
					smartFox.Disconnect();
				}
			}
		}

		internal void Reconnect()
		{
			lock (smartFoxLock)
			{
				ConnectionAttempts = 0;
				reconnect();
			}
		}

		internal User GetUserById(int userId)
		{
			lock (smartFoxLock)
			{
				return smartFox.UserManager.GetUserById(userId);
			}
		}

		internal void SetServerTimeUpdate(long serverTime)
		{
			lock (serverTimeLock)
			{
				pendingServerTime = serverTime;
				pendingTimer.Reset();
				pendingTimer.Start();
			}
		}

		internal bool TryGetServerTimeUpdate(out long serverTime)
		{
			bool result = false;
			serverTime = 0L;
			if (pendingServerTime != 0)
			{
				lock (serverTimeLock)
				{
					if (pendingServerTime != 0)
					{
						serverTime = pendingServerTime + pendingTimer.ElapsedMilliseconds;
						result = true;
						pendingTimer.Stop();
						pendingServerTime = 0L;
					}
				}
			}
			return result;
		}

		internal bool TryClearJoinRoomData(out JoinRoomData joinRoomData)
		{
			bool result = false;
			joinRoomData = default(JoinRoomData);
			lock (joinRoomDataLock)
			{
				if (signedJoinRoomData != null)
				{
					result = true;
					joinRoomData = signedJoinRoomData.Data;
					signedJoinRoomData = null;
				}
			}
			return result;
		}

		internal bool TryClearJoinRoomDataIfRoom(string room, out JoinRoomData joinRoomData)
		{
			bool result = false;
			joinRoomData = default(JoinRoomData);
			lock (joinRoomDataLock)
			{
				if (signedJoinRoomData != null && RoomIdentifier.EqualsIgnoreInstanceId(signedJoinRoomData.Data.room, room))
				{
					result = true;
					joinRoomData = signedJoinRoomData.Data;
					signedJoinRoomData = null;
				}
			}
			return result;
		}

		internal bool TryClearSfsRoomToLeave(out Room room)
		{
			bool result = false;
			lock (roomToLeaveLock)
			{
				result = (sfsRoomToLeave != null);
				room = sfsRoomToLeave;
				sfsRoomToLeave = null;
			}
			return result;
		}

		internal SmartFoxGameServerClientShared(ClubPenguinClient clubPenguinClient, string gameZone, bool gameEncryption, bool gameDebugging, bool lagMonitoring)
		{
			zone = gameZone;
			UseEncryption = true;
			sfsDebugLogging = gameDebugging;
			enableLagMonitor = lagMonitoring;
			ClubPenguinClient = clubPenguinClient;
			JsonService = Service.Get<JsonService>();
			sfsThread = new SmartFoxGameServerClientSFSThread(this);
			setup();
		}

		private void setup()
		{
			lock (smartFoxLock)
			{
				if (smartFox == null)
				{

                    smartFox = new SmartFox();
                    UnityEngine.Debug.Log("After creating SmartFox: Created SFS instance: " + smartFox.GetHashCode());
					SmartFoxFramePump.Register(() => smartFox?.ProcessEvents());
                    sfsThread.AddListeners(smartFox);
                }
			}
		}

		internal void teardown()
		{
			lock (smartFoxLock)
			{
				clientRoomName = null;
				if (smartFox != null)
				{
					sfsThread.RemoveListeners(smartFox);
					smartFox = null;
				}
			}
		}

		internal void login()
		{
			string joinRoomDataUserName = JoinRoomDataUserName;
			string password = "";
			SFSObject sFSObject = new SFSObject();
			string val;
			lock (joinRoomDataLock)
			{
				val = JsonService.Serialize(signedJoinRoomData);
			}
			sFSObject.PutText("joinRoomData", val);
			lock (transientDataLock)
			{
				if (lastRoomTransientData != null)
				{
					sFSObject.PutString("transientData", JsonService.Serialize(lastRoomTransientData));
				}
			}
			send(new LoginRequest(joinRoomDataUserName, password, zone, sFSObject));
		}

		internal void onLogin()
		{
			if (enableLagMonitor)
			{
				lock (smartFoxLock)
				{
					smartFox.EnableLagMonitor(true, 1000, 10);
				}
			}
		}

		internal void connectUdp()
		{
			lock (smartFoxLock)
			{
				smartFox.ConnectUdp();
			}
		}

		internal void send(SmartfoxCommand socketCommands, IDictionary<string, SFSDataWrapper> parameters = null, object[] commandParameters = null, bool useUDP = false)
		{
			ISFSObject iSFSObject = SFSObject.NewInstance();
			if (parameters != null)
			{
				foreach (string key in parameters.Keys)
				{
					iSFSObject.Put(key, parameters[key]);
				}
			}
			Room currentRoom = getCurrentRoom();
			if (currentRoom != null)
			{
				string text = socketCommands.GetCommand();
				if (commandParameters != null)
				{
					text = string.Format(text, commandParameters);
				}
				TransportType transportType = useUDP && SmartFoxGameServerClient.EnableUDP ? TransportType.UDP : TransportType.TCP;
				send(new ExtensionRequest(text, iSFSObject, currentRoom, transportType));
			}
		}

		internal void send(IClientRequest request)
		{
			lock (smartFoxLock)
			{
				smartFox.Send(request);
			}
		}

		internal Room getCurrentRoom()
		{
			lock (smartFoxLock)
			{
				if (smartFox != null && smartFox.GetJoinedRooms().Count > 0)
				{
					return smartFox.GetJoinedRooms()[0];
				}
				return null;
			}
		}

		internal void triggerEvent(GameServerEvent gameServerEvent, object data)
		{
			TriggeredEvents.Enqueue(new KeyValuePair<GameServerEvent, object>(gameServerEvent, data));
		}

		internal void connect(string serverIP, int serverTcpPort, int serverHttpsPort)
		{
			lock (smartFoxLock)
			{
				setup();
				ConnectionAttempts = 0;
                ConfigData configData = new ConfigData();
				configData.Host = serverIP;
				configData.Port = serverTcpPort;
                configData.HttpsPort = serverHttpsPort;
				configData.Zone = zone;
				configData.NetDebugLevel = sfsDebugLogging ? NetDebugLevel.PROTOCOL : NetDebugLevel.OFF;
				configData.UseWebSocket = true;
				configData.UseSSL = UseEncryption;
				if (configData.BlueBox != null)
				{
					configData.BlueBox.IsActive = false;
				}

				configData.UdpPort = serverTcpPort;
                configData.Port = UseEncryption ? serverHttpsPort : configData.HttpPort;
                UnityEngine.Debug.Log("Before Connect: Created SFS instance: " + smartFox.GetHashCode());
                smartFox.Connect(configData);
            }
		}


        internal void reconnect()
		{
			ConnectionAttempts++;
			string host;
			int tcpPort;
			int httpsPort;
			lock (joinRoomDataLock)
			{
				host = signedJoinRoomData.Data.host;
				tcpPort = signedJoinRoomData.Data.tcpPort;
				httpsPort = signedJoinRoomData.Data.httpsPort;
			}
			lock (smartFoxLock)
			{
				teardown();
				setup();
				ConfigData configData = new ConfigData();
				configData.Host = host;
				configData.Port = tcpPort;
				configData.HttpsPort = httpsPort;
				configData.Zone = zone;
				configData.NetDebugLevel = sfsDebugLogging ? NetDebugLevel.PROTOCOL : NetDebugLevel.OFF;
				configData.UseWebSocket = true;
				configData.UseSSL = UseEncryption;
				if (configData.BlueBox != null)
				{
					configData.BlueBox.IsActive = false;
				}

				configData.UdpPort = tcpPort;
                configData.Port = UseEncryption ? httpsPort : configData.HttpPort;
                smartFox.Connect(configData);
            }
		}

        internal static SFSDataWrapper serialize(byte b)
		{
			return new SFSDataWrapper(SFSDataType.BYTE, b);
		}

		internal static SFSDataWrapper serialize(long l)
		{
			return new SFSDataWrapper(SFSDataType.LONG, l);
		}

		internal static SFSDataWrapper serialize(string str)
		{
			return serialize(str, false);
		}

		internal static SFSDataWrapper serialize(string str, bool asText)
		{
			if (asText)
			{
				return new SFSDataWrapper(SFSDataType.TEXT, str);
			}
			return new SFSDataWrapper(SFSDataType.STRING, str);
		}

		internal static SFSDataWrapper serialize(Vector3 vec)
		{
			return new SFSDataWrapper(SFSDataType.FLOAT_ARRAY, new float[3]
			{
				vec.x,
				vec.y,
				vec.z
			});
		}

		internal static SFSDataWrapper serialize(Vector2 vec)
		{
			return new SFSDataWrapper(SFSDataType.FLOAT_ARRAY, new float[2]
			{
				vec.x,
				vec.y
			});
		}

		internal static Vector3 deserializeVec3(ISFSObject props, string key)
		{
			float[] floatArray = props.GetFloatArray(key).ToArray();
			return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
		}

		internal static Vector2 deserializeVec2(ISFSObject props, string key)
		{
			float[] floatArray = props.GetFloatArray(key).ToArray();
			return new Vector2(floatArray[0], floatArray[1]);
		}
	}
}
#else
using ClubPenguin.Net.Client.Event;
using ClubPenguin.Net.Client.Smartfox;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Utils;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Org.BouncyCastle.Crypto.Generators;
using Sfs3.Client;
using Sfs3.Client.BitSwarm;
using Sfs3.Client.Entities;
using Sfs3.Client.Entities.Data;
using Sfs3.Client.Entities.Variables;
using Sfs3.Client.Requests;
using Sfs3.Client.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	public class SmartFoxGameServerClientShared
	{
		private readonly string zone;

		private readonly bool sfsDebugLogging;

		private readonly bool enableLagMonitor;

		internal readonly bool UseEncryption;

		internal SmartFoxEncryptor SmartFoxEncryptor;

		internal readonly JsonService JsonService;

		internal readonly ClubPenguinClient ClubPenguinClient;

		internal bool TriggerForceTeardownAfterDelay = false;

		internal bool WasTornDownImmediately = false;

		private object smartFoxLock = new object();

		public SmartFox smartFox;

		private object serverTimeLock = new object();

		private long pendingServerTime = 0L;

		private Stopwatch pendingTimer = new Stopwatch();


		internal ConcurrentQueue<KeyValuePair<GameServerEvent, object>> TriggeredEvents = new ConcurrentQueue<KeyValuePair<GameServerEvent, object>>();

		private object joinRoomDataLock = new object();

		private SignedResponse<JoinRoomData> signedJoinRoomData;

		private object roomToLeaveLock = new object();

		private Room sfsRoomToLeave = null;

		private object transientDataLock = new object();

		private SignedResponse<TransientData> lastRoomTransientData;

		internal int ConnectionAttempts;

		private SmartFoxGameServerClientSFSThread sfsThread;

		private string clientRoomName;

		internal object smartFoxRef
		{
			get
			{
				return smartFox;
			}
		}

		internal ConfigData smartFoxConfiguration
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox.Config;
				}
			}
		}

		internal bool isConnected
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox != null && smartFox.IsConnected;
				}
			}
		}

		internal bool isLoggedIn
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox != null && smartFox.MySelf != null;
				}
			}
		}

		internal int UserCount
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox.UserManager.UserCount;
				}
			}
		}

		internal SignedResponse<JoinRoomData> SignedJoinRoomData
		{
			set
			{
				lock (joinRoomDataLock)
				{
					signedJoinRoomData = value;
				}
			}
		}

		internal string JoinRoomDataRoom
		{
			get
			{
				lock (joinRoomDataLock)
				{
					if (signedJoinRoomData != null)
					{
						return signedJoinRoomData.Data.room;
					}
					return null;
				}
			}
		}

		internal string JoinRoomDataUserName
		{
			get
			{
				lock (joinRoomDataLock)
				{
					if (signedJoinRoomData != null)
					{
						return signedJoinRoomData.Data.userName;
					}
					return null;
				}
			}
		}

		internal long JoinRoomDataSessionId
		{
			get
			{
				lock (joinRoomDataLock)
				{
					if (signedJoinRoomData != null)
					{
						return signedJoinRoomData.Data.sessionId;
					}
					return 0L;
				}
			}
		}

		internal Room SfsRoomToLeave
		{
			set
			{
				lock (roomToLeaveLock)
				{
					sfsRoomToLeave = value;
				}
			}
		}

		internal SignedResponse<TransientData> LastRoomTransientData
		{
			get
			{
				lock (transientDataLock)
				{
					return lastRoomTransientData;
				}
			}
			set
			{
				lock (transientDataLock)
				{
					lastRoomTransientData = value;
				}
			}
		}

		internal string ClientRoomName
		{
			get
			{
				if (smartFox != null)
				{
					lock (smartFox)
					{
						return clientRoomName;
					}
				}
				return null;
			}
			set
			{
				if (smartFox != null)
				{
					lock (smartFox)
					{
						clientRoomName = value;
					}
				}
			}
		}

		internal bool TryGetUserVariable(string key, out UserVariable var)
		{
			bool result = false;
			var = null;
			lock (smartFoxLock)
			{
				if (smartFox != null && smartFox.MySelf != null && smartFox.MySelf.ContainsVariable(key))
				{
					var = smartFox.MySelf.GetVariable(key);
					result = true;
				}
			}
			return result;
		}

		internal void Disconnect()
		{
			lock (smartFoxLock)
			{
				if (smartFox != null)
				{
					smartFox.Disconnect();
				}
			}
		}

		internal void Reconnect()
		{
			lock (smartFoxLock)
			{
				ConnectionAttempts = 0;
				reconnect();
			}
		}

		internal User GetUserById(int userId)
		{
			lock (smartFoxLock)
			{
				return smartFox.UserManager.GetUserById(userId);
			}
		}

		internal void SetServerTimeUpdate(long serverTime)
		{
			lock (serverTimeLock)
			{
				pendingServerTime = serverTime;
				pendingTimer.Reset();
				pendingTimer.Start();
			}
		}

		internal bool TryGetServerTimeUpdate(out long serverTime)
		{
			bool result = false;
			serverTime = 0L;
			if (pendingServerTime != 0)
			{
				lock (serverTimeLock)
				{
					if (pendingServerTime != 0)
					{
						serverTime = pendingServerTime + pendingTimer.ElapsedMilliseconds;
						result = true;
						pendingTimer.Stop();
						pendingServerTime = 0L;
					}
				}
			}
			return result;
		}

		internal bool TryClearJoinRoomData(out JoinRoomData joinRoomData)
		{
			bool result = false;
			joinRoomData = default(JoinRoomData);
			lock (joinRoomDataLock)
			{
				if (signedJoinRoomData != null)
				{
					result = true;
					joinRoomData = signedJoinRoomData.Data;
					signedJoinRoomData = null;
				}
			}
			return result;
		}

		internal bool TryClearJoinRoomDataIfRoom(string room, out JoinRoomData joinRoomData)
		{
			bool result = false;
			joinRoomData = default(JoinRoomData);
			lock (joinRoomDataLock)
			{
				if (signedJoinRoomData != null && RoomIdentifier.EqualsIgnoreInstanceId(signedJoinRoomData.Data.room, room))
				{
					result = true;
					joinRoomData = signedJoinRoomData.Data;
					signedJoinRoomData = null;
				}
			}
			return result;
		}

		internal bool TryClearSfsRoomToLeave(out Room room)
		{
			bool result = false;
			lock (roomToLeaveLock)
			{
				result = (sfsRoomToLeave != null);
				room = sfsRoomToLeave;
				sfsRoomToLeave = null;
			}
			return result;
		}

		internal static bool IsLocalGameServerHost(string host)
		{
			if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) || host == "127.0.0.1")
			{
				return true;
			}

			IPAddress hostAddress;
			if (!IPAddress.TryParse(host, out hostAddress))
			{
				return false;
			}

			try
			{
				NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				for (int i = 0; i < networkInterfaces.Length; i++)
				{
					if (networkInterfaces[i].OperationalStatus != OperationalStatus.Up)
					{
						continue;
					}

					UnicastIPAddressInformationCollection addresses = networkInterfaces[i].GetIPProperties().UnicastAddresses;
					foreach (UnicastIPAddressInformation address in addresses)
					{
						if (address.Address.Equals(hostAddress))
						{
							return true;
						}

						if (hostAddress.AddressFamily == AddressFamily.InterNetwork && address.Address.AddressFamily == AddressFamily.InterNetwork && address.IPv4Mask != null && IsSameIPv4Network(address.Address, hostAddress, address.IPv4Mask))
						{
							return true;
						}
					}
				}
			}
			catch (NetworkInformationException)
			{
				return false;
			}

			return false;
		}

		private static bool IsSameIPv4Network(IPAddress localAddress, IPAddress hostAddress, IPAddress subnetMask)
		{
			byte[] localBytes = localAddress.GetAddressBytes();
			byte[] hostBytes = hostAddress.GetAddressBytes();
			byte[] maskBytes = subnetMask.GetAddressBytes();
			for (int i = 0; i < maskBytes.Length; i++)
			{
				if ((localBytes[i] & maskBytes[i]) != (hostBytes[i] & maskBytes[i]))
				{
					return false;
				}
			}

			return true;
		}

		internal SmartFoxGameServerClientShared(ClubPenguinClient clubPenguinClient, string gameZone, bool gameEncryption, bool gameDebugging, bool lagMonitoring)
		{
			zone = gameZone;
			string gameServerHost = Service.Get<ICommonGameSettings>().GameServerHost;
			UseEncryption = !IsLocalGameServerHost(gameServerHost);
			sfsDebugLogging = gameDebugging;
			enableLagMonitor = lagMonitoring;
			ClubPenguinClient = clubPenguinClient;
			JsonService = Service.Get<JsonService>();
			sfsThread = new SmartFoxGameServerClientSFSThread(this);
			setup();
		}

        private void setup()
        {
            lock (smartFoxLock)
            {
                if (smartFox == null)
                {

                    smartFox = new SmartFox();
                    UnityEngine.Debug.Log("After creating SmartFox: Created SFS instance: " + smartFox.GetHashCode());
					SmartFoxFramePump.Register(() => smartFox?.ProcessEvents());
                    sfsThread.AddListeners(smartFox);
                }
            }
        }

        internal void teardown()
		{
			lock (smartFoxLock)
			{
				clientRoomName = null;
				if (smartFox != null)
				{
					sfsThread.RemoveListeners(smartFox);
					smartFox = null;
				}
			}
		}

		internal void login()
		{
			string joinRoomDataUserName = JoinRoomDataUserName;
			string password = "";
			SFSObject sFSObject = new SFSObject();
			string val;
			lock (joinRoomDataLock)
			{
				val = JsonService.Serialize(signedJoinRoomData);
			}
			sFSObject.PutText("joinRoomData", val);
			lock (transientDataLock)
			{
				if (lastRoomTransientData != null)
				{
					sFSObject.PutString("transientData", JsonService.Serialize(lastRoomTransientData));
				}
			}
			send(new LoginRequest(joinRoomDataUserName, password, zone, sFSObject));
		}

		internal void onLogin()
		{
			if (enableLagMonitor)
			{
				lock (smartFoxLock)
				{
					smartFox.EnableLagMonitor(true, 1000, 10);
				}
			}
		}

		internal void connectUdp()
		{
			lock (smartFoxLock)
			{
				smartFox.ConnectUdp();
			}
		}

		internal void send(SmartfoxCommand socketCommands, IDictionary<string, SFSDataWrapper> parameters = null, object[] commandParameters = null, bool useUDP = false)
		{
			ISFSObject iSFSObject = SFSObject.NewInstance();
			if (parameters != null)
			{
				foreach (string key in parameters.Keys)
				{
					iSFSObject.Put(key, parameters[key]);
				}
			}
			Room currentRoom = getCurrentRoom();
			if (currentRoom != null)
			{
				string text = socketCommands.GetCommand();
				if (commandParameters != null)
				{
					text = string.Format(text, commandParameters);
				}
				TransportType transportType = useUDP && SmartFoxGameServerClient.EnableUDP ? TransportType.UDP : TransportType.TCP;
				send(new ExtensionRequest(text, iSFSObject, currentRoom, transportType));
			}
		}

		internal void send(IClientRequest request)
		{
			lock (smartFoxLock)
			{
				if (smartFox == null || !smartFox.IsConnected)
				{
					return;
				}

				try
				{
					smartFox.Send(request);
				}
				catch (InvalidOperationException)
				{
                    smartFox.Disconnect();
                }
			}
		}

		internal Room getCurrentRoom()
		{
			lock (smartFoxLock)
			{
				if (smartFox != null && smartFox.GetJoinedRooms().Count > 0)
				{
					return smartFox.GetJoinedRooms()[0];
				}
				return null;
			}
		}

		internal void triggerEvent(GameServerEvent gameServerEvent, object data)
		{
			TriggeredEvents.Enqueue(new KeyValuePair<GameServerEvent, object>(gameServerEvent, data));
		}

		internal void connect(string serverIP, int serverTcpPort, int serverHttpsPort)
		{
			lock (smartFoxLock)
			{
				setup();
				ConnectionAttempts = 0;
                ConfigData configData = new ConfigData();
				configData.Host = serverIP;
				configData.Port = serverTcpPort;
                configData.HttpsPort = serverHttpsPort;
				configData.Zone = zone;
				configData.NetDebugLevel = sfsDebugLogging ? NetDebugLevel.PROTOCOL : NetDebugLevel.OFF;
				configData.UseWebSocket = true;
				configData.UseSSL = UseEncryption;
				if (configData.BlueBox != null)
				{
					configData.BlueBox.IsActive = false;
				}

				configData.UdpPort = serverTcpPort;
                configData.Port = UseEncryption ? serverHttpsPort : configData.HttpPort;
                UnityEngine.Debug.Log("Before Connect: Created SFS instance: " + smartFox.GetHashCode());
                smartFox.Connect(configData);
            }
		}


        internal void reconnect()
		{
			ConnectionAttempts++;
			string host;
			int tcpPort;
			int httpsPort;
			lock (joinRoomDataLock)
			{
				host = signedJoinRoomData.Data.host;
				tcpPort = signedJoinRoomData.Data.tcpPort;
				httpsPort = signedJoinRoomData.Data.httpsPort;
			}
			lock (smartFoxLock)
			{
				teardown();
				setup();
				ConfigData configData = new ConfigData();
				configData.Host = host;
				configData.Port = tcpPort;
				configData.HttpsPort = httpsPort;
				configData.Zone = zone;
				configData.NetDebugLevel = sfsDebugLogging ? NetDebugLevel.PROTOCOL : NetDebugLevel.OFF;
				configData.UseWebSocket = true;
				configData.UseSSL = UseEncryption;
				if (configData.BlueBox != null)
				{
					configData.BlueBox.IsActive = false;
				}

				configData.UdpPort = tcpPort;
                configData.Port = UseEncryption ? httpsPort : configData.HttpPort;
                smartFox.Connect(configData);
            }
		}

        internal static SFSDataWrapper serialize(byte b)
		{
			return new SFSDataWrapper(SFSDataType.BYTE, b);
		}

		internal static SFSDataWrapper serialize(long l)
		{
			return new SFSDataWrapper(SFSDataType.LONG, l);
		}

		internal static SFSDataWrapper serialize(string str)
		{
			return serialize(str, false);
		}

		internal static SFSDataWrapper serialize(string str, bool asText)
		{
			if (asText)
			{
				return new SFSDataWrapper(SFSDataType.TEXT, str);
			}
			return new SFSDataWrapper(SFSDataType.STRING, str);
		}

		internal static SFSDataWrapper serialize(Vector3 vec)
		{
			return new SFSDataWrapper(SFSDataType.FLOAT_ARRAY, new float[3]
			{
				vec.x,
				vec.y,
				vec.z
			});
		}

		internal static SFSDataWrapper serialize(Vector2 vec)
		{
			return new SFSDataWrapper(SFSDataType.FLOAT_ARRAY, new float[2]
			{
				vec.x,
				vec.y
			});
		}

		internal static Vector3 deserializeVec3(ISFSObject props, string key)
		{
			float[] floatArray = props.GetFloatArray(key).ToArray();
			return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
		}

		internal static Vector2 deserializeVec2(ISFSObject props, string key)
		{
			float[] floatArray = props.GetFloatArray(key).ToArray();
			return new Vector2(floatArray[0], floatArray[1]);
		}
	}
}
#endif
