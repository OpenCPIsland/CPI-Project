using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ExchangeListItem : MonoBehaviour
	{
		private const string CANNOT_EXCHANGE_HEX_COLOR = "EF424A";

		public Image Icon;

		public Text ItemNameText;

		public Text QuantityEarnedText;

		private ExchangeItem _exchangeItem;

		public ExchangeItem ExchangeItem
		{
			get
			{
				return _exchangeItem;
			}
			set
			{
				_exchangeItem = value;
				_exchangeItem.LocalizedItemName = Service.Get<Localizer>().GetTokenTranslation(value.CollectibleDefinition.NameToken);
				ItemNameText.text = _exchangeItem.LocalizedItemName;
				QuantityEarnedText.text = value.QuantityEarned.ToString();
				if (!value.CanExchange())
				{
					QuantityEarnedText.color = ColorUtils.HexToColor("EF424A");
				}
				Content.LoadAsync(onIconSpriteLoaded, value.CollectibleDefinition.SpriteAsset);
			}
		}

		public void ListItemOutroComplete()
		{
			Object.Destroy(base.gameObject);
		}

		private void onIconSpriteLoaded(string path, Sprite sprite)
		{
			_exchangeItem.ItemSprite = sprite;
			Icon.sprite = sprite;
		}
	}
}
