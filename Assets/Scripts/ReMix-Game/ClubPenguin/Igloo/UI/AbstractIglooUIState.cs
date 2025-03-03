using ClubPenguin.Adventure;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Igloo.UI
{
	public abstract class AbstractIglooUIState : MonoBehaviour
	{
		protected CPDataEntityCollection dataEntityCollection;

		protected EventDispatcher eventDispatcher;

		protected EventChannel eventChannel;

		protected virtual void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
		}

		public abstract void OnEnter();

		public abstract void OnExit();

		protected void SuspendActiveQuest()
		{
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.SuspendQuest(activeQuest));
			}
		}
	}
}
