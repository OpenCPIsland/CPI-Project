using ClubPenguin.Analytics;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneAppButton : MonoBehaviour
	{
		public CellPhoneAppBehaviour Behaviour;

		public string BehaviourParam;

		public string LoadingScreenOverride;

		public string AppName;

		public int AppletSceneSystemMemoryThreshold = -1;

		private void Start()
		{
		}

		public void OnButtonPressed()
		{
			logAppOpenBi();
			Service.Get<EventDispatcher>().DispatchEvent(new CellPhoneEvents.ChangeCellPhoneScreen(Behaviour, BehaviourParam, AppName, LoadingScreenOverride, AppletSceneSystemMemoryThreshold));
		}

		private void logAppOpenBi()
		{
			Service.Get<ICPSwrveService>().Action("flipper_phone_app", AppName);
		}
	}
}
