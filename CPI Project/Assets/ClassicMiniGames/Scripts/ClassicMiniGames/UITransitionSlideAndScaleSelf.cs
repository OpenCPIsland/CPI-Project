using System;
using UnityEngine;

public class UITransitionSlideAndScaleSelf : MonoBehaviour
{
	public Vector3 DeltaPos = Vector3.zero;

	public Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f);

	public Vector3 endScale = Vector3.one;

	private Vector3 StartPos = Vector3.zero;

	private Vector3 EndPos = Vector3.zero;

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
			base.transform.localScale = startScale;
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
			base.transform.localScale = (InterpForward ? endScale : startScale);
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
			vector = (InterpForward ? startScale : endScale);
			a = (InterpForward ? endScale : startScale);
			base.transform.localScale = vector + (a - vector) * animationCurve.Evaluate(InterpT / Duration);
		}
	}

	public void Play(bool forward = true, MonoBehaviour callBackScript = null, string callbackMethodName = "OnTransitionEnd")
	{
		Interping = true;
		InterpForward = forward;
		InterpT = 0f;
		if (InterpForward)
		{
			StartPos = base.transform.localPosition;
			EndPos = base.transform.localPosition + DeltaPos;
		}
		else
		{
			EndPos = base.transform.localPosition;
			StartPos = base.transform.localPosition - DeltaPos;
		}
		base.transform.localPosition = (InterpForward ? StartPos : EndPos);
		CallbackMethodName = callbackMethodName;
		CallbackScript = callBackScript;
	}
}
