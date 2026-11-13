// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Starts playback of a VideoPlayer. Initiates the preparation if not already done, in which case the playback will not start instantly.")]
	public class VideoPlayerPlay : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		GameObject go;

		VideoPlayer _vp;


		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			GetVideoPlayer ();

			if (_vp != null)
			{
				_vp.Play();
			}

			Finish();
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