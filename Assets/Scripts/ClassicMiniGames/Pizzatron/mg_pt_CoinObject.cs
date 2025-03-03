using MinigameFramework;
using System;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_CoinObject : MonoBehaviour
	{
		private Vector2 m_velocity;

		private Vector2 m_screenBottom;

		public mg_pt_ECoinState State
		{
			get;
			private set;
		}

		protected virtual void Awake()
		{
			m_screenBottom = MinigameSpriteHelper.GetScreenEdge(EScreenEdge.BOTTOM, MinigameManager.GetActive().MainCamera);
		}

		public void Spawn(Vector2 p_spawnAt)
		{
			State = mg_pt_ECoinState.ACTIVE;
			base.gameObject.SetActive(true);
			base.transform.position = p_spawnAt;
			float num = UnityEngine.Random.Range(0f, 1f) * 90f + 45f;
			float f = (float)Math.PI / 180f * num;
			float num2 = 0.3f;
			m_velocity = new Vector2(num2 * Mathf.Cos(f), num2 * Mathf.Sin(f));
			MinigameManager.GetActive().PlaySFX("mg_pt_sfx_coin");
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			if (State == mg_pt_ECoinState.ACTIVE)
			{
				m_velocity.y -= 1f * p_deltaTime;
				Vector2 v = base.transform.localPosition;
				v += m_velocity;
				base.transform.localPosition = v;
				if (base.transform.position.y < m_screenBottom.y)
				{
					State = mg_pt_ECoinState.INACTIVE;
					base.gameObject.SetActive(false);
				}
			}
		}
	}
}
