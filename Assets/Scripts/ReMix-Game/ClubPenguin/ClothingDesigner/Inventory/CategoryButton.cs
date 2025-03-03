using ClubPenguin.Gui;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	[RequireComponent(typeof(TintToggleGroupButton))]
	public class CategoryButton : MonoBehaviour
	{
		[SerializeField]
		private Text buttonText;

		private EquipmentCategoryDefinitionContentKey categoryDefinitionKey;

		private CategoryManagerEventProxy eventProxy;

		public void Init(EquipmentCategoryDefinitionContentKey categoryDefinitionKey, CategoryManagerEventProxy eventProxy)
		{
			this.categoryDefinitionKey = categoryDefinitionKey;
			this.eventProxy = eventProxy;
			CoroutineRunner.Start(loadCategoryDefinition(), this, "setupModelRenderer");
		}

		private IEnumerator loadCategoryDefinition()
		{
			AssetRequest<EquipmentCategoryDefinition> equipmentCategoryDefinitionAssetRequest = null;
			try
			{
				equipmentCategoryDefinitionAssetRequest = Content.LoadAsync(categoryDefinitionKey);
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "Could not load category definition {0}.", categoryDefinitionKey);
			}
			if (equipmentCategoryDefinitionAssetRequest != null)
			{
				yield return equipmentCategoryDefinitionAssetRequest;
				string categoryToken = equipmentCategoryDefinitionAssetRequest.Asset.DisplayName;
				buttonText.text = Service.Get<Localizer>().GetTokenTranslation(categoryToken);
			}
		}

		public void OnButtonPressed()
		{
			eventProxy.OnButtonPressed(categoryDefinitionKey.Key);
		}
	}
}
