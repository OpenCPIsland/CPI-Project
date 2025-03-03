using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitEnvironmentManagerAction : InitActionComponent
	{
		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = typeof(EnvironmentManager).Name;
			EnvironmentManager environmentManager = gameObject.AddComponent<EnvironmentManagerStandalone>();
			Service.Set(environmentManager);
			environmentManager.SetLogger(LoggerDelegate);
			environmentManager.Initialize();
			yield break;
		}

		private void LoggerDelegate(object sourceObject, string message, LogType logType)
		{
			switch (logType)
			{
			case LogType.Warning:
				break;
			case LogType.Log:
				break;
			case LogType.Exception:
				Log.LogFatal(sourceObject, message);
				break;
			case LogType.Error:
			case LogType.Assert:
				Log.LogError(sourceObject, message);
				break;
			}
		}
	}
}
