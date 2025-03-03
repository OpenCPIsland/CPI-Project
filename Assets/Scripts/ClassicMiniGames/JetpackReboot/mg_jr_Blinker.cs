using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Blinker : MonoBehaviour
	{
		private bool m_isCurrentlyVisible = true;

		private List<Renderer> m_renderersToBlink;

		private bool m_isBlinkingForDuration = false;

		private float m_duration = 3f;

		private float m_timeSinceVisibilityChange = 0f;

		private float m_timeSinceStartBlink = 0f;

		public float BlinkPeriod
		{
			get;
			set;
		}

		private void Awake()
		{
			BlinkPeriod = 0.1f;
			m_renderersToBlink = new List<Renderer>();
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			m_renderersToBlink.AddRange(componentsInChildren);
			base.enabled = false;
		}

		private void Update()
		{
			if (!MinigameManager.IsPaused)
			{
				m_timeSinceVisibilityChange += Time.deltaTime;
				if (m_timeSinceVisibilityChange > BlinkPeriod * 0.5f)
				{
					SetVisibilityOfRenderers(!m_isCurrentlyVisible);
				}
				m_timeSinceStartBlink += Time.deltaTime;
				if (m_isBlinkingForDuration && m_timeSinceStartBlink > m_duration)
				{
					StopBlinking();
				}
			}
		}

		private void SetVisibilityOfRenderers(bool _visible)
		{
			m_isCurrentlyVisible = _visible;
			foreach (Renderer item in m_renderersToBlink)
			{
				item.enabled = m_isCurrentlyVisible;
			}
			m_timeSinceVisibilityChange = 0f;
		}

		public void StartBlinking(float _duration)
		{
			m_isBlinkingForDuration = true;
			m_duration = _duration;
			StartBlinkingCommon();
		}

		public void StartBlinking()
		{
			m_isBlinkingForDuration = false;
			StartBlinkingCommon();
		}

		private void StartBlinkingCommon()
		{
			m_timeSinceStartBlink = 0f;
			m_timeSinceVisibilityChange = 0f;
			base.enabled = true;
			SetVisibilityOfRenderers(false);
		}

		public void StopBlinking()
		{
			m_isBlinkingForDuration = false;
			base.enabled = false;
			SetVisibilityOfRenderers(true);
		}

		public float CurrentPhase()
		{
			return m_timeSinceStartBlink / BlinkPeriod;
		}

		public void StartBlinkingFromPhase(float _phase)
		{
			m_isBlinkingForDuration = false;
			StartBlinkingCommon();
			if (_phase < 0.5f)
			{
				SetVisibilityOfRenderers(false);
				m_timeSinceVisibilityChange = BlinkPeriod * _phase;
			}
			else
			{
				SetVisibilityOfRenderers(true);
				m_timeSinceVisibilityChange = BlinkPeriod * _phase + BlinkPeriod * 0.5f;
			}
		}

		public bool IsBlinking()
		{
			return base.enabled;
		}
	}
}
