using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class FindFourPlayerHud : MonoBehaviour
	{
		private const int DEFAULT_SELECTOR_INDEX = 0;

		private const int LOCAL_PLAYER_SELECTOR_INDEX = 1;

		public SpriteSelector TokenSpriteSelector;

		public Text PlayerNameText;

		public TintSelector BgTintSelector;

		public GameObject ActiveOutline;

		public GameObject TimerObject;

		public UITimer Timer;

		private bool isLocalPlayer;

		private bool isTimerRunning;

		private float timeLeft;

		public void Init(long playerId, int playerNum)
		{
			DataEntityHandle handle = Service.Get<CPDataEntityCollection>().FindEntity<SessionIdData, long>(playerId);
			DisplayNameData component;
			if (Service.Get<CPDataEntityCollection>().TryGetComponent(handle, out component))
			{
				PlayerNameText.text = component.DisplayName;
			}
			TokenSpriteSelector.SelectSprite(playerNum);
			isLocalPlayer = (playerId == Service.Get<CPDataEntityCollection>().LocalPlayerSessionId);
			if (isLocalPlayer)
			{
				ActiveOutline.GetComponent<TintSelector>().SelectColor(1);
			}
		}

		public void SetHighlighted(bool highlighted)
		{
			ActiveOutline.SetActive(highlighted);
			if (isLocalPlayer)
			{
				if (highlighted)
				{
					PlayerNameText.GetComponent<TintSelector>().SelectColor(1);
					BgTintSelector.SelectColor(1);
				}
				else
				{
					PlayerNameText.GetComponent<TintSelector>().SelectColor(0);
					BgTintSelector.SelectColor(0);
				}
			}
		}

		public void StartTimer(float time)
		{
			TokenSpriteSelector.gameObject.SetActive(false);
			TimerObject.SetActive(true);
			Timer.StartCountdown(time);
		}

		public void StopTimer()
		{
			TokenSpriteSelector.gameObject.SetActive(true);
			TimerObject.SetActive(false);
			Timer.StopCountdown();
		}
	}
}
