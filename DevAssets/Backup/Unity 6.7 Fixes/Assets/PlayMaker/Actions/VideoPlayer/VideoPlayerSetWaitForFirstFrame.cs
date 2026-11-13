// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set whether the player will wait for the first frame to be loaded into the texture before starting playback when VideoPlayer.playOnAwake is on")]
	public class VideoPlayerSetWaitForFirstFrame : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool waitForFirstFrame;

		GameObject go;

		VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			waitForFirstFrame = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			ExecuteAction ();

			Finish ();

		}

		void ExecuteAction()
		{
			if (_vp == null)
			{
				return;
			}

			_vp.waitForFirstFrame = waitForFirstFrame.Value;
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