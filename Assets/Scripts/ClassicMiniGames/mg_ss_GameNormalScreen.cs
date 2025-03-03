using SmoothieSmash;

public class mg_ss_GameNormalScreen : mg_ss_GameScreen
{
	public mg_ss_OrderSystemObject OrderSystem
	{
		get;
		private set;
	}

	protected override void Awake()
	{
		m_logic = new mg_ss_GameLogic_Normal();
		base.Awake();
		OrderSystem = m_gameSpecific.GetComponentInChildren<mg_ss_OrderSystemObject>();
		base.BlobSplatterFinish = OrderSystem.transform.Find("mg_ss_blob_finish");
	}
}
