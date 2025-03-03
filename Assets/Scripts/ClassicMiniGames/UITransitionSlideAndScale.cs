using DisneyMobile.CoreUnitySystems;
using DisneyMobile.CoreUnitySystems.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionSlideAndScale : UIElementBase, IScreenTransition
{
	public Vector3 StartPos = Vector3.zero;

	public Vector3 EndPos = Vector3.zero;

	public AnimationCurve InterpCurveForward = null;

	public AnimationCurve InterpCurveBackward = null;

	public float Duration = 1f;

	public bool PlayOnStart = true;

	public float MaxDeltaTime = 71f / (678f * (float)Math.PI);

	private bool InterpForward = true;

	private bool Interping = false;

	private float InterpT = 0f;

	private Vector3 startScale = new Vector3(0.01f, 0.01f, 0.01f);

	private Vector3 endScale = Vector3.one;

	private string CallbackMethodName = "OnTransitionEnd";

	private void Start()
	{
		if (PlayOnStart)
		{
			StartPos.z = base.transform.parent.localPosition.z;
			EndPos.z = base.transform.parent.localPosition.z;
			base.transform.parent.localPosition = StartPos;
			Play();
		}
		else
		{
			base.transform.parent.localPosition = StartPos;
			base.transform.parent.localScale = startScale;
		}
	}

	private void Update()
	{
		if (Interping)
		{
			InterpT = Mathf.Clamp(InterpT + Mathf.Min(Time.deltaTime, MaxDeltaTime), 0f, Duration);
			if (InterpT == Duration)
			{
				base.transform.parent.localPosition = (InterpForward ? EndPos : StartPos);
				base.transform.parent.localScale = (InterpForward ? endScale : startScale);
				Interping = false;
				base.Controller.SendMessage(CallbackMethodName, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Vector3 vector = InterpForward ? StartPos : EndPos;
				Vector3 a = InterpForward ? EndPos : StartPos;
				AnimationCurve animationCurve = InterpForward ? InterpCurveForward : InterpCurveBackward;
				base.transform.parent.localPosition = vector + (a - vector) * animationCurve.Evaluate(InterpT / Duration);
				vector = (InterpForward ? startScale : endScale);
				a = (InterpForward ? endScale : startScale);
				base.transform.parent.localScale = vector + (a - vector) * animationCurve.Evaluate(InterpT / Duration);
			}
		}
	}

	public void Play(bool forward = true, string callbackMethodName = "OnTransitionEnd")
	{
		Interping = true;
		InterpForward = forward;
		InterpT = 0f;
		base.transform.parent.localPosition = (InterpForward ? EndPos : StartPos);
		CallbackMethodName = callbackMethodName;
	}

	public void PlayInstant(bool forward = true)
	{
		Interping = false;
		base.transform.parent.localPosition = (forward ? StartPos : EndPos);
	}

	public override Dictionary<string, string> WriteAttributesToDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("StartPos", Utilities.formatVector3(StartPos));
		dictionary.Add("EndPos", Utilities.formatVector3(EndPos));
		dictionary.Add("name", base.gameObject.name);
		dictionary.Add("PlayOnStart", PlayOnStart ? "1" : "0");
		return dictionary;
	}

	public override void ReadAttributesFromDictionary(Dictionary<string, string> attributes)
	{
		StartPos = Utilities.stringToVector3(attributes["StartPos"]);
		EndPos = Utilities.stringToVector3(attributes["EndPos"]);
		base.gameObject.name = attributes["name"];
		if (attributes.ContainsKey("PlayOnStart"))
		{
			PlayOnStart = ((attributes["PlayOnStart"] == "1") ? true : false);
		}
	}
}
