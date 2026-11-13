using System;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/NGUI Slider")]
public class UISlider : UIProgressBar
{
	private enum Direction
	{
		Horizontal,
		Vertical,
		Upgraded
	}

	[HideInInspector]
	[SerializeField]
	private Transform foreground = null;

	[SerializeField]
	[HideInInspector]
	private float rawValue = 1f;

	[HideInInspector]
	[SerializeField]
	private Direction direction = Direction.Upgraded;

	[HideInInspector]
	[SerializeField]
	protected bool mInverted = false;

	[Obsolete("Use 'value' instead")]
	public float sliderValue
	{
		get
		{
			return base.value;
		}
		set
		{
			base.value = value;
		}
	}

	[Obsolete("Use 'fillDirection' instead")]
	public bool inverted
	{
		get
		{
			return base.isInverted;
		}
		set
		{
		}
	}

	protected override void Upgrade()
	{
		if (direction != Direction.Upgraded)
		{
			mValue = rawValue;
			if (foreground != null)
			{
				mFG = foreground.GetComponent<UIWidget>();
			}
			if (direction == Direction.Horizontal)
			{
				mFill = (mInverted ? FillDirection.RightToLeft : FillDirection.LeftToRight);
			}
			else
			{
				mFill = (mInverted ? FillDirection.TopToBottom : FillDirection.BottomToTop);
			}
			direction = Direction.Upgraded;
		}
	}

	protected override void OnStart()
	{
		GameObject go = (mBG != null && mBG.GetComponent<Collider>() != null) ? mBG.gameObject : base.gameObject;
		UIEventListener uIEventListener = UIEventListener.Get(go);
		uIEventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener.onPress, new UIEventListener.BoolDelegate(OnPressBackground));
		uIEventListener.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener.onDrag, new UIEventListener.VectorDelegate(OnDragBackground));
		if (thumb != null && thumb.GetComponent<Collider>() != null && (mFG == null || thumb != mFG.cachedTransform))
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(thumb.gameObject);
			uIEventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener2.onPress, new UIEventListener.BoolDelegate(OnPressForeground));
			uIEventListener2.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener2.onDrag, new UIEventListener.VectorDelegate(OnDragForeground));
		}
	}

	protected void OnPressBackground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = ScreenToValue(UICamera.lastTouchPosition);
			if (!isPressed && onDragFinished != null)
			{
				onDragFinished();
			}
		}
	}

	protected void OnDragBackground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = ScreenToValue(UICamera.lastTouchPosition);
		}
	}

	protected void OnPressForeground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			if (isPressed)
			{
				mOffset = ((mFG == null) ? 0f : (base.value - ScreenToValue(UICamera.lastTouchPosition)));
			}
			else if (onDragFinished != null)
			{
				onDragFinished();
			}
		}
	}

	protected void OnDragForeground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = mOffset + ScreenToValue(UICamera.lastTouchPosition);
		}
	}

	protected void OnKey(KeyCode key)
	{
		if (!base.enabled)
		{
			return;
		}
		float num = ((float)numberOfSteps > 1f) ? (1f / (float)(numberOfSteps - 1)) : 0.125f;

		// Use the new Input System for arrow key handling
		bool left = Keyboard.current != null && Keyboard.current.leftArrowKey.wasPressedThisFrame;
		bool right = Keyboard.current != null && Keyboard.current.rightArrowKey.wasPressedThisFrame;
		bool up = Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame;
		bool down = Keyboard.current != null && Keyboard.current.downArrowKey.wasPressedThisFrame;

		if (base.fillDirection == FillDirection.LeftToRight || base.fillDirection == FillDirection.RightToLeft)
		{
			if (key == KeyCode.LeftArrow || left)
			{
				base.value = mValue - num;
			}
			else if (key == KeyCode.RightArrow || right)
			{
				base.value = mValue + num;
			}
		}
		else
		{
			if (key == KeyCode.DownArrow || down)
			{
				base.value = mValue - num;
			}
			else if (key == KeyCode.UpArrow || up)
			{
				base.value = mValue + num;
			}
		}
	}
}