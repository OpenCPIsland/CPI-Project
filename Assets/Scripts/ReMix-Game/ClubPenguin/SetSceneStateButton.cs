using ClubPenguin.Igloo;
using ClubPenguin.SceneManipulation;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class SetSceneStateButton : MonoBehaviour
	{
		public SceneStateData.SceneState State;

		public bool SendLayoutId;

		[Header("Required if SendLayoutId is true")]
		public SceneLayoutIdComponent SceneLayoutId;

		public bool SetSplashScreen;

		[Header("Required if SetSplashScreen is true")]
		public PrefabContentKey SplashScreenOverride;

		protected EventDispatcher eventDispatcher;

		protected virtual void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			if (SendLayoutId && SceneLayoutId == null)
			{
				Log.LogError(this, "SendLayoutId is set to true, but no layout id is set");
			}
		}

		public void OnClick()
		{
			long layoutId = 0L;
			if (SceneLayoutId != null)
			{
				layoutId = SceneLayoutId.LayoutId;
			}
			eventDispatcher.DispatchEvent(new IglooUIEvents.SetStateButtonPressed(State, SetSplashScreen, SplashScreenOverride, layoutId));
		}
	}
}
