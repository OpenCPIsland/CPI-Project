using MinigameFramework;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_JellyDropZone : mg_bc_DropZone
	{
		private int m_wrongBagsUnloaded;

		private GameObject[] m_bags;

		internal mg_bc_JellyTruck Truck;

		private GameObject m_wrongBag;

		private SpriteRenderer m_wrongBagTint;

		private SpriteRenderer m_wrongBagLines;

		public override void Awake()
		{
			m_bags = new GameObject[60];
			m_wrongBagsUnloaded = 0;
			int num = 0;
			Transform transform = base.transform.Find("mg_bc_jelly_stack_01");
			for (int i = 0; i < transform.childCount; i++)
			{
				m_bags[num] = transform.GetChild(i).gameObject;
				num++;
			}
			transform = base.transform.Find("mg_bc_jelly_stack_02");
			for (int i = 0; i < transform.childCount; i++)
			{
				m_bags[num] = transform.GetChild(i).gameObject;
				num++;
			}
			base.Awake();
		}

		protected override void AddBag(mg_bc_Bag _bag)
		{
			mg_bc_JellyBag component = _bag.GetComponent<mg_bc_JellyBag>();
			mg_bc_EJellyColors color = component.Color;
			if (Truck.IsTargetColor(color))
			{
				base.AddBag(_bag);
				GameObject gameObject = m_bags[m_storedBags - 1].transform.GetChild(0).gameObject;
				gameObject.GetComponent<SpriteRenderer>().color = mg_bc_Constants.GetColorForJelly(color);
				if (m_wrongBag != null && m_wrongBagLines != null)
				{
					Color color2 = m_wrongBagLines.color;
					color2.a = 1f;
					m_wrongBagLines.color = color2;
				}
				m_wrongBag = null;
				m_wrongBagTint = null;
				m_wrongBagLines = null;
			}
			else
			{
				mg_bc_ScoreController.Instance.OnBagUnloaded(false);
				MinigameManager.GetActive().PlaySFX("mg_bc_sfx_BagUnloadError");
				if (m_wrongBagsUnloaded % 3 == 0)
				{
					MinigameManager.GetActive<mg_BeanCounter>().GameLogic.ShowHint("Throw back\nwrong bags!", 2f);
				}
				m_wrongBagsUnloaded++;
				m_wrongBag = m_bags[m_storedBags];
				m_wrongBagTint = m_wrongBag.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
				m_wrongBagLines = m_wrongBag.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
				m_wrongBagTint.color = mg_bc_Constants.GetColorForJelly(color);
				m_wrongBag.SetActive(true);
			}
		}

		protected override void SetBags(int _bags)
		{
			base.SetBags(_bags);
			int num = 0;
			GameObject[] bags = m_bags;
			foreach (GameObject gameObject in bags)
			{
				gameObject.SetActive(num < _bags);
				num++;
			}
		}

		internal override void DropUpdate(float _delta)
		{
			if (m_wrongBag != null)
			{
				Color color = m_wrongBagTint.color;
				color.a -= _delta;
				m_wrongBagTint.color = color;
				Color color2 = m_wrongBagLines.color;
				color2.a = color.a;
				m_wrongBagLines.color = color2;
				if (color.a <= 0f)
				{
					m_wrongBag.SetActive(false);
					m_wrongBag = null;
					m_wrongBagTint = null;
					color2.a = 1f;
					m_wrongBagLines.color = color2;
					m_wrongBagLines = null;
				}
			}
		}
	}
}
