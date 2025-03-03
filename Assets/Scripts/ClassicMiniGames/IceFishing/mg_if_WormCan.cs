using System;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_WormCan : MonoBehaviour
	{
		private GameObject[] m_worms;

		public void Awake()
		{
			m_worms = new GameObject[3];
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				string name = renderer.gameObject.name;
				if (name.StartsWith("mg_if_Can_Worm_"))
				{
					m_worms[Convert.ToInt32(name.Substring(name.Length - 2)) - 1] = renderer.gameObject;
					renderer.gameObject.SetActive(false);
				}
			}
		}

		public void UpdateWorms(int p_lives)
		{
			for (int i = 0; i < m_worms.Length; i++)
			{
				m_worms[i].SetActive(i < p_lives - 1);
			}
		}
	}
}
