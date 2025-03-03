using MinigameFramework;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_ToppingHolderObject : MonoBehaviour
	{
		private GameObject m_resource;

		protected string m_grabbedTagSFX;

		private Vector2 m_originalPos;

		public mg_pt_EToppingType ToppingType
		{
			get;
			private set;
		}

		public string HeldTagSFX
		{
			get;
			private set;
		}

		public virtual bool IsSauce
		{
			get
			{
				return false;
			}
		}

		public virtual void Initialize(GameObject p_resource, mg_pt_EToppingType p_toppingType, string p_grabbedTagSFX, string p_heldedTagSFX)
		{
			m_grabbedTagSFX = p_grabbedTagSFX;
			HeldTagSFX = p_heldedTagSFX;
			m_originalPos = base.transform.localPosition;
			ToppingType = p_toppingType;
			m_resource = p_resource;
			Vector2 v = m_resource.transform.localPosition;
			MinigameSpriteHelper.AssignParent(m_resource, base.gameObject);
			m_resource.transform.localPosition = v;
			if (p_toppingType >= mg_pt_EToppingType.MIN_TOPPINGS && p_toppingType < mg_pt_EToppingType.MAX_TOPPINGS)
			{
				Hide();
			}
		}

		public virtual void OnGrabbed()
		{
			if (string.IsNullOrEmpty(m_grabbedTagSFX))
			{
				MinigameManager.GetActive().PlaySFX("mg_pt_sfx_topping_grab_0" + Random.Range(1, 3));
			}
			else
			{
				MinigameManager.GetActive().PlaySFX(m_grabbedTagSFX);
			}
		}

		public bool Clicked(Vector2 p_point)
		{
			return base.gameObject.activeInHierarchy && m_resource.GetComponent<BoxCollider>().bounds.Contains(p_point);
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
			Vector2 v = base.transform.position;
			v.x = MinigameSpriteHelper.GetScreenEdge(EScreenEdge.RIGHT, MinigameManager.GetActive().MainCamera).x;
			base.transform.position = v;
		}

		public void Show()
		{
			base.gameObject.SetActive(true);
			float duration = Mathf.Max(0.5f, (base.transform.localPosition.x - m_originalPos.x) / 3.8f);
			TweenPosition.Begin(base.gameObject, duration, m_originalPos).ignoreTimeScale = false;
		}
	}
}
