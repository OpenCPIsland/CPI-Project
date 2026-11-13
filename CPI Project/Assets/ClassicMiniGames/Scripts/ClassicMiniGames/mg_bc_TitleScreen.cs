using BeanCounter;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

public class mg_bc_TitleScreen : MinigameScreen
{
	private static int RUNNING_STATE = Animator.StringToHash("Base Layer.mg_bc_candybag_anim");

	private GameObject m_candyUI;

	private GameObject m_normalUI;

	private Button m_btnPlay;

	private Button m_btnInstruction;

	private Button m_btnNormal;

	private Button m_btnHard;

	private Button m_btnExtreme;

	private Button m_btnEasterEgg;

	private Animator m_easterEggAnimator;

	private bool m_isCandy = false;

	protected override void Awake()
	{
		MinigameManager.GetActive<mg_BeanCounter>().RestartMusic("mg_bc_main_music");
		base.Awake();
	}

	private void Start()
	{
		base.transform.Find("mg_bc_pf_TitleScreen/penguin/mg_bc_title_penguin_tint").GetComponent<Image>().color = MinigameManager.Instance.GetPenguinColor();
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_bc_PlayButton")
			{
				m_btnPlay = button;
			}
			else if (button.gameObject.name == "mg_bc_InstructionsButton")
			{
				m_btnInstruction = button;
			}
			else if (button.gameObject.name == "mg_bc_NormalButton")
			{
				m_btnNormal = button;
			}
			else if (button.gameObject.name == "mg_bc_HardButton")
			{
				m_btnHard = button;
			}
			else if (button.gameObject.name == "mg_bc_ExtremeButton")
			{
				m_btnExtreme = button;
			}
			else if (button.gameObject.name == "mg_bc_title_candybag_anim")
			{
				m_btnEasterEgg = button;
			}
		}
		m_btnPlay.onClick.AddListener(OnPlayClicked);
		m_btnInstruction.onClick.AddListener(OnInstructionsClicked);
		m_btnNormal.onClick.AddListener(OnNormalClicked);
		m_btnHard.onClick.AddListener(OnHardClicked);
		m_btnExtreme.onClick.AddListener(OnExtremeClicked);
		m_btnEasterEgg.onClick.AddListener(ToggleCandy);
		m_easterEggAnimator = m_btnEasterEgg.GetComponent<Animator>();
		Transform child = base.gameObject.transform.GetChild(0);
		m_candyUI = child.Find("mg_bc_jellybuttons").gameObject;
		m_normalUI = child.Find("mg_bc_normalbuttons").gameObject;
		UpdateUI();
	}

	private void UpdateUI()
	{
		if (m_isCandy)
		{
			m_easterEggAnimator.ResetTrigger("clear_trigger");
			m_easterEggAnimator.SetTrigger("start_trigger");
			m_candyUI.SetActive(true);
			m_normalUI.SetActive(false);
			MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UISelectJellyBeans");
		}
		else
		{
			m_easterEggAnimator.ResetTrigger("start_trigger");
			m_easterEggAnimator.SetTrigger("clear_trigger");
			m_candyUI.SetActive(false);
			m_normalUI.SetActive(true);
			MinigameManager.GetActive().StopSFX("mg_bc_sfx_UISelectJellyBeans");
		}
	}

	private void ToggleCandy()
	{
		if (m_easterEggAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != RUNNING_STATE)
		{
			m_isCandy = !m_isCandy;
			UpdateUI();
		}
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_btnPlay.onClick.RemoveListener(OnPlayClicked);
		m_btnInstruction.onClick.RemoveListener(OnInstructionsClicked);
		m_btnNormal.onClick.RemoveListener(OnNormalClicked);
		m_btnHard.onClick.RemoveListener(OnHardClicked);
		m_btnExtreme.onClick.RemoveListener(OnExtremeClicked);
		m_btnEasterEgg.onClick.RemoveListener(ToggleCandy);
		m_easterEggAnimator = null;
	}

	public void OnInstructionsClicked()
	{
		mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
		active.GameMode = mg_bc_EGameMode.COFFEE_NORMAL;
		ShowInstructions();
	}

	public void OnPlayClicked()
	{
		mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
		active.GameMode = mg_bc_EGameMode.COFFEE_NORMAL;
		UIManager.Instance.PopScreen();
		MinigameManager.GetActive<mg_BeanCounter>().LaunchGame();
	}

	protected override void OnCloseClicked()
	{
		base.OnCloseClicked();
	}

	private void OnExtremeClicked()
	{
		mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
		active.GameMode = mg_bc_EGameMode.JELLY_EXTREME;
		ShowInstructions();
	}

	private void OnHardClicked()
	{
		mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
		active.GameMode = mg_bc_EGameMode.JELLY_HARD;
		ShowInstructions();
	}

	private void OnNormalClicked()
	{
		mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
		active.GameMode = mg_bc_EGameMode.JELLY_NORMAL;
		ShowInstructions();
	}

	private void ShowInstructions()
	{
		UIManager.Instance.OpenScreen("mg_bc_InstructionsScreen", false, null, null);
		SetButtonsVisible(false);
	}

	public override void OnPoppedToTop(string prevTopName)
	{
		SetButtonsVisible(true);
		base.OnPoppedToTop(prevTopName);
	}

	private void SetButtonsVisible(bool p_visible)
	{
		m_btnPlay.gameObject.SetActive(p_visible);
		m_btnInstruction.gameObject.SetActive(p_visible);
		m_btnNormal.gameObject.SetActive(p_visible);
		m_btnHard.gameObject.SetActive(p_visible);
		m_btnExtreme.gameObject.SetActive(p_visible);
		m_btnEasterEgg.enabled = p_visible;
	}
}
