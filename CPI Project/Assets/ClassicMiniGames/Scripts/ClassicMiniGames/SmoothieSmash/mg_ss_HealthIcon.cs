using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_HealthIcon : MonoBehaviour
	{
		private Vector2 m_startPos;

		private Vector2 m_targetPos;

		private float m_time;

		public void Initialize(mg_ss_Item_FruitObject p_fruit, Transform p_target)
		{
			m_startPos = p_fruit.transform.position;
			m_targetPos = p_target.position;
			base.transform.position = m_startPos;
		}

		protected void Update()
		{
			m_time += Time.deltaTime;
			base.transform.position = Vector2.Lerp(m_startPos, m_targetPos, m_time / 0.75f);
			if (m_time >= 0.75f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
