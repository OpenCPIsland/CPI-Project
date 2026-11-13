// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the playback speed can be changed on a VideoPlayer. (Read Only)")]
	public class VideoPlayerGetCanSetPlaybackSpeed : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetPlaybackSpeed;

		[Tooltip("Event sent if PlaybackSpeed can be set")]
		public FsmEvent canSetTimePlaybackSpeed;

		[Tooltip("Event sent if PlaybackSpeed can not be set")]
		public FsmEvent canNotSetTimePlaybackSpeed;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			canSetPlaybackSpeed = null;
			canSetTimePlaybackSpeed = null;
			canNotSetTimePlaybackSpeed = null;
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
				canSetPlaybackSpeed.Value = _vp.canSetPlaybackSpeed;
				Fsm.Event(_vp.canSetTime?canSetTimePlaybackSpeed:canNotSetTimePlaybackSpeed);
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