using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_SpeedLineScreenFx : MonoBehaviour
	{
		public enum LineStartMode
		{
			RANDOM_POSITION,
			STAGGERED_START
		}

		public delegate void LineStopCompleted();

		private const float m_leftPositionOfLines = -16f;

		private const float STAGGER_DELAY = 0.1f;

		private static int IDLE_ANIMATION_HASH = Animator.StringToHash("Base Layer.mg_jr_speedLine_idle");

		private bool m_tryingToStop = false;

		private float m_highestLinePosition;

		private float m_lowestLinePosition;

		private float m_lineSpacing;

		private int m_maxLineFX = 10;

		private List<Animator> m_speedLines = new List<Animator>();

		private void Awake()
		{
			mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
			mg_jr_Resources resources = active.Resources;
			Vector3 position = new Vector3(-16f, active.VisibleWorldBounds.max.y, 0f);
			base.transform.position = position;
			m_highestLinePosition = active.VisibleWorldBounds.max.y;
			m_lowestLinePosition = active.VisibleWorldBounds.min.y;
			float num = m_highestLinePosition - m_lowestLinePosition;
			m_lineSpacing = num / (float)(m_maxLineFX - 2);
			for (int i = 0; i < m_maxLineFX; i++)
			{
				GameObject pooledResource = resources.GetPooledResource(mg_jr_ResourceList.GAME_PREFAB_EFFECT_SPEEDLINE);
				Animator component = pooledResource.GetComponent<Animator>();
				Assert.NotNull(component, "SpeedLine needs an animator");
				pooledResource.transform.position = new Vector3(base.transform.position.x, m_highestLinePosition - (float)i * m_lineSpacing, 0f);
				pooledResource.transform.parent = base.transform;
				SpriteRenderer component2 = pooledResource.GetComponent<SpriteRenderer>();
				component2.sortingOrder = mg_jr_SpriteDrawingLayers.Instance.SpriteOrder(mg_jr_SpriteDrawingLayers.DrawingLayers.FX_OVERLAY_0);
				m_speedLines.Add(component);
			}
			m_speedLines = m_speedLines.OrderBy((Animator item) => Random.Range(0, m_speedLines.Count)).ToList();
		}

		public void StartLines(LineStartMode _lineStartMode)
		{
			if (m_tryingToStop)
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Can't start lines while lines are trying to stop");
				return;
			}
			switch (_lineStartMode)
			{
			case LineStartMode.RANDOM_POSITION:
				foreach (Animator speedLine in m_speedLines)
				{
					speedLine.Play("mg_jr_speedLine", -1, Random.Range(0f, 1f));
				}
				break;
			case LineStartMode.STAGGERED_START:
				StartCoroutine(StaggeredStart());
				break;
			default:
				Assert.IsTrue(false, "Unknown line start mode.");
				break;
			}
		}

		public void StopLine(LineStopCompleted _onLinesAllStopped)
		{
			foreach (Animator speedLine in m_speedLines)
			{
				speedLine.SetTrigger("Off");
			}
			StartCoroutine(WaitForLineAnimationsToFinish(_onLinesAllStopped));
		}

		public IEnumerator WaitForLineAnimationsToFinish(LineStopCompleted _onLinesAllStopped)
		{
			m_tryingToStop = true;
			bool areAllLinesStopped = false;
			while (!areAllLinesStopped)
			{
				areAllLinesStopped = true;
				foreach (Animator speedLine in m_speedLines)
				{
					if (speedLine.GetCurrentAnimatorStateInfo(0).fullPathHash != IDLE_ANIMATION_HASH)
					{
						areAllLinesStopped = false;
						break;
					}
				}
				yield return 0;
			}
			m_tryingToStop = false;
			if (_onLinesAllStopped != null)
			{
				_onLinesAllStopped();
			}
		}

		public void StopLinesImmediately()
		{
			foreach (Animator speedLine in m_speedLines)
			{
				speedLine.Play("mg_jr_speedLine_idle", -1, 0f);
			}
		}

		private IEnumerator StaggeredStart()
		{
			int i = 0;
			while (i < m_speedLines.Count)
			{
				m_speedLines[i++].Play("mg_jr_speedLine", -1, 0f);
				yield return new WaitForSeconds(0.1f);
			}
		}
	}
}
