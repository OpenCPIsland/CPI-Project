using System;
using System.Globalization;
using MinigameFramework;
using UnityEngine;

public static class ClassicMinigameDoubleCoins
{
	private const string GAME_KEY = "ClassicMiniGames.DailyDoubleCoins.Game";
	private const string DATE_KEY = "ClassicMiniGames.DailyDoubleCoins.Date";

	public static bool ForceAllGames { get; private set; }

	private static readonly string[] GAME_IDS =
	{
		"mg_BeanCounter",
		"mg_IceFishing",
		"mg_JetpackReboot",
		"mg_PuffleRoundup",
		"mg_Pizzatron",
		"mg_SmoothieSmash"
	};

	public static string GetActiveGameId()
	{
		string utcDate = DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
		string savedDate = PlayerPrefs.GetString(DATE_KEY, string.Empty);
		string savedGame = PlayerPrefs.GetString(GAME_KEY, string.Empty);
		if (savedDate == utcDate && Array.IndexOf(GAME_IDS, savedGame) >= 0)
		{
			return savedGame;
		}

		string gameId = GAME_IDS[UnityEngine.Random.Range(0, GAME_IDS.Length)];
		PlayerPrefs.SetString(DATE_KEY, utcDate);
		PlayerPrefs.SetString(GAME_KEY, gameId);
		PlayerPrefs.Save();
		return gameId;
	}

	public static bool IsActive(Minigame minigame)
	{
		return minigame != null && (ForceAllGames || minigame.GetType().Name == GetActiveGameId());
	}

	public static void SetForceAllGames(bool forceAllGames)
	{
		ForceAllGames = forceAllGames;
	}
}

public class mg_DailyDoubleCoins : MonoBehaviour
{
	public bool forceAllGamesDoubleCoins;

	public GameObject beanCounterObject;
	public GameObject iceFishingObject;
	public GameObject jetpackRebootObject;
	public GameObject puffleRoundupObject;
	public GameObject pizzatronObject;
	public GameObject smoothieSmashObject;

	private DateTime m_nextUtcDay;
	private bool m_previousForceAllGames;

	private void Start()
	{
		m_previousForceAllGames = forceAllGamesDoubleCoins;
		ClassicMinigameDoubleCoins.SetForceAllGames(forceAllGamesDoubleCoins);
		RefreshGameObjects();
	}

	private void Update()
	{
		if (m_previousForceAllGames != forceAllGamesDoubleCoins)
		{
			m_previousForceAllGames = forceAllGamesDoubleCoins;
			ClassicMinigameDoubleCoins.SetForceAllGames(forceAllGamesDoubleCoins);
			RefreshGameObjects();
		}

		if (DateTime.UtcNow >= m_nextUtcDay)
		{
			RefreshGameObjects();
		}
	}

	private void RefreshGameObjects()
	{
		DateTime utcDate = DateTime.UtcNow.Date;
		string activeGameId = ClassicMinigameDoubleCoins.GetActiveGameId();
		bool forceAllGames = ClassicMinigameDoubleCoins.ForceAllGames;
		SetGameObjectActive(beanCounterObject, forceAllGames || activeGameId == "mg_BeanCounter");
		SetGameObjectActive(iceFishingObject, forceAllGames || activeGameId == "mg_IceFishing");
		SetGameObjectActive(jetpackRebootObject, forceAllGames || activeGameId == "mg_JetpackReboot");
		SetGameObjectActive(puffleRoundupObject, forceAllGames || activeGameId == "mg_PuffleRoundup");
		SetGameObjectActive(pizzatronObject, forceAllGames || activeGameId == "mg_Pizzatron");
		SetGameObjectActive(smoothieSmashObject, forceAllGames || activeGameId == "mg_SmoothieSmash");
		m_nextUtcDay = utcDate.AddDays(1);
	}

	private static void SetGameObjectActive(GameObject target, bool isActive)
	{
		if (target != null && target.activeSelf != isActive)
		{
			target.SetActive(isActive);
		}
	}
}
