using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Follow : MonoBehaviour
	{
		private GameObject m_followTarget = null;

		private Vector3 m_offset = Vector3.zero;

		private float m_distanceReductionRate = 0.15f;

		private bool m_lockPositionWhenClose;

		private float m_distanceSqrToEngageLock = 0.01f;

		private bool m_isPositionCurrentlyLocked = false;

		public bool IsFollowing
		{
			get
			{
				return m_followTarget != null && base.enabled;
			}
		}

		public float DistanceReductionRate
		{
			get
			{
				return m_distanceReductionRate;
			}
			set
			{
				m_distanceReductionRate = Mathf.Clamp(value, 0.05f, 1f);
			}
		}

		private bool LockPositionWhenClose
		{
			get
			{
				return m_lockPositionWhenClose;
			}
			set
			{
				m_lockPositionWhenClose = value;
				if (!m_lockPositionWhenClose)
				{
					m_isPositionCurrentlyLocked = false;
				}
			}
		}

		private void FixedUpdate()
		{
			if (!(m_followTarget != null))
			{
				return;
			}
			Vector3 position = m_followTarget.transform.position;
			Vector3 vector = position + m_offset;
			Vector3 vector2 = vector - base.transform.position;
			if (!m_isPositionCurrentlyLocked)
			{
				if (m_lockPositionWhenClose && vector2.sqrMagnitude < m_distanceSqrToEngageLock)
				{
					base.transform.position = vector;
					m_isPositionCurrentlyLocked = true;
				}
				else
				{
					vector2 *= DistanceReductionRate;
					base.transform.position += vector2;
				}
			}
			else
			{
				base.transform.position = vector;
			}
		}

		public void FollowTarget(GameObject _target, Vector3 _offset, bool _lockWhenClose = false)
		{
			if (_target == null)
			{
				ClearFollowTarget();
				return;
			}
			m_isPositionCurrentlyLocked = false;
			m_followTarget = _target;
			m_offset = _offset;
			base.enabled = true;
			LockPositionWhenClose = _lockWhenClose;
		}

		public void ClearFollowTarget()
		{
			m_followTarget = null;
			base.enabled = false;
			m_isPositionCurrentlyLocked = false;
			LockPositionWhenClose = false;
		}
	}
}
