// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Defines how the video content will be stretched to fill the target area.")]
	public class VideoPlayerSetAspectRatio : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The AspectRatio type")]
		[ObjectType(typeof(VideoAspectRatio))]
		public FsmEnum aspectRatio;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			aspectRatio = VideoAspectRatio.NoScaling;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			ExecuteAction ();
		
			Finish ();
		}


		void ExecuteAction()
		{
			if (_vp != null)
			{
				_vp.aspectRatio = (VideoAspectRatio)aspectRatio.Value;
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