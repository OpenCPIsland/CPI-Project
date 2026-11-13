using DisneyMobile.CoreUnitySystems;
using DisneyMobile.CoreUnitySystems.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionSimpleSlide : UIElementBase, IScreenTransition
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
		}
	}

	private void Update()
	{
		if (!Interping)
		{
			return;
		}
		InterpT = Mathf.Clamp(InterpT + Mathf.Min(Time.deltaTime, MaxDeltaTime), 0f, Duration);
		if (InterpT == Duration)
		{
			base.transform.parent.localPosition = (InterpForward ? EndPos : StartPos);
			Interping = false;
			if (base.Controller != null)
			{
				base.Controller.SendMessage(CallbackMethodName, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Vector3 vector = InterpForward ? StartPos : EndPos;
			Vector3 a = InterpForward ? EndPos : StartPos;
			AnimationCurve animationCurve = InterpForward ? InterpCurveForward : InterpCurveBackward;
			base.transform.parent.localPosition = vector + (a - vector) * animationCurve.Evaluate(InterpT / Duration);
		}
	}

	public void Play(bool forward = true, string callbackMethodName = "OnTransitionEnd")
	{
		Interping = true;
		InterpForward = forward;
		InterpT = 0f;
		StartPos.z = base.transform.parent.localPosition.z;
		EndPos.z = base.transform.parent.localPosition.z;
		base.transform.parent.localPosition = (InterpForward ? StartPos : EndPos);
		CallbackMethodName = callbackMethodName;
	}

	public void PlayInstant(bool forward = true)
	{
		Interping = false;
		base.transform.parent.localPosition = (forward ? EndPos : StartPos);
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
