using ClubPenguin.Progression;
using ClubPenguin.Tutorial;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class LevelUpSequence : MonoBehaviour
	{
		private static PrefabContentKey LevelUpParticlesKey = new PrefabContentKey("FX/Character/Prefabs/LevelUp");

		public static string HAS_SHOWN_RATE_PROMPT_KEY = "HasShownRatePrompt";

		private static TutorialDefinitionKey ProgressTutorialDefinition = new TutorialDefinitionKey(0);

		private static TutorialDefinitionKey MaxLevelTutorialDefinition = new TutorialDefinitionKey(8);

		private LevelUpParticles levelUpParticles;

		private void Start()
		{
			Content.LoadAsync<GameObject>(LevelUpParticlesKey.Key, onLevelUpParticlesLoaded);
		}

		private void onLevelUpParticlesLoaded(string path, GameObject particlesPrefab)
		{
			GameObject gameObject = Object.Instantiate(particlesPrefab);
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.localPosition = Vector3.zero;
			levelUpParticles = gameObject.GetComponent<LevelUpParticles>();
			levelUpParticles.OnLevelUpParticlesComplete += onLevelUpParticlesComplete;
		}

		private void onLevelUpParticlesComplete()
		{
			levelUpParticles.OnLevelUpParticlesComplete -= onLevelUpParticlesComplete;
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
			checkTutorial();
			Object.Destroy(this);
		}

		private void checkTutorial()
		{
			ProgressionService progressionService = Service.Get<ProgressionService>();
			TutorialManager tutorialManager = Service.Get<TutorialManager>();
			if (progressionService != null && tutorialManager != null)
			{
				if (progressionService.Level == progressionService.MaxUnlockLevel)
				{
					tutorialManager.TryStartTutorial(MaxLevelTutorialDefinition.Id);
				}
				else if (progressionService.Level == 1 || progressionService.Level == 2)
				{
					tutorialManager.TryStartTutorial(ProgressTutorialDefinition.Id);
				}
			}
		}
	}
}
