using CameraExtensionMethods;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Warning : MonoBehaviour
	{
		private const float RED_DISTANCE = 5.12f;

		private const float YELLOW_DISTANCE = 10.24f;

		private const float SIGN_SEPARATION = 0.1f;

		private mg_JetpackReboot m_miniGame;

		private SpriteRenderer m_renderer;

		private List<mg_jr_UICautionSign> m_warningSigns = new List<mg_jr_UICautionSign>();

		private bool m_isWarningActive = false;

		private bool m_isThreatToRight = true;

		public SpriteRenderer RendererToWarnAbout
		{
			get
			{
				return m_renderer;
			}
			set
			{
				Assert.NotNull(value, "Can't set RendererToWarnAbout to null");
				m_renderer = value;
			}
		}

		protected virtual void Awake()
		{
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			Assert.NotNull(m_miniGame, "mini game not found");
			m_renderer = GetComponentsInChildren<SpriteRenderer>(true)[0];
			Assert.NotNull(m_renderer, "Renderer not found");
		}

		public void ActivateWarning(int _numberOfSigns, bool _isThreatToRight = true)
		{
			if (_numberOfSigns != m_warningSigns.Count)
			{
				NumberOfSignsChanged(_numberOfSigns);
			}
			m_isThreatToRight = _isThreatToRight;
			m_isWarningActive = true;
			ArrangeSigns();
		}

		public void DeactivateWarning()
		{
			m_isWarningActive = false;
			foreach (mg_jr_UICautionSign warningSign in m_warningSigns)
			{
				warningSign.ChangeState(mg_jr_UICautionSign.SignState.INACTIVE);
			}
		}

		private void NumberOfSignsChanged(int _newCount)
		{
			if (_newCount > m_warningSigns.Count)
			{
				int num = _newCount - m_warningSigns.Count;
				for (int i = 0; i < num; i++)
				{
					mg_jr_UICautionSign pooledResourceByComponent = m_miniGame.Resources.GetPooledResourceByComponent<mg_jr_UICautionSign>(mg_jr_ResourceList.WARNING_SIGN);
					pooledResourceByComponent.ChangeState(mg_jr_UICautionSign.SignState.INACTIVE);
					m_warningSigns.Add(pooledResourceByComponent);
					pooledResourceByComponent.transform.parent = m_miniGame.GameLogic.transform;
				}
			}
			else
			{
				if (_newCount >= m_warningSigns.Count)
				{
					return;
				}
				for (int i = m_warningSigns.Count - 1; i >= _newCount; i--)
				{
					if (m_warningSigns[i] != null)
					{
						m_warningSigns[i].ChangeState(mg_jr_UICautionSign.SignState.INACTIVE);
						m_miniGame.Resources.ReturnPooledResource(m_warningSigns[i].gameObject);
						m_warningSigns.RemoveAt(i);
					}
				}
			}
		}

		private void ArrangeSigns()
		{
			Assert.IsTrue(m_warningSigns.Count > 0, "No signs in warning sign list");
			float y = m_warningSigns[0].SignSize.y;
			float num = y * 0.5f;
			float num2 = y * (float)m_warningSigns.Count + 0.1f * (float)(m_warningSigns.Count - 1);
			float num3 = num2 * 0.5f;
			for (int i = 0; i < m_warningSigns.Count; i++)
			{
				float verticalPosition = m_renderer.bounds.center.y + num3 - num - (float)i * (0.1f + y);
				m_warningSigns[i].SetPosition(verticalPosition, m_isThreatToRight);
				m_warningSigns[i].ChangeState(mg_jr_UICautionSign.SignState.INACTIVE);
			}
		}

		private void Update()
		{
			if (!m_miniGame.IsPaused && m_isWarningActive)
			{
				float num = DistanceToNearestScreenEdge();
				if (num <= 0f)
				{
					DeactivateWarning();
				}
				else if (num < 5.12f)
				{
					foreach (mg_jr_UICautionSign warningSign in m_warningSigns)
					{
						warningSign.ChangeState(mg_jr_UICautionSign.SignState.SHOWING_RED);
					}
				}
				else if (num < 10.24f)
				{
					foreach (mg_jr_UICautionSign warningSign2 in m_warningSigns)
					{
						warningSign2.ChangeState(mg_jr_UICautionSign.SignState.SHOWING_YELLOW);
					}
				}
			}
		}

		private void OnDisable()
		{
			m_isWarningActive = false;
			for (int num = m_warningSigns.Count - 1; num >= 0; num--)
			{
				if (m_warningSigns[num] != null)
				{
					m_warningSigns[num].ChangeState(mg_jr_UICautionSign.SignState.INACTIVE);
					m_miniGame.Resources.ReturnPooledResource(m_warningSigns[num].gameObject);
					m_warningSigns.RemoveAt(num);
				}
			}
			m_warningSigns.Clear();
		}

		private float DistanceToNearestScreenEdge()
		{
			float num = 0f;
			float x;
			float num2;
			if (m_isThreatToRight)
			{
				x = m_renderer.bounds.min.x;
				num2 = Camera.main.RightEdgeInWorld();
				return x - num2;
			}
			x = m_renderer.bounds.max.x;
			num2 = Camera.main.LeftEdgeInWorld();
			return 0f - (x - num2);
		}
	}
}
