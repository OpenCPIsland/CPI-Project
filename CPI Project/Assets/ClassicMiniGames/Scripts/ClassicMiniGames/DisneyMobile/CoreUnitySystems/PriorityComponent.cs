using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class PriorityComponent : MonoBehaviour, IPooledComponent
	{
		private int DefaultPriority = 0;

		private int m_priority = 0;

		public int Priority
		{
			get
			{
				return m_priority;
			}
			set
			{
				m_priority = value;
			}
		}

		public void OnSpawn()
		{
		}

		public void Reset()
		{
			m_priority = DefaultPriority;
		}
	}
}
