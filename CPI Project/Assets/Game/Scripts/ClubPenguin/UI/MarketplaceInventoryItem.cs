using ClubPenguin.Props;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MarketplaceInventoryItem : MonoBehaviour
	{
		public Image ItemIcon;

		public Text CountText;

		public Text ItemNameText;

		public GameObject ShareIcon;

		private PropDefinition itemDefinition;

		public PropDefinition ItemDefinition
		{
			get
			{
				return itemDefinition;
			}
		}

		public void OnDestroy()
		{
		}

		public void Init(Sprite sprite, PropDefinition propDef, int count)
		{
			CountText.text = count.ToString();
			ItemNameText.text = Service.Get<Localizer>().GetTokenTranslation(propDef.Name);
			if (propDef.Shareable)
			{
				ShareIcon.SetActive(true);
			}
			ItemIcon.sprite = sprite;
			itemDefinition = propDef;
		}

		public void SetCount(int count)
		{
			CountText.text = count.ToString();
		}
	}
}
