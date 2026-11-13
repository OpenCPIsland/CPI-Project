using DevonLocalization.Core;
using Disney.MobileNetwork;
using JetpackReboot;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_jr_SplashScreen : MinigameScreen
{
	private mg_jr_Resources m_resources;

	private List<string> m_tipIds = new List<string>();

	private Text m_gamePlayTip;

	private Image m_progressMask;

	private Image m_progressCircle;

	private float m_spinRate = 25f;

	private Localizer localizer;

	protected override void Awake()
	{
		base.Awake();
		if (Service.IsSet<Localizer>())
		{
			localizer = Service.Get<Localizer>();
		}
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		m_resources = MinigameManager.GetActive<mg_JetpackReboot>().Resources;
		if (localizer != null)
		{
			m_tipIds.Add(localizer.GetTokenTranslation("Activity.MiniGames.JetTip0"));
			m_tipIds.Add(localizer.GetTokenTranslation("Activity.MiniGames.JetTip1"));
			m_tipIds.Add(localizer.GetTokenTranslation("Activity.MiniGames.JetTip2"));
			m_tipIds.Add(localizer.GetTokenTranslation("Activity.MiniGames.JetTip3"));
			m_tipIds.Add(localizer.GetTokenTranslation("Activity.MiniGames.JetTip4"));
			m_tipIds.Add(localizer.GetTokenTranslation("Activity.MiniGames.JetTip5"));
			m_tipIds.Add(localizer.GetTokenTranslation("Activity.MiniGames.JetTip6"));
		}
		else
		{
			m_tipIds.Add("Complete goals to earn extra coins.");
			m_tipIds.Add("Swipe up and down to control your penguin in Turbo Mode.");
			m_tipIds.Add("You are INVINCIBLE in Turbo Mode! Smash through obstacles for bonus coins!");
			m_tipIds.Add("Collect Robot Penguins to protect you from getting hurt.");
			m_tipIds.Add("You can collect up to 3 Robot Penguins at a time.");
			m_tipIds.Add("Gary &apos;s MAGNETRON 3000 has malfunctioned, and now coins are floating everywhere.");
			m_tipIds.Add("Watch for &quot;!&quot; warnings. They tell you something dangerous is coming next.");
		}
		Image[] componentsInChildren = GetComponentsInChildren<Image>();
		Image[] array = componentsInChildren;
		foreach (Image image in array)
		{
			if (image.gameObject.name == "mg_jr_Circle")
			{
				m_progressCircle = image;
			}
			else if (image.gameObject.name == "mg_jr_LoadMask")
			{
				m_progressMask = image;
				m_progressMask.fillAmount = 0f;
			}
		}
		m_gamePlayTip = GetComponentInChildren<Text>();
		m_gamePlayTip.text = RandomTip();
		base.LoadUI(propertyList);
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
	}

	private void Update()
	{
		m_progressCircle.transform.Rotate(new Vector3(0f, 0f, m_spinRate));
		m_progressMask.fillAmount = 1f - m_resources.LoadProgress;
	}

	private string RandomTip()
	{
		if (m_tipIds.Count > 0)
		{
			return m_tipIds[Random.Range(0, m_tipIds.Count)];
		}
		return (localizer != null) ? localizer.GetTokenTranslation("Activity.MiniGames.JetTip0") : "Complete goals to earn extra coins.";
	}
}
