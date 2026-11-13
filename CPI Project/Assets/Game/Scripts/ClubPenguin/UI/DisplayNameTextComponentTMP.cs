using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using TMPro;
using UnityEngine;

namespace ClubPenguin.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DisplayNameTextComponentTMP : MonoBehaviour
    {
        public string Token;

        public int MaxNameLength;

        private TextMeshProUGUI nameText;

        private void OnEnable()
        {
            CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
            if (cPDataEntityCollection == null)
            {
                return;
            }

            nameText = GetComponent<TextMeshProUGUI>();

            DisplayNameData component;
            if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
            {
                string text = component.DisplayName;

                if (MaxNameLength > 0 && text.Length > MaxNameLength)
                {
                    text = text.Substring(0, MaxNameLength);
                }

                if (!string.IsNullOrEmpty(Token))
                {
                    string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(Token);
                    text = string.Format(tokenTranslation, text);
                }

                nameText.text = text;
            }
        }
    }
}