using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	public static UIButton current;

	public bool dragHighlight = false;

	public string hoverSprite;

	public string pressedSprite;

	public string disabledSprite;

	public bool pixelSnap = false;

	public List<EventDelegate> onClick = new List<EventDelegate>();

	[NonSerialized]
	private string mNormalSprite;

	[NonSerialized]
	private UISprite mSprite;

	public override bool isEnabled
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			Collider component = GetComponent<Collider>();
			if ((bool)component && component.enabled)
			{
				return true;
			}
			Collider2D component2 = GetComponent<Collider2D>();
			return (bool)component2 && component2.enabled;
		}
		set
		{
			if (isEnabled == value)
			{
				return;
			}
			Collider component = GetComponent<Collider>();
			if (component != null)
			{
				component.enabled = value;
				SetState((!value) ? State.Disabled : State.Normal, false);
				return;
			}
			Collider2D component2 = GetComponent<Collider2D>();
			if (component2 != null)
			{
				component2.enabled = value;
				SetState((!value) ? State.Disabled : State.Normal, false);
			}
			else
			{
				base.enabled = value;
			}
		}
	}

	public string normalSprite
	{
		get
		{
			if (!mInitDone)
			{
				OnInit();
			}
			return mNormalSprite;
		}
		set
		{
			if (mSprite != null && !string.IsNullOrEmpty(mNormalSprite) && mNormalSprite == mSprite.spriteName)
			{
				mNormalSprite = value;
				SetSprite(value);
				NGUITools.SetDirty(mSprite);
				return;
			}
			mNormalSprite = value;
			if (mState == State.Normal)
			{
				SetSprite(value);
			}
		}
	}

	protected override void OnInit()
	{
		base.OnInit();
		mSprite = (mWidget as UISprite);
		if (mSprite != null)
		{
			mNormalSprite = mSprite.spriteName;
		}
	}

	protected override void OnEnable()
	{
		if (isEnabled)
		{
			if (mInitDone)
			{
				if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
				{
					OnHover(UICamera.selectedObject == base.gameObject);
				}
				else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse)
				{
					OnHover(UICamera.hoveredObject == base.gameObject);
				}
				else
				{
					SetState(State.Normal, false);
				}
			}
		}
		else
		{
			SetState(State.Disabled, true);
		}
	}

	protected override void OnDragOver()
	{
		if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == base.gameObject))
		{
			base.OnDragOver();
		}
	}

	protected override void OnDragOut()
	{
		if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == base.gameObject))
		{
			base.OnDragOut();
		}
	}

	protected virtual void OnClick()
	{
		if (current == null && isEnabled)
		{
			current = this;
			EventDelegate.Execute(onClick);
			current = null;
		}
	}

	public override void SetState(State state, bool immediate)
	{
		base.SetState(state, immediate);
		switch (state)
		{
		case State.Normal:
			SetSprite(mNormalSprite);
			break;
		case State.Hover:
			SetSprite(hoverSprite);
			break;
		case State.Pressed:
			SetSprite(pressedSprite);
			break;
		case State.Disabled:
			SetSprite(disabledSprite);
			break;
		}
	}

	protected void SetSprite(string sp)
	{
		if (mSprite != null && !string.IsNullOrEmpty(sp) && mSprite.spriteName != sp)
		{
			mSprite.spriteName = sp;
			if (pixelSnap)
			{
				mSprite.MakePixelPerfect();
			}
		}
	}
}
