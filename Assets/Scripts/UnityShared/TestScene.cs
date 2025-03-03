using Disney.LaunchPadFramework;
using Disney.LaunchPadFramework.Utility.DesignPatterns;
using UnityEngine;

public class TestScene : MonoBehaviour
{
	public GameObject PooledLifeTimeObject = null;

	private GameObjectPool m_pool = null;

	private void Start()
	{
		Singleton<GameObjectPoolManager>.Instance.UpdatePoolMapping();
		m_pool = Singleton<GameObjectPoolManager>.Instance.GetPool(PooledLifeTimeObject);
		if (m_pool != null)
		{
			GameObject gameObject = m_pool.Spawn(base.gameObject.transform, 10f);
			m_pool.Spawn(base.gameObject.transform, 5f);
			m_pool.Spawn(base.gameObject.transform, 2f);
			m_pool.Spawn(base.gameObject.transform, 8f);
		}
	}
}
