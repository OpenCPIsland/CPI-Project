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
			if (other.CompareTag("Player") && Service.Get<QuestService>().ActiveQuest == null && (!PlayerPrefs.HasKey(GetPlatformKey("DisneyStoreShowTutorial")) || PlayerPrefs.GetInt(GetPlatformKey("DisneyStoreShowTutorial")) == 1) && Service.Get<TutorialManager>().TryStartTutorial(TutorialDefinition.Id))
			{
				PlayerPrefs.SetInt(GetPlatformKey("DisneyStoreShowTutorial"), 0);
				Service.Get<TutorialManager>().SetTutorial(TutorialDefinition.Id, true);
			}
		}

		private static string GetPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (UnityEngine.Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}
	}
}
