// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Send the loopPointReached event from a VideoPlayer.")]
	public class VideoPlayerLoopPointReachedEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("event invoked when the player reaches the end of the content to play.")]
		public FsmEvent OnLoopPointReachedEvent;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			OnLoopPointReachedEvent = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			if (_vp != null)
			{
				_vp.loopPointReached += OnLoopPointReached;
			}
		}

		public override void OnExit()
		{
			if (_vp != null)
			{
				_vp.loopPointReached -= OnLoopPointReached;
			}
		}

		void OnLoopPointReached(VideoPlayer source)
		{
			Fsm.EventData.GameObjectData = source.gameObject;
			Fsm.Event (OnLoopPointReachedEvent);
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