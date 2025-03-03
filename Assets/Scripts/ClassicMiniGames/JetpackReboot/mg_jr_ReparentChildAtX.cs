using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_ReparentChildAtX : MonoBehaviour
	{
		private float m_xPositionToUnparentAt = float.MaxValue;

		private Transform m_toChangeTo = null;

		public void ChangeCondition(Transform _changeTo, float _atXPosition)
		{
			m_xPositionToUnparentAt = _atXPosition;
			m_toChangeTo = _changeTo;
		}

		private void Update()
		{
			if (base.transform.position.x >= m_xPositionToUnparentAt)
			{
				foreach (Transform item in base.transform)
				{
					item.parent = m_toChangeTo;
					Vector3 position = item.position;
					position.x = m_xPositionToUnparentAt;
					item.position = position;
				}
			}
		}
	}
}
