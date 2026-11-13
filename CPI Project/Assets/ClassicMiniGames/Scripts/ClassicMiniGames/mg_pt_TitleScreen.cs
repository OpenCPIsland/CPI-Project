using MinigameFramework;
using Pizzatron;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_pt_TitleScreen : MinigameScreen
{
	private static string NORMAL = "mg_pt_pf_TitleScreen/Background/normal";

	private static string CANDY = "mg_pt_pf_TitleScreen/Background/cookie";

	private static string BTN_PLAY = "btn_play";

	private static string BTN_INSTRUCTIONS = "btn_instruction";

	private static string BTN_LEVER = "mg_pt_lever";

	private static string ANIM_TRIG_TURN_ON = "turn_on";

	private static string ANIM_TRIG_TURN_OFF = "turn_off";

	private static int OFF_STATE = Animator.StringToHash("Base Layer.mg_pt_title_lever_off");

	private static int ON_STATE = Animator.StringToHash("Base Layer.mg_pt_title_lever_on");

	private mg_pt_TitleLogic m_titleLogic;

	private Transform m_normal;

	private Transform m_candy;

	private Button m_btnLever;

	private Animator m_leverAnimator;

	private Button m_btnPlay;

	private Button m_btnInstructions;

	private MinigameSFX m_pizzatron;

	private MinigameSFX m_cookietron;

	protected override void Awake()
	{
		m_titleLogic = new mg_pt_TitleLogic();
		base.Awake();
	}

	protected void OnDestroy()
	{
		m_titleLogic = null;
	}

	protected void Start()
	{
		UpdateVisibility();
		if (m_titleLogic.IsCandy)
		{
			ToggleLever();
		}
	}

	private void ToggleLever()
	{
		if (m_leverAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == OFF_STATE)
		{
			m_leverAnimator.ResetTrigger(ANIM_TRIG_TURN_OFF);
			m_leverAnimator.SetTrigger(ANIM_TRIG_TURN_ON);
		}
		else if (m_leverAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == ON_STATE)
		{
			m_leverAnimator.ResetTrigger(ANIM_TRIG_TURN_ON);
			m_leverAnimator.SetTrigger(ANIM_TRIG_TURN_OFF);
		}
	}

	private void OnLeverClicked()
	{
		ToggleLever();
		SetCandy(!m_titleLogic.IsCandy);
	}

	private void SetCandy(bool p_isCandy)
	{
		m_titleLogic.IsCandy = p_isCandy;
		UpdateVisibility();
		if (p_isCandy)
		{
			m_titleLogic.Minigame.StopSFX(m_pizzatron.Name);
			m_titleLogic.Minigame.PlaySFX(m_cookietron.Name);
		}
		else
		{
			m_titleLogic.Minigame.StopSFX(m_cookietron.Name);
			m_titleLogic.Minigame.PlaySFX(m_pizzatron.Name);
		}
	}

	private void UpdateVisibility()
	{
		m_candy.gameObject.SetActive(m_titleLogic.IsCandy);
		m_normal.gameObject.SetActive(!m_titleLogic.IsCandy);
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		m_normal = base.transform.Find(NORMAL);
		m_candy = base.transform.Find(CANDY);
		base.LoadUI(propertyList);
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == BTN_PLAY)
			{
				m_btnPlay = button;
			}
			else if (button.gameObject.name == BTN_INSTRUCTIONS)
			{
				m_btnInstructions = button;
			}
			else if (button.gameObject.name == BTN_LEVER)
			{
				m_btnLever = button;
			}
		}
		m_leverAnimator = m_btnLever.GetComponentInChildren<Animator>();
		m_pizzatron = m_btnLever.transform.Find("sfx_pizzatron").GetComponent<MinigameSFX>();
		m_cookietron = m_btnLever.transform.Find("sfx_cookietron").GetComponent<MinigameSFX>();
		RegisterEvents();
	}

	public override void UnloadUI()
	{
		DeregisterEvents();
		m_btnPlay = null;
		m_btnInstructions = null;
		base.UnloadUI();
	}

	private void RegisterEvents()
	{
		m_btnPlay.onClick.AddListener(OnPlayClicked);
		m_btnInstructions.onClick.AddListener(OnInstructionsClicked);
		m_btnLever.onClick.AddListener(OnLeverClicked);
	}

	private void DeregisterEvents()
	{
		m_btnPlay.onClick.RemoveListener(OnPlayClicked);
		m_btnInstructions.onClick.RemoveListener(OnInstructionsClicked);
		m_btnLever.onClick.RemoveListener(OnLeverClicked);
	}

	private void OnPlayClicked()
	{
		m_titleLogic.OnPlayClicked();
	}

	private void OnInstructionsClicked()
	{
		m_titleLogic.OnInstructionsClicked();
		SetButtonsVisible(false);
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
		m_btnLever.enabled = p_visible;
	}
}
