// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set The Camera GameObject to draw to when VideoPlayer.renderMode is set to either Video.VideoTarget.CameraBackPlane or Video.VideoTarget.CameraFrontPlane.")]
	public class VideoPlayerSetTargetCamera : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The Camera GameObject")]
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject targetCamera;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		GameObject go;

		VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			targetCamera = null;
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
				if (targetCamera.Value!=null)
				{
					_vp.targetCamera = targetCamera.Value.GetComponent<Camera> ();
				}
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