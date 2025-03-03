using MinigameFramework;
using System;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_IceBox : MonoBehaviour
	{
		private GameObject[] m_fish;

		private void Start()
		{
			m_fish = new GameObject[60];
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				string name = renderer.gameObject.name;
				if (name.StartsWith("mg_if_FishHeap_"))
				{
					m_fish[Convert.ToInt32(name.Substring(name.Length - 2)) - 1] = renderer.gameObject;
					renderer.gameObject.SetActive(false);
				}
			}
		}

		public void OnFishCaught(int p_fishCount)
		{
			MinigameManager.GetActive().PlaySFX("mg_if_sfx_FishIceBox");
			GameObject gameObject = m_fish[p_fishCount - 1];
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}
}
