using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Net
{
	public class ConnectionManager : MonoBehaviour
	{
		public enum NetworkConnectionState
		{
			NoConnection,
			BasicConnection
		}

		private const float pingTimeout = 2f;

		private const string pingAddress = "8.8.8.8";

		public NetworkConnectionState ConnectionState
		{
			get
			{
				if (Service.Get<GameSettings>().OfflineMode)
				{
					return NetworkConnectionState.BasicConnection;
				}
				return (Application.internetReachability != 0) ? NetworkConnectionState.BasicConnection : NetworkConnectionState.NoConnection;
			}
		}

		public void DoPingCheck(Action<NetworkConnectionState> callback)
		{
			CoroutineRunner.Start(doPingCheck(callback), this, "Connection Ping Check");
		}

		private IEnumerator doPingCheck(Action<NetworkConnectionState> callback)
		{
			float pingStartTime = Time.unscaledTime;
			NetworkConnectionState connectionState = ConnectionState;
			if (Service.Get<GameSettings>().OfflineMode)
			{
				callback(NetworkConnectionState.BasicConnection);
				yield break;
			}
			bool isPinging = true;
			Ping ping = new Ping("8.8.8.8");
			while (isPinging)
			{
				if (ping.isDone)
				{
					connectionState = ((ping.time >= 0) ? NetworkConnectionState.BasicConnection : NetworkConnectionState.NoConnection);
					isPinging = false;
				}
				else if (Time.unscaledTime - pingStartTime >= 2f)
				{
					connectionState = NetworkConnectionState.NoConnection;
					isPinging = false;
				}
				yield return null;
			}
			callback(connectionState);
		}
	}
}
