using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	[RequireComponent(typeof(UIWidget))]
	public class DDLUILocalize : MonoBehaviour
	{
		public string key;

		private void Start()
		{
			if (TextManager.Instance != null)
			{
				Localize(TextManager.Instance.GetCurrentLanguage());
			}
		}

		private void OnEnable()
		{
			TextManager.LanguageChangeEvent += Localize;
		}

		private void OnDisable()
		{
			TextManager.LanguageChangeEvent -= Localize;
		}

		public virtual void Localize(Language lang)
		{
			UIWidget component = GetComponent<UIWidget>();
			UILabel uILabel = component as UILabel;
			UISprite uISprite = component as UISprite;
			if (string.IsNullOrEmpty(key) && uILabel != null)
			{
				key = uILabel.text;
			}
			string text = string.IsNullOrEmpty(key) ? "" : TextManager.Instance.GetString(key);
			if (uILabel != null)
			{
				UIInput uIInput = NGUITools.FindInParents<UIInput>(uILabel.gameObject);
				if (uIInput != null && uIInput.label == uILabel)
				{
					uIInput.defaultText = text;
				}
				else
				{
					uILabel.text = text;
				}
			}
			else if (uISprite != null)
			{
				uISprite.spriteName = text;
				uISprite.MakePixelPerfect();
			}
		}
	}
}
