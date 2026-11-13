// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the framedropped event from a VideoPlayer when playback detects it does not keep up with the time source..")]
	public class VideoPlayerFrameDroppedEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("event sent when playback detects it does not keep up with the time source.")]
		public FsmEvent onFrameDroppedEvent;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			onFrameDroppedEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			if (_vp != null)
			{
				_vp.frameDropped += OnFrameDropped;
			}
		}

		public override void OnExit()
		{
			if (_vp != null)
			{
				_vp.frameDropped -= OnFrameDropped;
			}
		}

		void OnFrameDropped(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			Fsm.Event (onFrameDroppedEvent);
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