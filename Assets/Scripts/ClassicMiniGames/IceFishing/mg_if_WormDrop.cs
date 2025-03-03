using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_WormDrop : MonoBehaviour
	{
		private float m_dropSpeed;

		public void Awake()
		{
			m_dropSpeed = MinigameManager.GetActive<mg_IceFishing>().Resources.Variables.WormDropSpeed;
		}

		public void DropWorm(Vector2 p_startingPos)
		{
			base.transform.position = p_startingPos;
			base.gameObject.SetActive(true);
		}

		private void Update()
		{
			Vector2 v = base.transform.position;
			v.y -= Time.deltaTime * m_dropSpeed;
			base.transform.position = v;
		}

		private void OnBecameInvisible()
		{
			base.gameObject.SetActive(false);
		}
	}
}
