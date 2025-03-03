using ClubPenguin.Input;
using ClubPenguin.UI;
using UnityEngine;

public class MainNavButtonSelector : MonoBehaviour
{
	public enum MainNavButtonNames
	{
		MainNavButton_Profile,
		MainNavButton_Quest,
		MainNavButton_Consumables,
		MainNavButton_MoreOptions,
		MainNavButton_Control
	}

	public MainNavButtonNames MainNavButtonName;

	private void Start()
	{
		SelectNavButton();
	}

	public void SelectNavButton()
	{
		GameObject gameObject = GameObject.Find(MainNavButtonName.ToString());
		if (gameObject != null)
		{
			gameObject.GetComponent<MainNavButton>().OnClick(ButtonClickListener.ClickType.UI);
		}
	}
}
