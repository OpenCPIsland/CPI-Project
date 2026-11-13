using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine.UI;

public class mg_pr_InstructionScreen : MinigameScreen
{
	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.HasPauseButton = true;
		base.LoadUI(propertyList);
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_pr_Back")
			{
				button.onClick.AddListener(OnBackClicked);
			}
			if (button.gameObject.name == "mg_pr_PlayGame")
			{
				button.onClick.AddListener(OnPlayClicked);
			}
		}
	}

	public override void UnloadUI()
	{
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_pr_Back")
			{
				button.onClick.RemoveListener(OnBackClicked);
			}
			if (button.gameObject.name == "mg_pr_PlayGame")
			{
				button.onClick.RemoveListener(OnPlayClicked);
			}
		}
		base.UnloadUI();
	}

	public override void CloseScreen()
	{
		base.CloseScreen();
	}

	public void OnBackClicked()
	{
		UIManager.Instance.PopScreen();
	}

	public void OnPlayClicked()
	{
		UIManager.Instance.PopScreen();
		UIManager.Instance.PopScreen();
		UIManager.Instance.OpenScreen("mg_pr_GameScreen", false, null, null);
	}
}
