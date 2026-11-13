using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class FixedOffsetFramer : Framer
	{
		public Vector3 Offset;

		public float KeyboardYOffset = -1.3f;

		public bool Local;

		private bool isShowingKeyboard;

		public void OnEnable()
		{
			if (SceneRefs.IsSet<IScreenContainerStateHandler>())
			{
				isShowingKeyboard = SceneRefs.Get<IScreenContainerStateHandler>().IsKeyboardShown;
			}
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden);
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
		}

		public void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden);
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
		}

		private bool onKeyboardShown(KeyboardEvents.KeyboardShown evt)
		{
			isShowingKeyboard = true;
			return false;
		}

		private bool onKeyboardHidden(KeyboardEvents.KeyboardHidden evt)
		{
			isShowingKeyboard = false;
			return false;
		}

		public override void Aim(ref Setup setup)
		{
			setup.LookAt = (Local ? setup.Focus.TransformPoint(Offset) : (setup.Focus.position + Offset));
			if (isShowingKeyboard)
			{
				setup.LookAt += new Vector3(0f, KeyboardYOffset, 0f);
			}
		}
	}
}
