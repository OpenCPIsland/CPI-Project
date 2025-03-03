// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether the time source followed by the video player can be changed. (Read Only)")]
	public class VideoPlayerGetCanSetTimeSource : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetTimeSource;

		[Tooltip("Event sent if timeSource can be set")]
		public FsmEvent canSetTimeSourceEvent;

		[Tooltip("Event sent if timeSource can not be set")]
		public FsmEvent canNotSetTimeSourceEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			canSetTimeSource = null;
			canSetTimeSourceEvent = null;
			canNotSetTimeSourceEvent = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			ExecuteAction ();

			if (!everyFrame)
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
			if (_vp != null)
			{
#if UNITY_2022_2_OR_NEWER
				canSetTimeSource.Value = _vp.canSetTimeUpdateMode;
#else	
				canSetTimeSource.Value = _vp.canSetTimeSource;
#endif
				Fsm.Event(_vp.canSetTime?canSetTimeSourceEvent:canNotSetTimeSourceEvent);
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
