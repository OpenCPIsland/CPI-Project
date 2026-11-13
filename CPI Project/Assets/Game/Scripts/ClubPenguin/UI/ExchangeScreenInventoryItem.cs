using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ExchangeScreenInventoryItem : MonoBehaviour
	{
		private static readonly int BoolExchanging = Animator.StringToHash("Exchanging");

		private static readonly Color NotEnoughTextColor = new Color(1f, 0.68f, 0.16f);

		public Image Icon;

		public Text ItemNameText;

		public Text QuantityEarnedText;

		private ExchangeItem _exchangeItem;

		public int CalculatedRowPosition = -1;

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
				Content.LoadAsync(onIconSpriteLoaded, value.CollectibleDefinition.SpriteAsset);
			}
		}

		public void StartExchanging()
		{
			GetComponent<Animator>().SetBool(BoolExchanging, true);
		}

		public void StopExchanging()
		{
			QuantityEarnedText.text = "0";
			GetComponent<Animator>().SetBool(BoolExchanging, false);
		}

		private void onIconSpriteLoaded(string path, Sprite sprite)
		{
			_exchangeItem.ItemSprite = sprite;
			Icon.sprite = sprite;
		}
	}
}
