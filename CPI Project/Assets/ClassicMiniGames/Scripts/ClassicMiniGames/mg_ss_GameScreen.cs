using MinigameFramework;
using SmoothieSmash;
using UnityEngine;

public class mg_ss_GameScreen : MinigameScreen
{
	protected mg_ss_GameLogic m_logic;

	protected GameObject m_gameSpecific;

	private GameObject m_gameGeneric;

	public mg_ss_PlayerObject PlayerObject
	{
		get;
		private set;
	}

	public mg_ss_ConveyorObject ConveyorObject
	{
		get;
		private set;
	}

	public mg_ss_SplatterObject SplatterObject
	{
		get;
		private set;
	}

	public mg_ss_ItemGenerator ItemGenerator
	{
		get;
		private set;
	}

	public Transform GameZoneLeft
	{
		get;
		private set;
	}

	public Transform GameZoneRight
	{
		get;
		private set;
	}

	public Transform BlobSplatterFinish
	{
		get;
		protected set;
	}

	protected override void Awake()
	{
		base.Awake();
		m_gameSpecific = m_logic.Minigame.Resources.GetInstancedResource(mg_ss_EResourceList.GAME_SPECIFIC);
		MinigameSpriteHelper.AssignParentTransform(m_gameSpecific, m_logic.Minigame.transform);
		m_gameGeneric = m_logic.Minigame.Resources.GetInstancedResource(mg_ss_EResourceList.GAME_GENERIC);
		MinigameSpriteHelper.AssignParentTransform(m_gameGeneric, m_gameSpecific.transform);
		PlayerObject = m_gameGeneric.GetComponentInChildren<mg_ss_PlayerObject>();
		ConveyorObject = m_gameGeneric.GetComponentInChildren<mg_ss_ConveyorObject>();
		SplatterObject = m_gameGeneric.GetComponentInChildren<mg_ss_SplatterObject>();
		GameZoneLeft = m_gameGeneric.transform.Find("game_zone/mg_ss_zone_left");
		GameZoneRight = m_gameGeneric.transform.Find("game_zone/mg_ss_zone_right");
		ItemGenerator = m_gameSpecific.GetComponentInChildren<mg_ss_ItemGenerator>();
	}

	protected virtual void Start()
	{
		MinigameManager.GetActive<mg_SmoothieSmash>().SetLogic(m_logic, this);
	}

	protected void OnDestroy()
	{
		Object.Destroy(m_gameSpecific);
		Object.Destroy(m_gameGeneric);
		m_logic = null;
	}
}
