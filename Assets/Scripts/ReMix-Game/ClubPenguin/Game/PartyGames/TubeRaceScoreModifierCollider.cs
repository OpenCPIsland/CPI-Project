using ClubPenguin.Core;
using ClubPenguin.PartyGames;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRaceScoreModifierCollider : MonoBehaviour
	{
		private TubeRaceScoreModifier scoreModifier;

		private bool hasCollidedWithPlayer = false;

		private GameObject DestroyFXPrefab;

		public PrefabContentKey DestroyFX;

		private void Start()
		{
			scoreModifier = GetComponentInParent<TubeRaceScoreModifier>();
			Content.LoadAsync(onDestroyFXComplete, DestroyFX);
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!hasCollidedWithPlayer && collider.gameObject.CompareTag("Player") && scoreModifier != null)
			{
				hasCollidedWithPlayer = true;
				Service.Get<EventDispatcher>().DispatchEvent(new TubeRaceEvents.ScoreModifierCollected(scoreModifier.ModifierId, scoreModifier.ScoreDelta));
				showScoreIndicator();
				playOutroAnimation();
			}
		}

		private void showScoreIndicator()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.RemovePlayerIndicator(Service.Get<CPDataEntityCollection>().LocalPlayerSessionId, false));
			if (scoreModifier.ScoreDelta > 0)
			{
				Content.LoadAsync(onIndicatorPrefabLoadComplete, TubeRaceScoreModifierLayout.PositiveIndicatorKey);
			}
			else if (scoreModifier.ScoreDelta < 0)
			{
				Content.LoadAsync(onIndicatorPrefabLoadComplete, TubeRaceScoreModifierLayout.NegativeIndicatorKey);
			}
		}

		private void onIndicatorPrefabLoadComplete(string path, GameObject indicatorPrefab)
		{
			GameObject gameObject = Object.Instantiate(indicatorPrefab);
			TubeRacePlayerIndicator componentInChildren = gameObject.GetComponentInChildren<TubeRacePlayerIndicator>();
			if (componentInChildren != null)
			{
				componentInChildren.SetScoreModifier(scoreModifier.ScoreDelta);
				Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.ShowPlayerIndicator(gameObject, Service.Get<CPDataEntityCollection>().LocalPlayerSessionId));
			}
		}

		private void playOutroAnimation()
		{
			Object.Instantiate(DestroyFXPrefab, base.transform.position, base.transform.rotation);
			base.gameObject.SetActive(false);
		}

		private void onDestroyFXComplete(string path, GameObject prefab)
		{
			DestroyFXPrefab = prefab;
		}
	}
}
