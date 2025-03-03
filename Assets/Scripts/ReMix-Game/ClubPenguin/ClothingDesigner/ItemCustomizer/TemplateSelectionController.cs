using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class TemplateSelectionController : MonoBehaviour
	{
		protected DItemCustomization itemModel;

		public void SetModel(DItemCustomization itemModel)
		{
			this.itemModel = itemModel;
		}

		public void Show()
		{
			base.gameObject.SetActive(true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
		}
	}
}
