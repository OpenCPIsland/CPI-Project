using IceFishing;
using MinigameFramework;
using UnityEngine;

public class mg_if_GameScreen : MinigameScreen
{
	private GameObject m_gameObject;

	protected override void Awake()
	{
		base.Awake();
		mg_IceFishing active = MinigameManager.GetActive<mg_IceFishing>();
		m_gameObject = active.Resources.GetInstancedResource(mg_if_EResourceList.GAME_LOGIC);
		MinigameSpriteHelper.AssignParentTransform(m_gameObject, active.transform);
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		Object.Destroy(m_gameObject);
	}
}
