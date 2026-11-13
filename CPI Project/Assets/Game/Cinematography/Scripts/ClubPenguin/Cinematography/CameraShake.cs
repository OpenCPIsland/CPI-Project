using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public class CameraShake : MonoBehaviour
	{
		private const float PI2 = (float)Math.PI * 2f;

		public float ShakeDuration = 0f;

		public bool UseCurve = false;

		public float ShakeSpeed = 20f;

		public float ShakeAmount = 0.04f;

		public float ShakeDecay = 0.97f;

		public AnimationCurve ShakeSpeedCurve;

		public AnimationCurve ShakeAmountCurve;

		private float currentShakeSpeed = 0f;

		private float currentShakeAmount = 0f;

		private float currentShakeDecay = 0f;

		private float shakeTimer = 0f;

		private bool isShaking = false;

		private Vector3 startPos = Vector3.zero;

		private Vector3 shake = Vector3.zero;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<CinematographyEvents.CameraShakeEvent>(onCameraShake);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<CinematographyEvents.CameraShakeEvent>(onCameraShake);
		}

		private void Update()
		{
			if (!isShaking)
			{
				return;
			}
			shakeTimer += Time.deltaTime;
			if (UseCurve && ShakeDuration > 0f && shakeTimer < ShakeDuration)
			{
				float time = shakeTimer / ShakeDuration;
				currentShakeSpeed = ShakeSpeedCurve.Evaluate(time);
				currentShakeAmount = ShakeAmountCurve.Evaluate(time);
			}
			shake.x = Mathf.Sin(shakeTimer * currentShakeSpeed) * currentShakeAmount;
			shake.y = Mathf.Sin((shakeTimer + 1.3f) * currentShakeSpeed * 1.2f) * currentShakeAmount;
			base.transform.localPosition = startPos + shake;
			if (ShakeDuration != 0f && !(shakeTimer > ShakeDuration))
			{
				return;
			}
			if (currentShakeDecay == 1f)
			{
				stopShake();
			}
			else if (currentShakeDecay != 0f)
			{
				currentShakeAmount *= 1f - currentShakeDecay * Time.deltaTime;
				if (currentShakeAmount < 0.01f)
				{
					stopShake();
				}
			}
		}

		private void startShake(float shakeSpeed, float shakeAmount, float shakeDecay, float shakeDuration)
		{
			currentShakeSpeed = shakeSpeed;
			currentShakeAmount = shakeAmount;
			currentShakeDecay = shakeDecay;
			ShakeDuration = shakeDuration;
			startPos = base.transform.localPosition;
			shake = Vector3.zero;
			isShaking = true;
		}

		private void stopShake()
		{
			currentShakeAmount = 0f;
			currentShakeSpeed = 0f;
			isShaking = false;
			shakeTimer = 0f;
			base.transform.localPosition = startPos;
		}

		private bool onCameraShake(CinematographyEvents.CameraShakeEvent evt)
		{
			UseCurve = evt.UseCurve;
			if (UseCurve)
			{
				ShakeSpeedCurve = evt.ShakeSpeedCurve;
				ShakeAmountCurve = evt.ShakeAmountCurve;
			}
			startShake(evt.ShakeSpeed, evt.ShakeAmount, evt.ShakeDecay, evt.ShakeDuration);
			return false;
		}
	}
}
