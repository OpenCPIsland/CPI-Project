using Disney.Kelowna.Common;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldEx : InputField
{
	[Tooltip("This is the TouchMonitor instance used with ScrollTouchThreshold")]
	public TouchMonitor TouchMonitor;

	private bool nothingEnteredYet = true;

	private bool justSelected = false;

	private bool _isSelected;

	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		private set
		{
			justSelected = value;
			_isSelected = value;
		}
	}

	public event Action<InputFieldEx> OnKeyboardDone;

	public event Action<string> onDeselected;

	protected override void Start()
	{
		TouchMonitor = GetComponentInParent<TouchMonitor>();
	}

	private void Update()
	{
		if (Input.anyKeyDown || base.text.Length > 0)
		{
			nothingEnteredYet = false;
		}
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			onKeyboardDone();
		}
		justSelected = nothingEnteredYet;
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		if (!nothingEnteredYet)
		{
			IsSelected = false;
			if (this.onDeselected != null)
			{
				this.onDeselected(base.text);
			}
			base.OnDeselect(eventData);
		}
	}

	public override void OnSelect(BaseEventData eventData)
	{
		if (TouchMonitor != null)
		{
			if (!TouchMonitor.IsSwiping)
			{
				IsSelected = true;
				base.OnSelect(eventData);
			}
		}
		else
		{
			IsSelected = true;
			base.OnSelect(eventData);
		}
	}

	public void TriggerOnKeyboardDone()
	{
		this.OnKeyboardDone.InvokeSafe(this);
	}

	private void onKeyboardDone()
	{
		if (IsSelected && !justSelected)
		{
			this.OnKeyboardDone.InvokeSafe(this);
		}
	}
}
