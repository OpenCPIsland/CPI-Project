// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether frameReady events are enabled")]
	public class VideoPlayerGetSendFrameReadyEvents : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isSendingFrameReadyEvents;

		[Tooltip("Event sent if frameReady events are sent")]
		public FsmEvent isSendingFrameReadyEventsEvent;

		[Tooltip("Event sent if frameReady events are not sent")]
		public FsmEvent isNotSendingFrameReadyEventsEvent;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		GameObject go;

		VideoPlayer _vp;

		int _isSendingFrameReadyEvents = -1;

		public override void Reset()
		{
			gameObject = null;
			isSendingFrameReadyEvents = null;
			isSendingFrameReadyEventsEvent = null;
			isNotSendingFrameReadyEventsEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			ExecuteAction ();
		}

		public override void OnUpdate()
		{
			ExecuteAction ();
		}


		void ExecuteAction()
		{
			if (_vp == null)
			{
				return;
			}

			if (_vp.sendFrameReadyEvents)
			{
				isSendingFrameReadyEvents.Value = true;
				if (_isSendingFrameReadyEvents != 1)
				{
					Fsm.Event (isSendingFrameReadyEventsEvent);
				}
				_isSendingFrameReadyEvents = 1;
			} else
			{
				isSendingFrameReadyEvents.Value = false;
				if (_isSendingFrameReadyEvents != 0)
				{
					Fsm.Event (isNotSendingFrameReadyEventsEvent);
				}
				_isSendingFrameReadyEvents = 0;
			}
		}
			
		void GetVideoPlayer()
		{
			go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go != null)
			{
				_vp = go.GetComponent<VideoPlayer>();
			}
		}
	}
}

#endif