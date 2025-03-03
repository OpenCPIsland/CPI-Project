using System;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_BlobObject : MonoBehaviour
	{
		private Vector2 m_initialPos;

		private Vector2 m_currentPos;

		private Vector2 m_finalPos;

		private float m_initialScale;

		private float m_currentScale;

		private float m_finalScale;

		private float m_startDelay;

		private float m_sinMod;

		private bool m_isVisible;

		public bool DoSplat
		{
			get;
			private set;
		}

		public void Initialize(int p_blobNumber, Vector2 p_initialOffset, Vector2 p_finalOffset, float p_initialClusterRadiusX, float p_initialClusterRadiusY, float p_finalClusterRadiusX, float p_finalClusterRadiusY, float p_initialBlobScale, float p_finalBlobScale, float p_blobScaleVariationPercentage, Color p_color, float p_blobDelay, float p_blobSinMin, float p_blobSinMax)
		{
			GetComponent<SpriteRenderer>().color = p_color;
			m_initialPos = p_initialOffset;
			m_finalPos = p_finalOffset;
			m_startDelay = p_blobDelay * (float)(p_blobNumber - 1) + p_blobDelay * (UnityEngine.Random.Range(0f, 50f) / 100f + 0.5f);
			m_isVisible = true;
			DoSplat = false;
			RandomRadii(p_initialClusterRadiusX, p_initialClusterRadiusY, p_finalClusterRadiusX, p_finalClusterRadiusY);
			m_initialScale = p_initialBlobScale;
			m_finalScale = p_finalBlobScale;
			int num = Mathf.RoundToInt(p_blobScaleVariationPercentage);
			int num2 = UnityEngine.Random.Range(0, num * 2) - num;
			if (num2 > 0)
			{
				m_initialScale *= 1f + (float)num2 / 100f;
				m_finalScale *= 1f + (float)num2 / 100f;
			}
			m_currentScale = m_initialScale;
			m_sinMod = UnityEngine.Random.Range(p_blobSinMin, p_blobSinMax);
			m_currentPos = m_initialPos;
			UpdateBlob();
		}

		private void RandomRadii(float p_initialRadiusX, float p_initialRadiusY, float p_finalRadiusX, float p_finalRadiusY)
		{
			float num = 0f - UnityEngine.Random.Range(0f, 360f);
			num = (float)Math.PI * num / 180f;
			m_initialPos.x += (UnityEngine.Random.Range(0f, p_initialRadiusX) - p_initialRadiusX) * Mathf.Sin(num);
			m_initialPos.y += (UnityEngine.Random.Range(0f, p_initialRadiusY) - p_initialRadiusY) * Mathf.Cos(num);
			m_finalPos.x += (UnityEngine.Random.Range(0f, p_finalRadiusX) - p_finalRadiusX) * Mathf.Sin(num);
			m_finalPos.y += (UnityEngine.Random.Range(0f, p_finalRadiusY) - p_finalRadiusY) * Mathf.Cos(num);
		}

		public void MinigameUpdate(float p_timeActive, float p_blobTTL, bool p_showSplatter)
		{
			if (p_timeActive < m_startDelay || !m_isVisible)
			{
				return;
			}
			float num = (p_timeActive - m_startDelay) / p_blobTTL;
			if (num > 1f)
			{
				m_isVisible = false;
				if (p_showSplatter)
				{
					DoSplat = true;
				}
			}
			else
			{
				m_currentPos.x = m_initialPos.x + (m_finalPos.x - m_initialPos.x) * num;
				m_currentPos.y = m_initialPos.y + (m_finalPos.y - m_initialPos.y) * num;
				m_currentPos.y += Mathf.Sin(num * (float)Math.PI) * m_sinMod;
				m_currentScale = m_initialScale + (m_finalScale - m_initialScale) * num;
			}
			UpdateBlob();
		}

		private void UpdateBlob()
		{
			base.transform.position = m_currentPos;
			Vector2 v = base.transform.localScale;
			v.x = m_currentScale;
			v.y = m_currentScale;
			base.transform.localScale = v;
			base.gameObject.SetActive(m_isVisible);
		}

		public Vector3 GetSplatPosition()
		{
			DoSplat = false;
			return m_currentPos;
		}
	}
}
