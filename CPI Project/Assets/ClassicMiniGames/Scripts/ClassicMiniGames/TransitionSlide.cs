using System;
using UnityEngine;

public class TransitionSlide : MonoBehaviour
{
	private Vector3 StartPos = Vector3.zero;

	private Vector3 EndPos = Vector3.zero;

	public Vector3 SlidePath = Vector3.zero;

	public AnimationCurve InterpCurveForward = null;

	public float Duration = 1f;

	public bool PlayOnStart = false;

	public float MaxDeltaTime = 71f / (678f * (float)Math.PI);

	private bool InterpForward = true;

	private bool Interping = false;

	private float InterpT = 0f;

	private GameObject callBackDelegate = null;

	private string CallbackMethodName = "OnTransitionEnd";

	private void Awake()
	{
		StartPos = base.transform.localPosition;
		EndPos = StartPos + SlidePath;
	}

	private void Start()
	{
		if (PlayOnStart)
		{
			base.transform.localPosition = StartPos;
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
			base.transform.localPosition = (InterpForward ? EndPos : StartPos);
			Interping = false;
			if (callBackDelegate != null)
			{
				callBackDelegate.SendMessage(CallbackMethodName, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Vector3 startPos = StartPos;
			AnimationCurve interpCurveForward = InterpCurveForward;
			base.transform.localPosition = startPos + SlidePath * interpCurveForward.Evaluate(InterpForward ? (InterpT / Duration) : (1f - InterpT / Duration));
		}
	}

	public void Play(bool forward = true, GameObject go = null, string callbackMethodName = "OnTransitionEnd")
	{
		callBackDelegate = go;
		Interping = true;
		InterpForward = forward;
		InterpT = 0f;
		if (forward)
		{
			StartPos = base.transform.localPosition;
			EndPos = StartPos + SlidePath;
		}
		else
		{
			EndPos = base.transform.localPosition;
			StartPos = EndPos - SlidePath;
		}
		base.transform.localPosition = (InterpForward ? StartPos : EndPos);
		CallbackMethodName = callbackMethodName;
	}
}
