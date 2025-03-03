using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Tutorial
{
	[RequireComponent(typeof(Button))]
	internal class TutorialButtonTrigger : MonoBehaviour
	{
		public TutorialDefinition Definition;

		private Button button;

		private void Start()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(onButtonClick);
		}

		private void OnDestroy()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(onButtonClick);
			}
		}

		private void onButtonClick()
		{
			triggerTutorial();
		}

		private void triggerTutorial()
		{
			Service.Get<TutorialManager>().TryStartTutorial(Definition);
		}
	}
}
