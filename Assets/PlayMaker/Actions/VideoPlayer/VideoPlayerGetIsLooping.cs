// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the player restarts from the beginning without when it reaches the end of the clip.")]
	public class VideoPlayerGetIsLooping : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isLooping;

		[Tooltip("Event sent if content is looping")]
		public FsmEvent isLoopingEvent;

		[Tooltip("Event sent if content is not looping")]
		public FsmEvent isNotLoopingEvent;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		GameObject go;

		VideoPlayer _vp;

		int _isLooping = -1;

		public override void Reset()
		{
			gameObject = null;
			isLooping = null;
			isLoopingEvent = null;
			isNotLoopingEvent = null;
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

			if (_vp.isLooping)
			{
				isLooping.Value = true;
				if (_isLooping != 1)
				{
					Fsm.Event (isLoopingEvent);
				}
				_isLooping = 1;
			} else
			{
				isLooping.Value = false;
				if (_isLooping != 0)
				{
					Fsm.Event (isNotLoopingEvent);
				}
				_isLooping = 0;
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