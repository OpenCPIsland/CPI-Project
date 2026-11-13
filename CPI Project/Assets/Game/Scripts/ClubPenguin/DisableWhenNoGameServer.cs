using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class DisableWhenNoGameServer : MonoBehaviour
	{
		private void Start()
		{
			if (!IsGameServerAvailable())
			{
				base.gameObject.SetActive(false);
			}
		}

		public static bool IsGameServerAvailable()
		{
			GameSettings gameSettings = Service.Get<GameSettings>();
			return !gameSettings.OfflineMode || !string.IsNullOrEmpty(gameSettings.GameServerHost);
		}
	}
}
