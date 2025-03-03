using ClubPenguin.Classic;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using Pizzatron;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_pt_ResultScreen : MinigameScreen
{
	private GameObject m_background;

	private bool m_canContinue;

	private string m_text;

	private Button m_btnExit;

	private Button m_btnReplay;

	private Button m_btnContinue;

	public bool Restart
	{
		get;
		private set;
	}

	public bool Continue
	{
		get;
		private set;
	}

	protected override void Awake()
	{
		base.Awake();
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
		mg_Pizzatron active = MinigameManager.GetActive<mg_Pizzatron>();
		int ordersCompleted = active.GameLogic.OrdersCompleted;
		Localizer localizer = null;
		if (Service.IsSet<Localizer>())
		{
			localizer = Service.Get<Localizer>();
		}
		mg_pt_EResourceList assetTag;
		if (ordersCompleted <= 5)
		{
			assetTag = mg_pt_EResourceList.RESULT_BG_01;
			m_text = ((localizer == null) ? "Kitchen Chaos!" : localizer.GetTokenTranslation("Activity.MiniGames.KitchenChaos"));
		}
		else if (ordersCompleted <= 30)
		{
			assetTag = mg_pt_EResourceList.RESULT_BG_02;
			m_text = ((localizer == null) ? "On Special!" : localizer.GetTokenTranslation("Activity.MiniGames.OnSpecial"));
		}
		else
		{
			assetTag = mg_pt_EResourceList.RESULT_BG_03;
			m_text = ((localizer == null) ? "Dish of the Day!" : localizer.GetTokenTranslation("Activity.MiniGames.DishoftheDay"));
		}
		m_canContinue = (ordersCompleted >= 40 && active.GameLogic.Lives > 0);
		m_background = active.Resources.GetInstancedResource(assetTag);
		MinigameSpriteHelper.AssignParentTransform(m_background, active.transform);
	}

	protected void Start()
	{
		m_background.transform.Find("tint").GetComponent<SpriteRenderer>().color = MinigameManager.Instance.GetPenguinColor();
		MinigameSpriteHelper.FitSpriteToScreen(MinigameManager.GetActive().MainCamera, m_background);
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.HasPauseButton = false;
		base.LoadUI(propertyList);
		Button[] componentsInChildren = GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			switch (button.gameObject.name)
			{
			case "btn_exit":
				m_btnExit = button;
				break;
			case "btn_replay":
				m_btnReplay = button;
				break;
			case "btn_continue":
				m_btnContinue = button;
				m_btnContinue.gameObject.SetActive(m_canContinue);
				break;
			}
		}
		GetComponentInChildren<Text>().text = m_text;
		AddEvents();
	}

	public override void UnloadUI()
	{
		RemoveEvents();
		m_btnExit = null;
		m_btnReplay = null;
		m_btnContinue = null;
		base.UnloadUI();
		Object.Destroy(m_background);
		m_background = null;
	}

	private void AddEvents()
	{
		m_btnExit.onClick.AddListener(OnExitClicked);
		m_btnReplay.onClick.AddListener(OnReplayClicked);
		m_btnContinue.onClick.AddListener(OnContinueClicked);
		ClassicMiniGames.PushBackButtonHandler(OnExitClicked);
	}

	private void RemoveEvents()
	{
		m_btnExit.onClick.RemoveListener(OnExitClicked);
		m_btnReplay.onClick.RemoveListener(OnReplayClicked);
		m_btnContinue.onClick.RemoveListener(OnContinueClicked);
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
	}

	private void OnExitClicked()
	{
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
		Restart = false;
		MinigameManager.Instance.OnMinigameEnded();
	}

	private void OnReplayClicked()
	{
		Restart = true;
		UIManager.Instance.PopScreen();
	}

	private void OnContinueClicked()
	{
		Continue = true;
		UIManager.Instance.PopScreen();
	}
}
