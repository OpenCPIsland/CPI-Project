using ClubPenguin.Gui;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(TintToggleGroupButton))]
	public class FurnitureTemplateCategoryButton : MonoBehaviour
	{
		[SerializeField]
		private FurnitureTemplateSelectionController templateSelectionController;

		public FurnitureTemplateCategory TemplateCategory;

		private Button button;

		private TintToggleGroupButton tintToggleGroupButton;

		public void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(delegate
			{
				templateSelectionController.SetTemplateCategory(TemplateCategory);
			});
			tintToggleGroupButton = GetComponent<TintToggleGroupButton>();
		}

		public void Activate()
		{
			tintToggleGroupButton.OnClick();
		}

		private void OnDestroy()
		{
			button.onClick.RemoveAllListeners();
		}
	}
}
