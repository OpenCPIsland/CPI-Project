// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set Whether the content will start playing back as soon as the component awakes.")]
	public class VideoPlayerSetPlayOnAwake : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Value")]
		public FsmBool playOnAwake;

		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		GameObject go;

		VideoPlayer _vp;

		public override void Reset()
		{
			gameObject = null;
			playOnAwake = null;
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

			_vp.playOnAwake = playOnAwake.Value;

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