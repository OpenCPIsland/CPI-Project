using DevonLocalization.Core;
using Disney.MobileNetwork;
using TMPro;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class GitCommitTextDisplay : MonoBehaviour
	{
		public const string Token = "GlobalUI.Settings.Settings.GitCommitText";

		public const string ResourcePath = "GitCommit";

		private void Start()
		{
			TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
			string hash = (asset != null) ? asset.text.Trim() : null;
			if (string.IsNullOrEmpty(hash))
			{
				base.gameObject.SetActive(false);
				return;
			}
			string translation = Service.Get<Localizer>().GetTokenTranslation(Token);
			if (string.IsNullOrEmpty(translation) || translation == Token)
			{
				translation = "Git Commit: {0}";
			}
			GetComponent<TextMeshProUGUI>().text = string.Format(translation, hash);
		}
	}
}
