using ClubPenguin.Adventure;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using TMPro;

namespace ClubPenguin
{
    public class LoadingSnippets : MonoBehaviour
    {
        public const float RETRY_TIME_SECONDS = 0.1f;

        public float DisplaySeconds = 1f;

        public DialogList Snippets;

        private TMP_Text snippetText;

        private float snippetChooseTime;

        public void Awake()
        {
            snippetText = GetComponent<TMP_Text>();

            if (snippetText == null)
            {
                snippetText = gameObject.AddComponent<TextMeshProUGUI>();
            }
        }

        public void OnEnable()
        {
            chooseSnippet();
        }

        public void Update()
        {
            if (Time.time > snippetChooseTime)
            {
                chooseSnippet();
            }
        }

        private void chooseSnippet()
        {
            if (Service.IsSet<Localizer>())
            {
                snippetText.text = Service.Get<Localizer>().GetTokenTranslation(Snippets.SelectRandom().ContentToken);
                snippetChooseTime = Time.time + DisplaySeconds;
            }
            else
            {
                snippetChooseTime = Time.time + RETRY_TIME_SECONDS;
            }
        }
    }
}