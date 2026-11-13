using System.Collections;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/IntroLoopOutroComponent")]
	public class IntroLoopOutroComponent : Component
	{
		public enum Stage
		{
			Intro,
			Loop,
			Outro,
			NumStages
		}

		[HideInInspector]
		[SerializeField]
		public Component[] _stages = new Component[3];

		[HideInInspector]
		[SerializeField]
		public float _transitionOffset;

		[HideInInspector]
		[SerializeField]
		public float _transitionOffsetRandomization;

		[HideInInspector]
		[SerializeField]
		public bool _playLoopToEnd;

		private Stage _activeStage;

		private bool _waitToStop;

		private bool _isCoroutineActive;

		public bool IsCurrentStageActive(int stage)
		{
			if (IsPlaying() && _stages[stage] != null)
			{
				return _stages[stage].IsPlaying();
			}
			return false;
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (CheckMIDI(zComponentInstance))
			{
				base.PlayInternal(zComponentInstance, _fadeInTime, _fadeInCurve, true);
				_waitToStop = false;
				PlayStage(Stage.Intro);
				PlayStage(Stage.Loop);
			}
		}

		private void PlayStage(Stage stage, double time = 0.0)
		{
			if (!(_stages[(int)stage] != null))
			{
				return;
			}
			double num = (double)_transitionOffset + base._random.NextDouble() * (double)_transitionOffsetRandomization;
			if (stage == Stage.Loop)
			{
				if (num < 0.0)
				{
					float time2 = _stages[0].GetLength() + (float)num;
					_isCoroutineActive = true;
					StartCoroutine(onPlayLoop(time2));
				}
				else
				{
					_componentInstance._instance.SetPlayScheduled(0.0, num);
					if (_stages[0] != null)
					{
						_componentInstance._instance.SetPlayScheduled((double)_stages[0].GetLength() + num, 0.0);
					}
					_stages[1].PlayInternal(_componentInstance, 0f, 0.5f);
					_componentInstance._instance.ResetPlayScheduled();
				}
			}
			else
			{
				_componentInstance._instance.SetPlayScheduled(time, 0.0);
				_stages[(int)stage].PlayInternal(_componentInstance, 0f, 0.5f);
			}
			_activeStage = stage;
		}

		private IEnumerator onPlayLoop(float time)
		{
			yield return new WaitForSeconds(time);
			if (_isCoroutineActive)
			{
				if (_stages[0].FadeOutTime > 0f)
				{
					_stages[0].StopInternal(false, false, 0f, 0.5f);
				}
				_stages[1].PlayInternal(_componentInstance, 0f, 0.5f);
			}
			_isCoroutineActive = false;
		}

		public override void StopInternal(bool stopInstances, bool forceStop, float target, float curve, double scheduleEnd = 0.0)
		{
			if (_activeStage == Stage.Intro || _activeStage == Stage.Outro)
			{
				base.StopInternal(stopInstances, forceStop, target, curve, scheduleEnd);
			}
			else if (_activeStage == Stage.Loop && !_playLoopToEnd)
			{
				if (_stages[0] != null)
				{
					_stages[0].StopInternal(false, false, 0f, 0.5f, scheduleEnd);
				}
				if (_stages[1] != null)
				{
					_stages[1].StopInternal(false, false, 0f, 0.5f, scheduleEnd);
					_isCoroutineActive = false;
				}
				PlayStage(Stage.Outro);
			}
			else
			{
				_waitToStop = true;
			}
		}

		internal override void OnFinishPlaying(double time)
		{
			if (_activeStage == Stage.Loop && _stages[1] != null && _waitToStop)
			{
				_stages[1].StopInternal(false, false, 0f, 0.5f, time);
				PlayStage(Stage.Outro, time);
			}
			else if (_activeStage == Stage.Outro)
			{
				base.OnFinishPlaying(time);
			}
		}
	}
}
