// (c) Copyright HutongGames, LLC 2010-2018. All rights reserved.

#if UNITY_5_6_OR_NEWER

using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get the size in pixels of a videoClip")]
	public class VideoClipGetSize : FsmStateAction
	{
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Or The video clip of the VideoPlayer. Leave to none, else gameObject is ignored")]
		public FsmObject orVideoClip;

		[UIHint(UIHint.Variable)]
		[Tooltip("The width of the VideoClip")]
		public FsmInt width;

		[UIHint(UIHint.Variable)]
		[Tooltip("The height of the VideoClip")]
		public FsmInt height;

		[UIHint(UIHint.Variable)]
		[Tooltip("The width and height of the VideoClip")]
		public FsmVector2 size;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		GameObject go;

		VideoPlayer _vp;
		VideoClip _vc;


		public override void Reset()
		{
			gameObject = null;
			orVideoClip = new FsmObject() {UseVariable=true};

			width = null;
			height = null;
			size = null;

			everyFrame = false;
		}

		public override void OnEnter()
		{
			GetVideoClip ();

			ExecuteAction ();

			if (!everyFrame)
			{
				Finish ();
			}
		}

		public override void OnUpdate()
		{
			GetVideoClip ();

			ExecuteAction ();
		}

		void ExecuteAction()
		{
			if (_vc != null)
			{
				if (! width.IsNone)
				{
					width.Value = (int)_vc.width;
				}

				if (! width.IsNone)
				{
					height.Value = (int)_vc.height;
				}

				if (! size.IsNone)
				{
					size.Value = new Vector2((int)_vc.width,(int)_vc.height);
				}

				return;
			}
		}

		void GetVideoClip()
		{
			if (orVideoClip.IsNone)
			{
				go = Fsm.GetOwnerDefaultTarget (gameObject);
				if (go != null)
				{
					_vp = go.GetComponent<VideoPlayer> ();
					if (_vp != null)
					{
						_vc = _vp.clip;
					}
				}
			} else
			{
				_vc = orVideoClip.Value as VideoClip;
			}

		}
	}
}

#endif