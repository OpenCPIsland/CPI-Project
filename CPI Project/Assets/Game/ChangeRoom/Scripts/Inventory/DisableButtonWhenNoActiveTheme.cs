using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	[RequireComponent(typeof(Button))]
	public class DisableButtonWhenNoActiveTheme : MonoBehaviour
	{
		public Color TintColor;

		public void Awake()
		{
			if (Service.Get<CatalogServiceProxy>().GetActiveThemeScheduleId() == 0)
			{
				GetComponent<Button>().interactable = false;
				Graphic[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Graphic>();
				foreach (Graphic graphic in componentsInChildren)
				{
					graphic.color = TintColor;
				}
			}
		}
	}
}
