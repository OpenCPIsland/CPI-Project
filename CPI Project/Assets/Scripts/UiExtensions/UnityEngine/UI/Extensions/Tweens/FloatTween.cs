using UnityEngine.Events;

namespace UnityEngine.UI.Extensions.Tweens
{
	public struct FloatTween : ITweenValue
	{
		public class FloatTweenCallback : UnityEvent<float>
		{
		}

		public class FloatFinishCallback : UnityEvent
		{
		}

		private float m_StartFloat;

		private float m_TargetFloat;

		private float m_Duration;

		private bool m_IgnoreTimeScale;

		private FloatTweenCallback m_Target;

		private FloatFinishCallback m_Finish;

		public float startFloat
		{
			get
			{
				return m_StartFloat;
			}
			set
			{
				m_StartFloat = value;
			}
		}

		public float targetFloat
		{
			get
			{
				return m_TargetFloat;
			}
			set
			{
				m_TargetFloat = value;
			}
		}

		public float duration
		{
			get
			{
				return m_Duration;
			}
			set
			{
				m_Duration = value;
			}
		}

		public bool ignoreTimeScale
		{
			get
			{
				return m_IgnoreTimeScale;
			}
			set
			{
				m_IgnoreTimeScale = value;
			}
		}

		public void TweenValue(float floatPercentage)
		{
			if (ValidTarget())
			{
				m_Target.Invoke(Mathf.Lerp(m_StartFloat, m_TargetFloat, floatPercentage));
			}
		}

		public void AddOnChangedCallback(UnityAction<float> callback)
		{
			if (m_Target == null)
			{
				m_Target = new FloatTweenCallback();
			}
			m_Target.AddListener(callback);
		}

		public void AddOnFinishCallback(UnityAction callback)
		{
			if (m_Finish == null)
			{
				m_Finish = new FloatFinishCallback();
			}
			m_Finish.AddListener(callback);
		}

		public bool GetIgnoreTimescale()
		{
			return m_IgnoreTimeScale;
		}

		public float GetDuration()
		{
			return m_Duration;
		}

		public bool ValidTarget()
		{
			return m_Target != null;
		}

		public void Finished()
		{
			if (m_Finish != null)
			{
				m_Finish.Invoke();
			}
		}
	}
}
