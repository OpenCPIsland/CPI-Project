using DisneyMobile.CoreUnitySystems;
using System.Collections.Generic;
using UnityEngine;

namespace MinigameFramework
{
	public class MinigameResources<E>
	{
		private Dictionary<E, GameObject> m_resources = new Dictionary<E, GameObject>();

		public virtual void LoadResources()
		{
		}

		protected void LoadResource(string _resourcePath, E _assetTag)
		{
			if (!m_resources.ContainsKey(_assetTag))
			{
				m_resources.Add(_assetTag, Resources.Load(_resourcePath) as GameObject);
			}
			else
			{
				DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Attempting to register same resource twice");
			}
		}

		public GameObject GetInstancedResource(E _assetTag)
		{
			if (m_resources.ContainsKey(_assetTag))
			{
				return Object.Instantiate(m_resources[_assetTag]);
			}
			return null;
		}

		public void UnloadResource(E _key)
		{
			if (m_resources.ContainsKey(_key))
			{
				m_resources.Remove(_key);
			}
		}

		public void UnloadAllResources()
		{
			m_resources.Clear();
			Resources.UnloadUnusedAssets();
		}
	}
}
