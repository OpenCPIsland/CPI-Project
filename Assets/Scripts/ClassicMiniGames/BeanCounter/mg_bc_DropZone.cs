using MinigameFramework;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_DropZone : MonoBehaviour
	{
		private GameObject m_arrowOn;

		private GameObject m_arrowOff;

		protected mg_bc_Penguin m_penguin;

		protected int m_storedBags = 0;

		public int StoredBags
		{
			get
			{
				return m_storedBags;
			}
		}

		public virtual void Awake()
		{
			m_arrowOn = base.transform.Find("mg_bc_dropzone_on").gameObject;
			m_arrowOff = base.transform.Find("mg_bc_dropzone_off").gameObject;
			m_arrowOn.SetActive(false);
			m_arrowOff.SetActive(true);
			ClearBags();
		}

		private void OnTriggerEnter2D(Collider2D _collision)
		{
			mg_bc_Penguin component = _collision.gameObject.GetComponent<mg_bc_Penguin>();
			if (component != null)
			{
				m_penguin = component;
				m_arrowOn.SetActive(true);
				m_arrowOff.SetActive(false);
			}
		}

		private void OnTriggerExit2D(Collider2D _collision)
		{
			mg_bc_Penguin component = _collision.gameObject.GetComponent<mg_bc_Penguin>();
			if (component != null)
			{
				m_penguin = null;
				m_arrowOn.SetActive(false);
				m_arrowOff.SetActive(true);
			}
		}

		internal void TakeBag()
		{
			if (m_penguin != null)
			{
				mg_bc_Bag mg_bc_Bag = m_penguin.RemoveBag();
				if (mg_bc_Bag != null)
				{
					AddBag(mg_bc_Bag);
					mg_bc_Bag.Destroy();
				}
			}
		}

		protected virtual void AddBag(mg_bc_Bag _bag)
		{
			SetBags(m_storedBags + 1);
			mg_bc_ScoreController.Instance.OnBagUnloaded();
			MinigameManager.GetActive().PlaySFX("mg_bc_sfx_BagUnload");
		}

		protected virtual void SetBags(int _bags)
		{
			m_storedBags = _bags;
		}

		internal void ClearBags()
		{
			SetBags(0);
		}

		internal virtual void DropUpdate(float _delta)
		{
		}
	}
}
