using System;
using UnityEngine;

public class ScaleSelf : MonoBehaviour
{
	public Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f);

	public Vector3 endScale = Vector3.one;

	public AnimationCurve InterpCurveForward = null;

	public AnimationCurve InterpCurveBackward = null;

	public float Duration = 1f;

	public bool PlayOnStart = true;

	public float MaxDeltaTime = 71f / (678f * (float)Math.PI);

	public float Delay = 0f;

	private float DelayTimer = 0f;

	private bool InterpForward = true;

	private bool Interping = false;

	private float InterpT = 0f;

	private AnimationCurve currentCurve = null;

	private MonoBehaviour CallbackScript = null;

	private string CallbackMethodName = "OnTransitionEnd";

	public string sfxName = "";

	private bool sfxPlayed = false;

	public ParticleSystem particleEffect = null;

	private bool particlePlayed = false;

	public float particleWillPlayAtTime = 0f;

	public float particleTimer = 0f;

	private Renderer[] renderers;

	private void Start()
	{
		if (PlayOnStart)
		{
			base.transform.localScale = startScale;
			Play(true, Delay);
		}
	}

	private void Update()
	{
		if (!Interping)
		{
			return;
		}
		if (DelayTimer > Delay)
		{
			InterpT = Mathf.Clamp(InterpT + Mathf.Min(Time.deltaTime, MaxDeltaTime), 0f, Duration);
			if (InterpT == Duration)
			{
				base.transform.localScale = (InterpForward ? endScale : startScale);
				Interping = false;
				DelayTimer = 0f;
				InterpT = 0f;
				if (CallbackScript != null)
				{
					CallbackScript.SendMessage(CallbackMethodName, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				if (sfxName != "" && !sfxPlayed)
				{
					sfxPlayed = true;
				}
				currentCurve = (InterpForward ? InterpCurveForward : InterpCurveBackward);
				Vector3 vector = InterpForward ? startScale : endScale;
				Vector3 a = InterpForward ? endScale : startScale;
				base.transform.localScale = vector + (a - vector) * currentCurve.Evaluate(InterpT / Duration);
			}
		}
		else
		{
			DelayTimer += Mathf.Min(Time.deltaTime, MaxDeltaTime);
			if (DelayTimer >= Delay && renderers != null)
			{
				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i].enabled = true;
				}
			}
		}
		if (particleEffect != null && !particlePlayed)
		{
			particleTimer += Mathf.Min(Time.deltaTime, MaxDeltaTime);
			if (particleTimer > particleWillPlayAtTime)
			{
				particleEffect.Play();
				particlePlayed = true;
			}
		}
	}

	public void Play(bool forward = true, float delay = 0f, MonoBehaviour callBackScript = null, string callbackMethodName = "OnTransitionEnd")
	{
		Interping = true;
		InterpForward = forward;
		InterpT = 0f;
		Delay = delay;
		sfxPlayed = false;
		base.transform.localScale = startScale;
		if (delay > 0f)
		{
			renderers = base.transform.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].enabled = false;
			}
		}
		CallbackMethodName = callbackMethodName;
		CallbackScript = callBackScript;
	}
}
