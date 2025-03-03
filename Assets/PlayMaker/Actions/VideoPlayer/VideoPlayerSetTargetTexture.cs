// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set the RenderTexture to draw to when VideoPlayer.renderMode is set to Video.VideoTarget.RenderTexture.")]
	public class VideoPlayerSetTargetTexture : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The RenderTexture")]
		public FsmTexture targetTexture;


		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
			targetTexture = null;
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
				_vp.targetTexture = (RenderTexture)targetTexture.Value;
				return;
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