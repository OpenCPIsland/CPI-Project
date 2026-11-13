// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether it's possible to set if the player can skips frames to catch up with current time. (Read Only)")]
	public class VideoPlayerGetCanSkipOnDrop : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetSkipOnDrop;

		[Tooltip("Event sent if SkipOnDrop can be set")]
		public FsmEvent canSetSkipOnDropEvent;

		[Tooltip("Event sent if SkipOnDrop can not be set")]
		public FsmEvent canNotSetSkipOnDropEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		int _canSetSkipOnDrop = -1;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			canSetSkipOnDrop = null;
			canSetSkipOnDropEvent = null;
			canNotSetSkipOnDropEvent = null;
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
			if (_vp.canSetSkipOnDrop)
			{
				canSetSkipOnDrop.Value = true;
				if (_canSetSkipOnDrop != 1)
				{
					Fsm.Event (canSetSkipOnDropEvent);
				}
				_canSetSkipOnDrop = 1;
			} else
			{
				canSetSkipOnDrop.Value = false;
				if (_canSetSkipOnDrop != 0)
				{
					Fsm.Event (canNotSetSkipOnDropEvent);
				}
				_canSetSkipOnDrop = 0;
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