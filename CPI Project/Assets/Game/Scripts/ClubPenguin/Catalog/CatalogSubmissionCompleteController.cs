using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogSubmissionCompleteController : MonoBehaviour
	{
		public GameObject ViewInCatalogButton;

		public AvatarRenderTextureComponent AvatarRenderTextureComponent;

		public Text ItemNameText;

		private DCustomEquipment submittedItem;

		public void EquipSubmittedItem()
		{
			AvatarDetailsData avatarDetailsData = new AvatarDetailsData();
			DCustomEquipment[] outfit = (submittedItem.Parts.Length != 0) ? new DCustomEquipment[1]
			{
				submittedItem
			} : new DCustomEquipment[0];
			avatarDetailsData.Init(outfit);
			AvatarDetailsData component;
			if (!Service.Get<CPDataEntityCollection>().LocalPlayerHandle.IsNull && Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out component))
			{
				avatarDetailsData.BodyColor = component.BodyColor;
			}
			AvatarRenderTextureComponent.RenderAvatar(avatarDetailsData);
		}

		public void Init(DCustomEquipment item)
		{
			submittedItem = item;
			if (ItemNameText != null)
			{
				TemplateDefinition templateDefinition = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>().Values.ToList().First((TemplateDefinition x) => x.Id == item.DefinitionId);
				ItemNameText.text = Service.Get<Localizer>().GetTokenTranslation(templateDefinition.Name);
			}
		}

		public void onViewInCatalogButon()
		{
			Service.Get<ICPSwrveService>().Action("clothing_catalog_challenge", "view_submission");
			CatalogContext.EventBus.DispatchEvent(default(CatalogUIEvents.ViewInCatalog));
		}
	}
}
