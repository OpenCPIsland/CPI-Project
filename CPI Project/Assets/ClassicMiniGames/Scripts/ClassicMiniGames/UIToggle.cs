using AnimationOrTween;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Toggle")]
public class UIToggle : UIWidgetContainer
{
	public static BetterList<UIToggle> list = new BetterList<UIToggle>();

	public static UIToggle current;

	public int group = 0;

	public UIWidget activeSprite;

	public Animation activeAnimation;

	public bool startsActive = false;

	public bool instantTween = false;

	public bool optionCanBeNone = false;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[HideInInspector]
	[SerializeField]
	private UISprite checkSprite = null;

	[SerializeField]
	[HideInInspector]
	private Animation checkAnimation;

	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	[SerializeField]
	[HideInInspector]
	private string functionName = "OnActivate";

	[SerializeField]
	[HideInInspector]
	private bool startsChecked = false;

	private bool mIsActive = true;

	private bool mStarted = false;

	public bool value
	{
		get
		{
			return mIsActive;
		}
		set
		{
			if (group == 0 || value || optionCanBeNone || !mStarted)
			{
				Set(value);
			}
		}
	}

	[Obsolete("Use 'value' instead")]
	public bool isChecked
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public static UIToggle GetActiveToggle(int group)
	{
		for (int i = 0; i < list.size; i++)
		{
			UIToggle uIToggle = list[i];
			if (uIToggle != null && uIToggle.group == group && uIToggle.mIsActive)
			{
				return uIToggle;
			}
		}
		return null;
	}

	private void OnEnable()
	{
		list.Add(this);
	}

	private void OnDisable()
	{
		list.Remove(this);
	}

	private void Start()
	{
		if (startsChecked)
		{
			startsChecked = false;
			startsActive = true;
		}
		if (!Application.isPlaying)
		{
			if (checkSprite != null && activeSprite == null)
			{
				activeSprite = checkSprite;
				checkSprite = null;
			}
			if (checkAnimation != null && activeAnimation == null)
			{
				activeAnimation = checkAnimation;
				checkAnimation = null;
			}
			if (Application.isPlaying && activeSprite != null)
			{
				activeSprite.alpha = (startsActive ? 1f : 0f);
			}
			if (EventDelegate.IsValid(onChange))
			{
				eventReceiver = null;
				functionName = null;
			}
		}
		else
		{
			mIsActive = !startsActive;
			mStarted = true;
			bool flag = instantTween;
			instantTween = true;
			Set(startsActive);
			instantTween = flag;
		}
	}

	private void OnClick()
	{
		if (base.enabled)
		{
			value = !value;
		}
	}

	private void Set(bool state)
	{
		if (!mStarted)
		{
			mIsActive = state;
			startsActive = state;
			if (activeSprite != null)
			{
				activeSprite.alpha = (state ? 1f : 0f);
			}
		}
		else
		{
			if (mIsActive == state)
			{
				return;
			}
			if (group != 0 && state)
			{
				int num = 0;
				int size = list.size;
				while (num < size)
				{
					UIToggle uIToggle = list[num];
					if (uIToggle != this && uIToggle.group == group)
					{
						uIToggle.Set(false);
					}
					if (list.size != size)
					{
						size = list.size;
						num = 0;
					}
					else
					{
						num++;
					}
				}
			}
			mIsActive = state;
			if (activeSprite != null)
			{
				if (instantTween)
				{
					activeSprite.alpha = (mIsActive ? 1f : 0f);
				}
				else
				{
					TweenAlpha.Begin(activeSprite.gameObject, 0.15f, mIsActive ? 1f : 0f);
				}
			}
			if (current == null)
			{
				current = this;
				if (EventDelegate.IsValid(onChange))
				{
					EventDelegate.Execute(onChange);
				}
				else if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
				{
					eventReceiver.SendMessage(functionName, mIsActive, SendMessageOptions.DontRequireReceiver);
				}
				current = null;
			}
			if (this.activeAnimation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(this.activeAnimation, state ? Direction.Forward : Direction.Reverse);
				if (instantTween)
				{
					activeAnimation.Finish();
				}
			}
		}
	}
}
