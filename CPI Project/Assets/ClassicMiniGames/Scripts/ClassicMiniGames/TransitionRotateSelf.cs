using System;
using UnityEngine;

public class TransitionRotateSelf : MonoBehaviour
{
	public Vector3 StartRotate = Vector3.zero;

	public Vector3 EndRotate = Vector3.zero;

	public AnimationCurve InterpCurveForward = null;

	public AnimationCurve InterpCurveBackward = null;

	public float Duration = 1f;

	public bool PlayOnStart = true;

	public float MaxDeltaTime = 71f / (678f * (float)Math.PI);

	private bool InterpForward = true;

	private bool Interping = false;

	private float InterpT = 0f;

	private MonoBehaviour CallbackScript = null;

	private string CallbackMethodName = "OnTransitionEnd";

	private void Start()
	{
		if (PlayOnStart)
		{
			base.transform.eulerAngles = StartRotate;
			Play();
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
			base.transform.eulerAngles = (InterpForward ? EndRotate : StartRotate);
			Interping = false;
			if (CallbackScript != null)
			{
				CallbackScript.SendMessage(CallbackMethodName, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Vector3 vector = InterpForward ? StartRotate : EndRotate;
			Vector3 a = InterpForward ? EndRotate : StartRotate;
			AnimationCurve animationCurve = InterpForward ? InterpCurveForward : InterpCurveBackward;
			base.transform.eulerAngles = vector + (a - vector) * animationCurve.Evaluate(InterpT / Duration);
		}
	}

	public void Play(bool forward = true, MonoBehaviour callBackScript = null, string callbackMethodName = "OnTransitionEnd")
	{
		Interping = true;
		InterpForward = forward;
		InterpT = 0f;
		base.transform.eulerAngles = (InterpForward ? StartRotate : EndRotate);
		CallbackScript = callBackScript;
		CallbackMethodName = callbackMethodName;
	}
}
