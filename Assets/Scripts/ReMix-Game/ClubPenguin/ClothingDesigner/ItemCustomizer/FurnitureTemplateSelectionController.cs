using ClubPenguin.ClothingDesigner.ItemCustomizer.MockData;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class FurnitureTemplateSelectionController : TemplateSelectionController
	{
		[SerializeField]
		private RectTransform IconsContent;

		[SerializeField]
		private GameObject TemplateNavContentPanel;

		[SerializeField]
		private FurnitureTemplateCategory CurrentCategory;

		private FurnitureTemplateCategoryButton[] templateCategoryButtons;

		private Dictionary<FurnitureTemplateCategory, string[]> rawTemplateData;

		public void Start()
		{
			templateCategoryButtons = TemplateNavContentPanel.gameObject.GetComponentsInChildren<FurnitureTemplateCategoryButton>(true);
			SetTemplateCategory(CurrentCategory);
		}

		public void SetTemplateCategory(FurnitureTemplateCategory templateCategory)
		{
			for (int i = 0; i < IconsContent.transform.childCount; i++)
			{
				UnityEngine.Object.Destroy(IconsContent.transform.GetChild(i).gameObject);
			}
			CurrentCategory = templateCategory;
			ActivateTemplateCategoryButton(CurrentCategory);
			if (rawTemplateData == null)
			{
				fetchTemplateData();
			}
			else if (CurrentCategory == FurnitureTemplateCategory.All)
			{
				populateIconsForAllCategories();
			}
			else
			{
				populateIconsForCategory(CurrentCategory);
			}
		}

		private void ActivateTemplateCategoryButton(FurnitureTemplateCategory templateCategory)
		{
			for (int i = 0; i < templateCategoryButtons.Length; i++)
			{
				if (templateCategoryButtons[i].TemplateCategory == templateCategory)
				{
					templateCategoryButtons[i].Activate();
				}
			}
		}

		private void fetchTemplateData()
		{
			onTemplateDataReceived(FurnitureTemplatesMockData.GetMockData());
		}

		private void onTemplateDataReceived(Dictionary<FurnitureTemplateCategory, string[]> rawTemplates)
		{
			rawTemplateData = rawTemplates;
			SetTemplateCategory(CurrentCategory);
		}

		private void populateIconsForAllCategories()
		{
			foreach (FurnitureTemplateCategory value in Enum.GetValues(typeof(FurnitureTemplateCategory)))
			{
				populateIconsForCategory(value);
			}
		}

		private void populateIconsForCategory(FurnitureTemplateCategory category)
		{
			if (category != 0)
			{
				string[] value;
				if (!rawTemplateData.TryGetValue(category, out value))
				{
					Log.LogError(this, "Data does not contain key for current category: " + category);
				}
				for (int i = 0; i < value.Length; i++)
				{
					Texture2DContentKey furnitureIconPath = EquipmentPathUtil.GetFurnitureIconPath(value[i]);
					Content.LoadAsync(onTemplateIconTexLoaded, furnitureIconPath);
				}
			}
		}

		private void onTemplateIconTexLoaded(string key, Texture2D templateIconTex)
		{
		}

		private void onTemplateSelected(string templateName)
		{
		}
	}
}
