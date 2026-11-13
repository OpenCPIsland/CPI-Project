using ClubPenguin.Igloo;
using Disney.MobileNetwork;
using UnityEngine;

namespace Game.UI.Igloos
{
	public class FlushRecentDecorationsOnDisable : MonoBehaviour
	{
		public void OnDisable()
		{
			RecentDecorationsService recentDecorationsService = Service.Get<RecentDecorationsService>();
			if (recentDecorationsService != null)
			{
				recentDecorationsService.FlushPlayerPrefs();
			}
		}
	}
}
