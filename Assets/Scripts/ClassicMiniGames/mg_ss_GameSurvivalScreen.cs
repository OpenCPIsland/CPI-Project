using SmoothieSmash;

public class mg_ss_GameSurvivalScreen : mg_ss_GameScreen
{
	public mg_ss_HealthBonusManager HealthBonusManager
	{
		get;
		private set;
	}

	protected override void Awake()
	{
		m_logic = new mg_ss_GameLogic_Survival();
		base.Awake();
		HealthBonusManager = m_gameSpecific.GetComponentInChildren<mg_ss_HealthBonusManager>();
	}
}
