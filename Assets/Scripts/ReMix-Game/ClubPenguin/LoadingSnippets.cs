using ClubPenguin.Adventure;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Text))]
	public class LoadingSnippets : MonoBehaviour
	{
		public const float RETRY_TIME_SECONDS = 0.1f;

		public float DisplaySeconds = 1f;

		public DialogList Snippets;

		private Text snippetText;

		private float snippetChooseTime;

		public void Awake()
		{
			snippetText = GetComponent<Text>();
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
				snippetChooseTime = Time.time + 0.1f;
			}
		}
	}
}
