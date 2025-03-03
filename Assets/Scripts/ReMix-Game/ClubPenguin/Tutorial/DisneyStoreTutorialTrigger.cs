using ClubPenguin.Adventure;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Tutorial
{
	public class DisneyStoreTutorialTrigger : MonoBehaviour
	{
		public TutorialDefinitionKey TutorialDefinition;

		private void Start()
		{
			if (!Service.Get<TutorialManager>().IsTutorialAvailable(TutorialDefinition.Id))
			{
				base.gameObject.SetActive(false);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player") && Service.Get<QuestService>().ActiveQuest == null && (!PlayerPrefs.HasKey("DisneyStoreShowTutorial") || PlayerPrefs.GetInt("DisneyStoreShowTutorial") == 1) && Service.Get<TutorialManager>().TryStartTutorial(TutorialDefinition.Id))
			{
				PlayerPrefs.SetInt("DisneyStoreShowTutorial", 0);
				Service.Get<TutorialManager>().SetTutorial(TutorialDefinition.Id, true);
			}
		}
	}
}
