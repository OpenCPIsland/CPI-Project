using MinigameFramework;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_DropAllowanceUIScript : mg_bc_UIValueLabel<int>
	{
		private GameObject m_parent;

		public override void Start()
		{
			base.Start();
			m_parent = base.gameObject.transform.parent.gameObject;
			MinigameManager.GetActive<mg_BeanCounter>().GameLogic.Truck.CurrentDropAllowance.SetDisplayer(this);
		}

		public override void SetValue(int _value)
		{
			base.SetValue(_value);
			m_parent.SetActive(_value >= 0);
		}
	}
}
