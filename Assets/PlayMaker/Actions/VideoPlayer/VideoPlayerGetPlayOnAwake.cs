// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the content will start playing back as soon as the component awakes.")]
	public class VideoPlayerGetPlayOnAwake : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPlayingOnAwake;

		[Tooltip("Event sent if content content will start playing back as soon as the component awakes")]
		public FsmEvent isPlayingOnAwakeEvent;

		[Tooltip("Event sent if content will not start playing back as soon as the component awakes")]
		public FsmEvent isNotPlayingOnAwakeEvent;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		GameObject go;

		VideoPlayer _vp;

		int _isPlayingOnAwake = -1;

		public override void Reset()
		{
			gameObject = null;
			isPlayingOnAwake = null;
			isPlayingOnAwakeEvent = null;
			isNotPlayingOnAwakeEvent = null;
			everyframe = false;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			ExecuteAction ();

			if (!everyframe)
			{
				Finish ();
			}
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

			if (_vp.playOnAwake)
			{
				isPlayingOnAwake.Value = true;
				if (_isPlayingOnAwake != 1)
				{
					Fsm.Event (isPlayingOnAwakeEvent);
				}
				_isPlayingOnAwake = 1;
			} else
			{
				isPlayingOnAwake.Value = false;
				if (_isPlayingOnAwake != 0)
				{
					Fsm.Event (isNotPlayingOnAwakeEvent);
				}
				_isPlayingOnAwake = 0;
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