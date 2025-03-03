using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class LookAheadFramer : Framer
	{
		public float MaxSpeed = 1f;

		public float Multiplier = 1f;

		public Vector3 Offset;

		public Vector3 OffsetWithKeyboard = new Vector3(0f, -2.5f, 0f);

		public float OffsetBlendTime = 0f;

		public AnimationCurve OffsetBlendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		private float blendT;

		private Vector3 desiredOffset;

		private Vector3 startOffset;

		private Vector3 curOffset;

		public void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden);
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
			if (SceneRefs.IsSet<IScreenContainerStateHandler>())
			{
				IScreenContainerStateHandler screenContainerStateHandler = SceneRefs.Get<IScreenContainerStateHandler>();
				if (screenContainerStateHandler.IsKeyboardShown)
				{
					setOffset(ref OffsetWithKeyboard);
				}
				else
				{
					setOffset(ref Offset);
				}
			}
			else
			{
				setOffset(ref Offset);
			}
		}

		public void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden);
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
		}

		private void setOffset(ref Vector3 offset)
		{
			blendT = 1f;
			desiredOffset = offset;
			curOffset = offset;
			startOffset = offset;
		}

		private void changeOffset(ref Vector3 newOffset)
		{
			blendT = 0f;
			startOffset = curOffset;
			desiredOffset = newOffset;
		}

		private bool onKeyboardShown(KeyboardEvents.KeyboardShown evt)
		{
			changeOffset(ref OffsetWithKeyboard);
			return false;
		}

		private bool onKeyboardHidden(KeyboardEvents.KeyboardHidden evt)
		{
			changeOffset(ref Offset);
			return false;
		}

		public override void Aim(ref Setup setup)
		{
			if (curOffset != desiredOffset)
			{
				if (OffsetBlendTime > 0f)
				{
					blendT += Time.deltaTime / OffsetBlendTime;
				}
				else
				{
					blendT = 1f;
				}
				float t = OffsetBlendCurve.Evaluate(blendT);
				curOffset = Vector3.Lerp(startOffset, desiredOffset, t);
			}
			Vector3 b = Vector3.ClampMagnitude(setup.FocusVelocity, MaxSpeed) * Multiplier + curOffset;
			setup.LookAt = setup.Focus.position + b;
		}
	}
}
