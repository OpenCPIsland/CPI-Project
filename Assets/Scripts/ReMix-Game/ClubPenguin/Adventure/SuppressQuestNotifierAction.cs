using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System.Collections;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class SuppressQuestNotifierAction : FsmStateAction
	{
		public bool Suppress;

		public bool AutoShow;

		public FsmString SceneName;

		public override void Reset()
		{
			SceneName = new FsmString
			{
				UseVariable = true
			};
			Suppress = true;
			AutoShow = true;
		}

		public override void OnEnter()
		{
			if (!SceneName.IsNone && !string.IsNullOrEmpty(SceneName.Value))
			{
				if (SceneManager.GetActiveScene().name == SceneName.Value)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.PermanentlySuppressQuestNotifier(Suppress, AutoShow));
				}
				Finish();
			}
			else
			{
				CoroutineRunner.StartPersistent(SendSuppressEvent(), this, "SuppressQuestNotifierAction");
			}
		}

		private IEnumerator SendSuppressEvent()
		{
			bool handled = Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.SuppressQuestNotifier(Suppress, AutoShow));
			while (!handled)
			{
				yield return null;
				handled = Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.SuppressQuestNotifier(Suppress, AutoShow));
			}
			Finish();
		}
	}
}
