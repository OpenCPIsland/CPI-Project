using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

public class mg_pr_UIRoundCount : MonoBehaviour
{
	private int _m_round;

	private Text m_label;

	private Localizer localizer;

	public int m_round
	{
		get
		{
			return _m_round;
		}
		set
		{
			_m_round = value;
			string format = (localizer == null) ? "ROUND {0}" : localizer.GetTokenTranslation("Activity.MiniGames.Round");
			m_label.text = string.Format(format, m_round);
		}
	}

	private void Start()
	{
		m_label = GetComponent<Text>();
		if (Service.IsSet<Localizer>())
		{
			localizer = Service.Get<Localizer>();
		}
	}
}
