using ClubPenguin.TutorialUI;
using Disney.MobileNetwork;

namespace ClubPenguin.UI
{
	public class AdventureButtonDisabler : MainNavButtonDisabler
	{
		public TutorialTooltipButton tooltipButton;

		public SceneDefinition[] ShowTooltipInScene;

		private bool isInTooltipScene;

		private void Awake()
		{
			tooltipButton.gameObject.SetActive(false);
			string currentScene = Service.Get<SceneTransitionService>().CurrentScene;
			int num = 0;
			while (true)
			{
				if (num < ShowTooltipInScene.Length)
				{
					if (currentScene == ShowTooltipInScene[num].SceneName)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			isInTooltipScene = true;
		}

		public override void DisableElement(bool hide)
		{
			base.DisableElement(hide);
			if (isInTooltipScene)
			{
				tooltipButton.gameObject.SetActive(true);
			}
		}

		public override void EnableElement()
		{
			base.EnableElement();
			if (isInTooltipScene)
			{
				tooltipButton.gameObject.SetActive(false);
			}
		}
	}
}
