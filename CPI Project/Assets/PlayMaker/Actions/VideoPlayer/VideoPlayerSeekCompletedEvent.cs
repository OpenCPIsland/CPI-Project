// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send event from a VideoPlayer after a seek operation completes..")]
	public class VideoPlayerSeekCompletedEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("event invoked when the player preparation is complete.")]
		public FsmEvent OnSeekCompletedEvent;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			OnSeekCompletedEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			if (_vp != null)
			{
				_vp.seekCompleted += OnSeekCompleted;
			}
		}

		public override void OnExit()
		{
			if (_vp != null)
			{
				_vp.seekCompleted -= OnSeekCompleted;
			}
		}

		void OnSeekCompleted(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			Fsm.Event (OnSeekCompletedEvent);
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