using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class PenguinRotationAnimate : MonoBehaviour
	{
		private const int HISTORY_COUNT = 10;

		private const int ZEROES_COUNT = 3;

		public SimpleSpringInterper rotateSpring = null;

		public AnimationCurve forceFalloffCurve = null;

		public Transform rotationHandle = null;

		public float forceDecayFactor = 0.95f;

		public float releaseStrengthFactor = 1f;

		private float[] forceHistories = new float[10];

		private float[] forceHistoriesTime = new float[10];

		private int forceHistoryIndex = 0;

		private int forceHistoryCount = 0;

		private Quaternion _originalRotation = Quaternion.identity;

		private bool _isForceActive = false;

		private float _minimumForce = 0.001f;

		public bool isAtRest
		{
			get
			{
				return rotateSpring.isAtRest;
			}
		}

		private void Awake()
		{
			_originalRotation = rotationHandle.localRotation;
			SimpleSpringInterper simpleSpringInterper = rotateSpring;
			simpleSpringInterper.OnSpringValueChanged = (SimpleSpringInterper.SpringValueChangedHandler)Delegate.Combine(simpleSpringInterper.OnSpringValueChanged, new SimpleSpringInterper.SpringValueChangedHandler(OnRotateValueChanged));
		}

		public void ResetRotationWithOffset(float rotationYOffset)
		{
			StopForces();
			setCurrentRotationPreventingGimbalLock();
			rotateSpring.SetSpringGoal(rotationYOffset);
		}

		public void ResetRotation(bool immediate = false)
		{
			StopForces();
			if (!immediate)
			{
				setCurrentRotationPreventingGimbalLock();
			}
			rotateSpring.SetSpringGoal(0f, immediate);
		}

		private void setCurrentRotationPreventingGimbalLock()
		{
			float num;
			for (num = rotateSpring.springValue; num >= 180f; num -= 360f)
			{
			}
			for (; num <= -180f; num += 360f)
			{
			}
			rotateSpring.SetSpringGoal(num, true);
		}

		public void OffsetRotation(float degrees)
		{
			StopForces();
			rotateSpring.SetSpringGoal(rotateSpring.springValue + degrees, true);
			forceHistories[forceHistoryIndex] = degrees;
			forceHistoriesTime[forceHistoryIndex] = Time.time;
			forceHistoryIndex = (forceHistoryIndex + 1) % 10;
			forceHistoryCount++;
		}

		public void AddForce(float force)
		{
			StopForces();
			CoroutineRunner.Start(ExecuteForce(force), this, "ExecuteForce");
		}

		public void AddReleaseForce()
		{
			float num = 0f;
			int num2 = 0;
			for (int i = 0; i < 10; i++)
			{
				int num3 = SafeMod(forceHistoryIndex - i - 1, 10);
				if (i >= forceHistoryCount)
				{
					continue;
				}
				float num4 = Time.time - forceHistoriesTime[num3];
				float num5 = 1f - Mathf.Clamp01(num4 / 0.333333343f);
				if (forceFalloffCurve != null && forceFalloffCurve.length > 0)
				{
					num5 = forceFalloffCurve.Evaluate(num5);
				}
				float num6 = Mathf.Lerp(0f, forceHistories[num3], num5);
				num += num6;
				if (num2 >= 0 && num6 == 0f)
				{
					num2++;
					if (num2 >= 3)
					{
						num = 0f;
						break;
					}
				}
				else
				{
					num2 = -1;
				}
			}
			if (num != 0f)
			{
				AddForce(num * releaseStrengthFactor);
			}
		}

		private int SafeMod(int val, int mod)
		{
			while (val < 0 && mod > 0)
			{
				val += mod;
			}
			return val % mod;
		}

		private IEnumerator ExecuteForce(float force)
		{
			_isForceActive = true;
			rotateSpring.PushSpring(force);
			while (Mathf.Abs(force) > _minimumForce)
			{
				rotateSpring.SetSpringGoal(rotateSpring.springGoal + force * Time.deltaTime);
				force *= forceDecayFactor;
				yield return null;
			}
			StopForces();
		}

		private void StopForces()
		{
			if (_isForceActive)
			{
				CoroutineRunner.StopAllForOwner(this);
				_isForceActive = false;
				forceHistoryIndex = 0;
				forceHistoryCount = 0;
			}
		}

		private void OnRotateValueChanged(float interp)
		{
			Quaternion rhs = Quaternion.Euler(Vector3.up * interp);
			rotationHandle.localRotation = _originalRotation * rhs;
		}
	}
}
