using System;
using System.Text;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Utils/DebugLog")]
	public class DebugLog : MonoBehaviour
	{
		private static DebugLog _instance = null;

		public bool _breakOnError;

		public bool _enableErrors;

		public bool _enableWarnings;

		public bool _enableInfos;

		private static StringBuilder _stringBuilder = new StringBuilder(512, 512);

		[NonSerialized]
		public bool _destroy;

		public static DebugLog Instance
		{
			get
			{
				if (Application.isEditor && FabricManager.IsInitialised() && FabricManager.Instance._enableDebugLog && _instance == null)
				{
					_instance = FabricManager.Instance.GetComponent<DebugLog>();
					if (_instance == null)
					{
						_instance = (DebugLog)FabricManager.Instance.gameObject.AddComponent(typeof(DebugLog));
					}
				}
				return _instance;
			}
		}

		public void Awake()
		{
			FabricManager.Instance._enableDebugLog = true;
		}

		public static void Print(string Msg, DebugLevel debugLevel = DebugLevel.Info)
		{
			DebugLog instance = Instance;
			if (instance != null)
			{
				instance.HandleDebug("Fabric: " + Msg, debugLevel);
			}
		}

		public static void Print(DebugLevel debugLevel, params string[] Msg)
		{
			DebugLog instance = Instance;
			if (instance != null)
			{
				_stringBuilder.Remove(0, _stringBuilder.Length);
				_stringBuilder.Append("Fabric: ");
				for (int i = 0; i < Msg.Length; i++)
				{
					_stringBuilder.Append(Msg[i]);
				}
				instance.HandleDebug(_stringBuilder.ToString(), debugLevel);
			}
		}

		private void HandleDebug(string Msg, DebugLevel debugLevel)
		{
			if (debugLevel == DebugLevel.Error && _enableErrors)
			{
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					Debug.LogWarning(Msg);
				}
				else
				{
					Debug.LogError(Msg);
				}
			}
			else if (debugLevel == DebugLevel.Warning && _enableWarnings)
			{
				Debug.LogWarning(Msg);
			}
			else if (debugLevel == DebugLevel.Info && _enableInfos)
			{
				Debug.Log(Msg);
			}
		}
	}
}
