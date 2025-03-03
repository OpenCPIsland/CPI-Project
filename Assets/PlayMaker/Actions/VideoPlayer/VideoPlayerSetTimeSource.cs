// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Sets Time source followed by the VideoPlayer when reading content.")]
	public class VideoPlayerSetTimeSource : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The timeSource Value")]
#if UNITY_2022_2_OR_NEWER
		[ObjectType(typeof(VideoTimeUpdateMode))]
#else
		[ObjectType(typeof(VideoTimeSource))]
#endif
		public FsmEnum timeSource;

		[Tooltip("Event sent if time can not be set")]
		public FsmEvent canNotSetTime;


		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
#if UNITY_2022_2_OR_NEWER
			timeSource = VideoTimeUpdateMode.DSPTime;
#else
			timeSource = VideoTimeSource.AudioDSPTimeSource;
#endif
			canNotSetTime = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			if (_vp != null && !_vp.canSetTime)
			{
				Fsm.Event (canNotSetTime);
			} else
			{
				ExecuteAction ();
			}

			Finish ();

		}

		void ExecuteAction()
		{
			if (_vp != null && _vp.canSetTime)
			{
#if UNITY_2022_2_OR_NEWER
				_vp.timeUpdateMode = (VideoTimeUpdateMode)timeSource.Value;
#else
				_vp.timeSource = (VideoTimeSource)timeSource.Value;
#endif				
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
