// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether content is being played. (Read Only)")]
	public class VideoPlayerGetIsPlaying : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPlaying;

		[Tooltip("Event sent if content is playing")]
		public FsmEvent isPlayingEvent;

		[Tooltip("Event sent if content is not playing")]
		public FsmEvent isNotPlayingEvent;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		GameObject go;

		VideoPlayer _vp;

		int _isPlaying = -1;

		public override void Reset()
		{
			gameObject = null;
			isPlaying = null;
			isPlayingEvent = null;
			isNotPlayingEvent = null;
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

			if (_vp.isPlaying)
			{
				isPlaying.Value = true;
				if (_isPlaying != 1)
				{
					Fsm.Event (isPlayingEvent);
				}
				_isPlaying = 1;
			} else
			{
				isPlaying.Value = false;
				if (_isPlaying != 0)
				{
					Fsm.Event (isNotPlayingEvent);
				}
				_isPlaying = 0;
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