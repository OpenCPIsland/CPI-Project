// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the frameReady event from a VideoPlayer when a new frame is ready.")]
	public class VideoPlayerFrameReadyEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("event sent when a new frame is ready.")]
		public FsmEvent onFrameReadyEvent;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			onFrameReadyEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			if (_vp != null)
			{
				_vp.frameReady += OnFrameReady;
			}
		}

		public override void OnExit()
		{
			if (_vp != null)
			{
				_vp.frameReady -= OnFrameReady;
			}
		}

		void OnFrameReady(VideoPlayer source, long frameIndex)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			Fsm.EventData.IntData = (int)frameIndex;
			Fsm.Event (onFrameReadyEvent);
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