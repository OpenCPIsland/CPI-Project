using ClubPenguin.Net.Client.Smartfox;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Utils;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Sfs2X;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Util;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	internal class SmartFoxGameServerClientShared
	{
		private readonly string zone;

		private readonly bool sfsDebugLogging;

		private readonly bool enableLagMonitor;

		internal readonly bool UseEncryption;

		internal SmartFoxEncryptor SmartFoxEncryptor;

		internal readonly JsonService JsonService;

		internal readonly ClubPenguinClient ClubPenguinClient;

		internal bool TriggerInitCrypto = false;

		internal bool TriggerForceTeardownAfterDelay = false;

		internal bool WasTornDownImmediately = false;

		private object smartFoxLock = new object();

		private SmartFox smartFox;

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

		private bool isUDPEstablished
		{
			get
			{
				lock (smartFoxLock)
				{
					return smartFox.UdpInited;
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

		internal IEnumerator InitCrypto()
		{
			lock (smartFoxLock)
			{
				return smartFox.InitCrypto();
			}
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
			UseEncryption = gameEncryption;
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
					smartFox = new SmartFox(sfsDebugLogging);
					smartFox.ThreadSafeMode = false;
					smartFox.UseBlueBox = false;
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
					sFSObject.PutUtfString("transientData", JsonService.Serialize(lastRoomTransientData));
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
					smartFox.EnableLagMonitor(true);
				}
			}
		}

		internal void initUDP()
		{
			lock (smartFoxLock)
			{
				smartFox.InitUDP();
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
				send(new ExtensionRequest(text, iSFSObject, currentRoom, useUDP && isUDPEstablished));
			}
		}

		internal void send(IRequest request)
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
				if (smartFox != null && smartFox.JoinedRooms.Count > 0)
				{
					return smartFox.JoinedRooms[0];
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
				configData.Debug = sfsDebugLogging;
				configData.UseBlueBox = false;
				configData.UdpHost = serverIP;
				configData.UdpPort = serverTcpPort;
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
				configData.Debug = sfsDebugLogging;
				configData.UseBlueBox = false;
				configData.UdpHost = host;
				configData.UdpPort = tcpPort;
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
			return new SFSDataWrapper(SFSDataType.UTF_STRING, str);
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
			float[] floatArray = props.GetFloatArray(key);
			return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
		}

		internal static Vector2 deserializeVec2(ISFSObject props, string key)
		{
			float[] floatArray = props.GetFloatArray(key);
			return new Vector2(floatArray[0], floatArray[1]);
		}
	}
}
