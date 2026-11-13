using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class DisableWhenNoGameService : MonoBehaviour
	{
		private void Start()
		{
			GameSettings gameSettings = Service.Get<GameSettings>();
			if (gameSettings.OfflineMode && string.IsNullOrEmpty(gameSettings.CPAPIServicehost))
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
