using System;
using UnityEngine;
using UnityEngine.UI;

public class UITransitionGrayOut : MonoBehaviour
{
	public Image target;

	public float startAlpha = 0f;

	public float endAlpha = 0f;

	public float duration = 1f;

	public bool PlayOnStart = true;

	public AnimationCurve InterpCurveForward = null;

	public AnimationCurve InterpCurveBackward = null;

	public float MaxDeltaTime = 71f / (678f * (float)Math.PI);

	private bool InterpForward = true;

	private bool Interping = false;

	private float InterpT = 0f;

	private MonoBehaviour CallbackScript = null;

	private string CallbackMethodName = "OnTransitionEnd";

	private void Start()
	{
		Color color = target.color;
		color.a = startAlpha;
		target.color = color;
		if (PlayOnStart)
		{
			Play();
		}
	}

	private void Update()
	{
		if (!Interping)
		{
			return;
		}
		InterpT = Mathf.Clamp(InterpT + Mathf.Min(Time.deltaTime, MaxDeltaTime), 0f, duration);
		float num = 0f;
		if (InterpT == duration)
		{
			num = (InterpForward ? endAlpha : startAlpha);
			Interping = false;
			if (CallbackScript != null)
			{
				CallbackScript.SendMessage(CallbackMethodName, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			float num2 = InterpForward ? startAlpha : endAlpha;
			float num3 = InterpForward ? endAlpha : startAlpha;
			AnimationCurve animationCurve = InterpForward ? InterpCurveForward : InterpCurveBackward;
			num = num2 + (num3 - num2) * animationCurve.Evaluate(InterpT / duration);
		}
		Color color = target.color;
		color.a = num;
		target.color = color;
	}

	public void Play(bool forward = true, MonoBehaviour callBackScript = null, string callbackMethodName = "OnTransitionEnd")
	{
		Interping = true;
		InterpForward = forward;
		InterpT = 0f;
		Color color = target.color;
		color.a = (InterpForward ? startAlpha : endAlpha);
		target.color = color;
		CallbackScript = callBackScript;
		CallbackMethodName = callbackMethodName;
	}
}
