using ClubPenguin.Analytics;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class CustomizationContext : MonoBehaviour
	{
		[SerializeField]
		private TemplateSelectionController templateSelectionController;

		[SerializeField]
		private TemplateChosenController templateChosenController;

		[SerializeField]
		private PropertyCustomizationController propertyCustomizationController;

		[SerializeField]
		private EquipmentCustomizationController equipmentCustomizationController;

		[SerializeField]
		private CustomizerMode mode;

		public DItemCustomization itemCustomizationModel;

		private static EventDispatcher eventBus;

		private CustomizerModel mainModel;

		public CustomizerState CustomizerState
		{
			get
			{
				return mainModel.State;
			}
		}

		public static EventDispatcher EventBus
		{
			get
			{
				if (eventBus == null)
				{
					eventBus = new EventDispatcher();
				}
				return eventBus;
			}
		}

		public void Init(CustomizerAvatarController customizerAvatarController, Texture2D[] defaultSwatchTextures, string categoryToSelect)
		{
			itemCustomizationModel = new DItemCustomization();
			itemCustomizationModel.SetDefaultTextures(defaultSwatchTextures);
			ClothingDesignerOutliner component = customizerAvatarController.GetComponent<ClothingDesignerOutliner>();
			propertyCustomizationController.SetOutliner(component);
			mainModel = new CustomizerModel(itemCustomizationModel);
			propertyCustomizationController.SetModel(mainModel);
			customizerAvatarController.SetModel(mainModel);
			templateSelectionController.SetModel(itemCustomizationModel);
			templateChosenController.SetModel(itemCustomizationModel);
			if (mode == CustomizerMode.CLOTHING)
			{
				equipmentCustomizationController.SetDependancies(templateSelectionController, templateChosenController, propertyCustomizationController, mainModel);
				EventBus.DispatchEvent(new CustomizerActiveSwatchEvents.SetIsFabric(true));
				(templateSelectionController as ClothingTemplateSelectionController).Init(categoryToSelect);
			}
			else if (mode == CustomizerMode.FURNITURE)
			{
				equipmentCustomizationController.SetDependancies(templateSelectionController, templateChosenController, propertyCustomizationController, mainModel);
			}
			equipmentCustomizationController.Init();
			Service.Get<ICPSwrveService>().StartTimer("designer_time", "my_style.designer");
		}

		private void OnDestroy()
		{
			Service.Get<ICPSwrveService>().EndTimer("designer_time");
		}
	}
}
