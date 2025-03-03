using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRacePlayerIndicator : MonoBehaviour
	{
		public float TimeShownInSeconds;

		public Text ScoreText;

		private void Start()
		{
			CoroutineRunner.Start(removeIndicatorCoroutine(), this, "removeIndicatorCoroutine");
		}

		public void SetScoreModifier(int scoreModifier)
		{
			ScoreText.text = ((scoreModifier > 0) ? ("+" + scoreModifier) : scoreModifier.ToString());
		}

		private IEnumerator removeIndicatorCoroutine()
		{
			yield return new WaitForSeconds(TimeShownInSeconds);
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.RemovePlayerIndicator(Service.Get<CPDataEntityCollection>().LocalPlayerSessionId, false));
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
