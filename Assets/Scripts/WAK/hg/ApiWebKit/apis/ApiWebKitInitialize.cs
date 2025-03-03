using hg.ApiWebKit.providers;
using System;
using UnityEngine;

namespace hg.ApiWebKit.apis
{
	public class ApiWebKitInitialize : MonoBehaviour
	{
		public bool DestroyOperationOnCompletion = true;

		public float YieldTime = 0f;

		public virtual void Awake()
		{
			Configuration.SetSetting("log-internal", false);
			Configuration.SetSetting("log-VERBOSE", false);
			Configuration.SetSetting("log-DEBUG", true);
			Configuration.SetSetting("log-INFO", true);
			Configuration.SetSetting("log-WARNING", true);
			Configuration.SetSetting("log-ERROR", true);
			Configuration.SetSetting("log-callback", (Action<string, LogSeverity>)delegate(string message, LogSeverity severity)
			{
				Debug.Log("(callback) " + message);
			});
			Configuration.SetSetting("destroy-operation-on-completion", DestroyOperationOnCompletion);
			Configuration.SetSetting("default-http-client", typeof(HttpWWWClient));
			Configuration.SetSetting("request-timeout", 10f);
			Configuration.SetSetting("yield-time", YieldTime);
		}

		public virtual void Start()
		{
			Configuration.Bootstrap();
		}
	}
}
