using ClubPenguin.Core;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class SimpleSpringInterper : MonoBehaviour
	{
		public delegate void SpringValueChangedHandler(float springValue);

		private float _springValue = 0f;

		private float _springVelocity = 0f;

		private float _springT = -1f;

		private Coroutine _activeRoutine = null;

		public float springAngularFrequency = 5f;

		public float springDampingRatio = 1f;

		private float _springGoal = 0f;

		public float startingGoal = 0f;

		private bool _inited = false;

		private GameObject _gameObject = null;

		public float minimumDelta = 0.001f;

		public float cooloffDuration = 1f;

		private float _maximumDuration = 10f;

		public SpringValueChangedHandler OnSpringValueChanged = null;

		public float springValue
		{
			get
			{
				return _springValue;
			}
			private set
			{
				_springValue = value;
				if (OnSpringValueChanged != null)
				{
					OnSpringValueChanged(value);
				}
			}
		}

		public bool isAtRest
		{
			get
			{
				return _activeRoutine == null;
			}
		}

		public float springGoal
		{
			get
			{
				return _springGoal;
			}
		}

		private void Awake()
		{
			Cache();
		}

		private void Cache()
		{
			if (!_inited)
			{
				_inited = true;
				_gameObject = base.gameObject;
				SetSpringGoal(startingGoal, true);
			}
		}

		public void PushSpring(float strength)
		{
			_springVelocity += strength;
			StartSpring();
		}

		public void SetSpringGoal(float springGoal, bool instant = false)
		{
			Cache();
			_springGoal = springGoal;
			if (instant)
			{
				if (_activeRoutine != null)
				{
					StopCoroutine(_activeRoutine);
				}
				Complete(true);
			}
			else
			{
				StartSpring();
			}
		}

		private void StartSpring()
		{
			if (_activeRoutine == null && _gameObject.activeInHierarchy)
			{
				_activeRoutine = StartCoroutine(ExecuteSpring());
			}
			else
			{
				_springT = 0f;
			}
		}

		private IEnumerator ExecuteSpring()
		{
			_springT = 0f;
			float totalT = 0f;
			float lastSpringValue = _springValue;
			while (_springT < cooloffDuration && totalT < _maximumDuration)
			{
				FeepMath.calcDampedSimpleHarmonicMotion(ref _springValue, ref _springVelocity, _springGoal, Time.deltaTime, springAngularFrequency, springDampingRatio);
				float delta = _springValue - lastSpringValue;
				if (Mathf.Abs(delta) >= minimumDelta)
				{
					lastSpringValue = _springValue;
					_springT = 0f;
					springValue = _springValue;
				}
				yield return null;
				_springT += Time.deltaTime;
				totalT += Time.deltaTime;
			}
			Complete();
		}

		private void Complete(bool forceUpdate = false)
		{
			if (springValue != _springGoal || forceUpdate)
			{
				springValue = _springGoal;
			}
			_springVelocity = 0f;
			_springT = -1f;
			_activeRoutine = null;
		}

		private void OnDisable()
		{
			if (_activeRoutine != null)
			{
				StopCoroutine(_activeRoutine);
				Complete();
			}
		}
	}
}
