using MinigameFramework;
using Pizzatron;
using UnityEngine;

public class mg_pt_GameScreen : MinigameScreen
{
	private mg_pt_GameLogic m_logic;

	private GameObject m_game;

	private Transform m_pizzaSpawnPoints;

	public mg_pt_ConveyorObject ConveyorObject
	{
		get
		{
			return m_game.GetComponentInChildren<mg_pt_ConveyorObject>();
		}
	}

	public mg_pt_ToppingBar ToppingBar
	{
		get
		{
			return m_game.GetComponentInChildren<mg_pt_ToppingBar>();
		}
	}

	public Transform GameTransform
	{
		get
		{
			return m_game.transform;
		}
	}

	public Transform PizzaSpawnStart
	{
		get
		{
			return m_pizzaSpawnPoints.Find("start");
		}
	}

	public Transform PizzaSpawnEnd
	{
		get
		{
			return m_pizzaSpawnPoints.Find("end");
		}
	}

	public Transform PizzaSpeedPoint
	{
		get
		{
			return m_pizzaSpawnPoints.Find("speed");
		}
	}

	public mg_pt_PenguinManager PenguinManager
	{
		get
		{
			return m_game.GetComponentInChildren<mg_pt_PenguinManager>();
		}
	}

	public mg_pt_BoardObject BoardObject
	{
		get
		{
			return m_game.GetComponentInChildren<mg_pt_BoardObject>();
		}
	}

	protected override void Awake()
	{
		m_logic = new mg_pt_GameLogic();
		base.Awake();
		m_game = m_logic.Minigame.Resources.GetInstancedResource(mg_pt_EResourceList.GAME_GENERIC);
		MinigameSpriteHelper.AssignParentTransform(m_game, m_logic.Minigame.transform);
		m_pizzaSpawnPoints = m_game.transform.Find("pizza_spawn_points");
	}

	protected void Start()
	{
		m_game.transform.localScale = MinigameSpriteHelper.CalculateScaleToFitScreen(m_logic.Minigame.MainCamera, new Vector2(1364f, 768f) / 100f);
		m_logic.Initialize(this);
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		Object.Destroy(m_game);
		m_game = null;
	}
}
