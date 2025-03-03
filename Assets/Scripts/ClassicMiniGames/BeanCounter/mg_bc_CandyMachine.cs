using System.Collections.Generic;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_CandyMachine : MonoBehaviour
	{
		private Dictionary<mg_bc_EJellyColors, GameObject> m_slices;

		private List<mg_bc_EJellyColors> m_targets;

		private mg_bc_CandyBulb m_bulb;

		private void Awake()
		{
			m_bulb = GetComponentInChildren<mg_bc_CandyBulb>();
			m_slices = new Dictionary<mg_bc_EJellyColors, GameObject>();
			mg_bc_ColorSlice[] componentsInChildren = GetComponentsInChildren<mg_bc_ColorSlice>();
			mg_bc_ColorSlice[] array = componentsInChildren;
			foreach (mg_bc_ColorSlice mg_bc_ColorSlice in array)
			{
				m_slices[mg_bc_ColorSlice.SliceColor] = mg_bc_ColorSlice.gameObject;
				mg_bc_ColorSlice.gameObject.SetActive(false);
			}
			if (m_targets != null)
			{
				SetColors(m_targets);
			}
		}

		internal void SetColors(List<mg_bc_EJellyColors> _targets)
		{
			if (m_slices == null)
			{
				m_targets = _targets;
				return;
			}
			foreach (mg_bc_EJellyColors key in m_slices.Keys)
			{
				m_slices[key].SetActive(_targets.Contains(key));
			}
			m_bulb.SetColors(_targets);
		}
	}
}
