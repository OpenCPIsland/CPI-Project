using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class MarketplaceTray : MonoBehaviour
	{
		public event Action TrayCloseAnimationComplete;

		public event Action MarketplaceItemAnimationComplete;

		public void OnTrayCloseAnimationComplete()
		{
			if (this.TrayCloseAnimationComplete != null)
			{
				this.TrayCloseAnimationComplete();
			}
		}

		public void OnMarketplaceItemAnimationComplete()
		{
			if (this.MarketplaceItemAnimationComplete != null)
			{
				this.MarketplaceItemAnimationComplete();
			}
		}
	}
}
