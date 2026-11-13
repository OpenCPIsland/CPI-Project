using System;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_BobMovement
	{
		private const float M_PI = (float)Math.PI;

		public float Base;

		private float m_amplitude;

		private float m_rate;

		private float m_offset;

		private float m_totalTime;

		public mg_if_BobMovement(float p_timeRate, float p_amplitude, float p_timeOffset)
		{
			m_amplitude = p_amplitude;
			m_rate = (float)Math.PI * 2f / p_timeRate;
			m_offset = p_timeOffset * m_rate;
			m_totalTime = 0f;
		}

		public float GetValue(float p_deltaTime)
		{
			m_totalTime += p_deltaTime;
			float f = m_totalTime * m_rate * m_offset;
			return Base + Mathf.Sin(f) * m_amplitude;
		}
	}
}
