using ClubPenguin.Classic;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using PuffleRoundup;
using System.Collections.Generic;
using UnityEngine.UI;

public class mg_pr_ResultScreen : MinigameScreen
{
	private float m_gameTime;

	private float m_coinsRound;

	private float m_coinsTotal;

	private float m_bonusTime;

	private int m_caught;

	private bool m_complete;

	private bool m_failed;

	public mg_PuffleRoundup Minigame;

	protected override void Awake()
	{
		base.Awake();
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
		Minigame = MinigameManager.GetActive<mg_PuffleRoundup>();
	}

	private void Start()
	{
		Minigame.transform.Find("mg_pr_GameContainer/mg_pr_pf_GameBG(Clone)/mg_pr_InputController").gameObject.GetComponent<mg_pr_InputHandler>().enabled = false;
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		m_complete = UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_RoundController>().m_complete;
		m_failed = UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_RoundController>().m_failed;
		base.HasPauseButton = false;
		base.LoadUI(propertyList);
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_pr_result_exit")
			{
				button.onClick.AddListener(OnBackClicked);
			}
			if (button.gameObject.name == "mg_pr_result_next")
			{
				button.onClick.AddListener(OnNextClicked);
				button.gameObject.SetActive(!m_failed);
			}
			if (button.gameObject.name == "mg_pr_result_replay")
			{
				button.onClick.AddListener(OnReplayClicked);
				button.gameObject.SetActive(!m_complete);
			}
		}
		ClassicMiniGames.PushBackButtonHandler(OnBackClicked);
		Text[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Text>();
		Text[] array2 = componentsInChildren2;
		foreach (Text text in array2)
		{
			if (text.gameObject.name == "mg_pr_time_remaining_count")
			{
				m_gameTime = UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_UITimerCount>().m_timer;
				text.text = m_gameTime.ToString("n0");
			}
			if (text.gameObject.name == "mg_pr_puffles_caught_count")
			{
				m_caught = UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_UICaughtCount>().m_caught;
				text.text = m_caught.ToString();
			}
			if (text.gameObject.name == "mg_pr_coins_round_count")
			{
				m_coinsRound = UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_RoundController>().m_coinsRound;
				text.text = m_coinsRound.ToString("n0");
			}
			if (text.gameObject.name == "mg_pr_total_coins_count")
			{
				m_coinsTotal = UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_RoundController>().m_coinsTotal;
				text.text = m_coinsTotal.ToString("n0");
			}
			if (text.gameObject.name == "mg_pr_bonus_time_count")
			{
				m_bonusTime = UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_RoundController>().m_bonusTime;
				text.text = m_bonusTime.ToString("n0");
			}
		}
	}

	public override void UnloadUI()
	{
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_pr_result_exit")
			{
				button.onClick.RemoveListener(OnBackClicked);
			}
			if (button.gameObject.name == "mg_pr_result_next")
			{
				button.onClick.RemoveListener(OnNextClicked);
			}
			if (button.gameObject.name == "mg_pr_result_replay")
			{
				button.onClick.RemoveListener(OnReplayClicked);
			}
		}
		ClassicMiniGames.RemoveBackButtonHandler(OnBackClicked);
		base.UnloadUI();
	}

	public void OnBackClicked()
	{
		ClassicMiniGames.RemoveBackButtonHandler(OnBackClicked);
		UIManager.Instance.PopScreen();
		MinigameManager.Instance.OnMinigameEnded();
	}

	public void OnNextClicked()
	{
		UIManager.Instance.PopScreen();
		UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_RoundController>().NextRound();
		Minigame.GetComponentInChildren<mg_pr_InputHandler>().enabled = true;
	}

	public void OnReplayClicked()
	{
		UIManager.Instance.PopScreen();
		UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_RoundController>().RestartRound();
		Minigame.GetComponentInChildren<mg_pr_InputHandler>().enabled = true;
	}
}
