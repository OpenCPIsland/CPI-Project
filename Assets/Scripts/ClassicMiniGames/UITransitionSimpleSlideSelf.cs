using System;
using UnityEngine;

public class UITransitionSimpleSlideSelf : MonoBehaviour
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

	private MonoBehaviour CallbackScript = null;

	private string CallbackMethodName = "OnTransitionEnd";

	private void Start()
	{
		if (PlayOnStart)
		{
			StartPos.z = base.transform.localPosition.z;
			EndPos.z = base.transform.localPosition.z;
			base.transform.localPosition = StartPos;
			Play();
		}
		else
		{
			base.transform.localPosition = StartPos;
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
			base.transform.localPosition = (InterpForward ? EndPos : StartPos);
			Interping = false;
			if (CallbackScript != null)
			{
				CallbackScript.SendMessage(CallbackMethodName, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Vector3 vector = InterpForward ? StartPos : EndPos;
			Vector3 a = InterpForward ? EndPos : StartPos;
			AnimationCurve animationCurve = InterpForward ? InterpCurveForward : InterpCurveBackward;
			base.transform.localPosition = vector + (a - vector) * animationCurve.Evaluate(InterpT / Duration);
		}
	}

	public void Play(bool forward = true, MonoBehaviour callBackScript = null, string callbackMethodName = "OnTransitionEnd")
	{
		Interping = true;
		InterpForward = forward;
		InterpT = 0f;
		base.transform.localPosition = (InterpForward ? StartPos : EndPos);
		CallbackScript = callBackScript;
		CallbackMethodName = callbackMethodName;
	}
}
