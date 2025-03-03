using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class ChatActivityData : BaseData
	{
		private const float timeOutSeconds = 5f;

		public Action OnTimeOutComplete;

		public Action SendChatActivity;

		private ICoroutine chatActivityCancelTimer;

		public bool IsChatActive
		{
			get;
			private set;
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(ChatActivityDataMonoBehaviour);
			}
		}

		public void OnSendChatActivity()
		{
			if (!IsChatActive)
			{
				IsChatActive = true;
			}
			else
			{
				stopCoroutine(chatActivityCancelTimer);
			}
			chatActivityCancelTimer = CoroutineRunner.Start(startChatActivityCancelTimer(5f), this, "startChatActivityTimeOut");
		}

		public void OnSetChatActiveCancel()
		{
			IsChatActive = false;
			stopCoroutine(chatActivityCancelTimer);
		}

		public void OnSendChatMessage()
		{
			IsChatActive = false;
			stopCoroutine(chatActivityCancelTimer);
		}

		private IEnumerator startChatActivityCancelTimer(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			IsChatActive = false;
			if (OnTimeOutComplete != null)
			{
				OnTimeOutComplete();
			}
		}

		private void stopCoroutine(ICoroutine coroutine)
		{
			if (coroutine != null && !coroutine.Disposed)
			{
				coroutine.Stop();
			}
		}

		protected override void notifyWillBeDestroyed()
		{
			CoroutineRunner.StopAllForOwner(this);
			OnTimeOutComplete = null;
			SendChatActivity = null;
		}
	}
}
