using MinigameFramework;
using SmoothieSmash;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_ss_TitleScreen : MinigameScreen, mg_ss_ITitleScreen
{
	private mg_ss_TitleLogic m_logic;

	private GameObject m_title;

	private GameObject m_survivalLogo;

	private Button m_btnPlay;

	private Button m_btnInstructions;

	private Button m_btnBomb;

	protected override void Awake()
	{
		m_logic = new mg_ss_TitleLogic(this);
		base.Awake();
		m_title = m_logic.Minigame.Resources.GetInstancedResource(mg_ss_EResourceList.TITLE_LOGIC);
		MinigameSpriteHelper.AssignParentTransform(m_title, m_logic.Minigame.transform);
	}

	protected void OnDestroy()
	{
		Object.Destroy(m_title);
		m_logic = null;
	}

	public void Start()
	{
		UpdateGameMode();
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
		m_survivalLogo = GameObject.Find("mg_ss_Survival");
		if (m_survivalLogo == null)
		{
			Debug.LogFormat("Failed to find game object named '{0}'.", "mg_ss_Survival");
		}
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			switch (button.gameObject.name)
			{
			case "btn_play":
				m_btnPlay = button;
				break;
			case "btn_instruction":
				m_btnInstructions = button;
				break;
			case "btn_bomb":
				m_btnBomb = button;
				break;
			}
		}
		RegisterUIEvents();
	}

	private void RegisterUIEvents()
	{
		m_btnPlay.onClick.AddListener(OnPlayClicked);
		m_btnInstructions.onClick.AddListener(OnInstructionsClicked);
		m_btnBomb.onClick.AddListener(OnBombClicked);
	}

	public override void UnloadUI()
	{
		DeregisterUIEvents();
		m_btnPlay = null;
		m_btnInstructions = null;
		m_btnBomb = null;
		base.UnloadUI();
	}

	private void DeregisterUIEvents()
	{
		m_btnPlay.onClick.RemoveListener(OnPlayClicked);
		m_btnInstructions.onClick.RemoveListener(OnInstructionsClicked);
		m_btnBomb.onClick.RemoveListener(OnBombClicked);
	}

	public void UpdateGameMode()
	{
		m_survivalLogo.SetActive(m_logic.Minigame.GameMode == mg_ss_EGameMode.SURVIVAL);
	}

	private void OnPlayClicked()
	{
		m_logic.PlayGame();
	}

	private void OnInstructionsClicked()
	{
		m_logic.ShowInstructions();
		SetButtonsVisible(false);
	}

	private void OnBombClicked()
	{
		m_logic.SwitchGameModes();
	}

	public override void OnPoppedToTop(string p_prevTopName)
	{
		SetButtonsVisible(true);
		base.OnPoppedToTop(p_prevTopName);
	}

	private void SetButtonsVisible(bool p_visible)
	{
		m_btnPlay.gameObject.SetActive(p_visible);
		m_btnInstructions.gameObject.SetActive(p_visible);
	}
}
