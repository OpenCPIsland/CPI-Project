using hg.ApiWebKit.providers;
using System;
using UnityEngine;

namespace hg.ApiWebKit.core.tcp
{
	public class TcpOperation : MonoBehaviour
	{
		[HideInInspector]
		private TcpOneShotClient _client;

		public TcpPath OperationSettings;

		private bool _destroyProvider = false;

		protected virtual void Start()
		{
		}

		protected virtual void OnFailure(Exception exception)
		{
		}

		protected virtual string OnRequest()
		{
			return null;
		}

		protected virtual void OnResponse(string message)
		{
		}

		public void Send(Action<string> onMessage, Action<Exception> onFailure)
		{
			_client = (TcpOneShotClient)Configuration.Bootstrap().AddComponent(OperationSettings.ProviderType);
			_client.Setup(OperationSettings.Hostname, OperationSettings.Port, OperationSettings.ReadBufferSize, OperationSettings.ConnectTimeout, OperationSettings.ReceiveTimeout, OperationSettings.SendTimeout);
			_client.OnMessage += delegate(object sender, EventArgs e)
			{
				if (onMessage != null)
				{
					onMessage((string)sender);
				}
				OnResponse((string)sender);
				operationCompleted();
			};
			_client.OnFailedTransmission += delegate(object sender, EventArgs e)
			{
				if (onFailure != null)
				{
					onFailure((Exception)sender);
				}
				OnFailure((Exception)sender);
				operationCompleted();
			};
			_client.OnConnectionError += delegate(object sender, EventArgs e)
			{
				if (onFailure != null)
				{
					onFailure((Exception)sender);
				}
				OnFailure((Exception)sender);
				operationCompleted();
			};
			_client.OnConnectionSuccess += delegate
			{
			};
			string data = OnRequest();
			_client.Send(data);
		}

		protected virtual void Update()
		{
			if (_destroyProvider)
			{
				_destroyProvider = false;
				UnityEngine.Object.Destroy(_client);
			}
		}

		private void operationCompleted()
		{
			_destroyProvider = true;
		}
	}
}
