using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

public class HarnessInitializer : MonoBehaviour
{
	private static GameObject m_minigameManager;

	private static GameObject m_inputManager;

	public void Start()
	{
		if (m_minigameManager == null)
		{
			m_minigameManager = new GameObject("MinigameManager");
			m_minigameManager.AddComponent<MinigameManager>();
		}
		if (m_inputManager == null)
		{
			m_inputManager = new GameObject("InputManager");
			m_inputManager.AddComponent<InputManager>();
		}
		Resources.UnloadUnusedAssets();
	}
}
