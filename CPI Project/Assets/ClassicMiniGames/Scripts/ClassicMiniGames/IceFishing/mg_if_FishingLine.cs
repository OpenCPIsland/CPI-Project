using UnityEngine;

namespace IceFishing
{
	public class mg_if_FishingLine : MonoBehaviour
	{
		private mg_if_FishingRod m_rod;

		private GameObject m_shock;

		private float m_originalHeight;

		public void Awake()
		{
			m_shock = base.transform.Find("mg_if_Shock").gameObject;
		}

		public void Start()
		{
			m_originalHeight = GetComponent<SpriteRenderer>().bounds.size.y;
		}

		public void Initialize(mg_if_FishingRod p_rod)
		{
			m_rod = p_rod;
			StopShock();
		}

		public void OnTriggerEnter2D(Collider2D p_other)
		{
			if (!m_rod.IsBroken && p_other.name != "object_size")
			{
				mg_if_ObstacleLine componentInParent = p_other.GetComponentInParent<mg_if_ObstacleLine>();
				if (componentInParent != null)
				{
					componentInParent.OnObstacleHitLine(m_rod);
				}
			}
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			float y = Mathf.Abs(m_rod.HookTransform.position.y - base.transform.position.y) / m_originalHeight;
			base.transform.localScale = new Vector3(base.transform.localScale.x, y);
		}

		public void Shock()
		{
			m_shock.SetActive(true);
		}

		public void StopShock()
		{
			m_shock.SetActive(false);
		}
	}
}
