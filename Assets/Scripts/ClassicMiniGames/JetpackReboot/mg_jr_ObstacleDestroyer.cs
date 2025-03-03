using MinigameFramework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_ObstacleDestroyer : MonoBehaviour
	{
		public delegate void ObstacleDestroyedHandler(int _coinValue);

		private mg_jr_GoalManager m_goalManager;

		public event ObstacleDestroyedHandler ObstacleDestroyed;

		private void Awake()
		{
			m_goalManager = MinigameManager.GetActive<mg_JetpackReboot>().GoalManager;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!base.enabled)
			{
				return;
			}
			mg_jr_Obstacle component = other.GetComponent<mg_jr_Obstacle>();
			if (component != null)
			{
				component.Explode();
				m_goalManager.AddToProgress(mg_jr_Goal.GoalType.DESTROY_OBSTACLES, 1f);
				if (this.ObstacleDestroyed != null)
				{
					this.ObstacleDestroyed(component.CoinValue);
				}
			}
		}
	}
}
