using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Collector : MonoBehaviour
	{
		public delegate void ItemCollectedHandler(int _numberCollected);

		public delegate void RoboPenguinCollectedHandler(mg_jr_RobotPenguin _theRoboPenguin);

		public event ItemCollectedHandler CoinCollected;

		public event ItemCollectedHandler TurboCollected;

		public event RoboPenguinCollectedHandler RobotPenguinCollected;

		private void OnTriggerEnter2D(Collider2D other)
		{
			mg_jr_Collectable component = other.GetComponent<mg_jr_Collectable>();
			if (!(component != null))
			{
				return;
			}
			bool flag = false;
			if (component is mg_jr_Coin)
			{
				if (this.CoinCollected != null)
				{
					this.CoinCollected(component.Quantity);
				}
				flag = true;
			}
			else if (component is mg_jr_RobotPenguin)
			{
				if (this.RobotPenguinCollected != null)
				{
					this.RobotPenguinCollected(component as mg_jr_RobotPenguin);
				}
				flag = true;
			}
			else if (component is mg_jr_TurboPickup)
			{
				if (this.TurboCollected != null)
				{
					this.TurboCollected(component.Quantity);
				}
				flag = true;
			}
			if (flag)
			{
				component.OnCollection();
			}
		}
	}
}
