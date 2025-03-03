using ClubPenguin.NativeWebViewer;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class LegalTextController : MonoBehaviour
	{
		private WebViewController webView;

		public TextWithEvents LegalText;

		public Toggle LegalCheckBox;

		public Image PlaceHolderImage;

		private LegalCheckboxesValidator checkboxesValidator;

		private bool updateLinks;

		private void Start()
		{
			webView = new WebViewController(base.gameObject);
		}

		private void Update()
		{
			if (updateLinks && LegalText.linksDefined)
			{
				initLinks();
			}
		}

		private string enforceATSPolicy(string legalText)
		{
			legalText = legalText.Replace("http://", "https://");
			legalText = legalText.Replace("https://www.disney.sg", "http://www.disney.sg");
			return legalText;
		}

		public void SetLegalText(string legalText, LegalCheckboxesValidator validator)
		{
			legalText = enforceATSPolicy(legalText);
			checkboxesValidator = validator;
			LegalText.text = legalText;
			updateLinks = true;
			LegalCheckBox.gameObject.SetActive(false);
			PlaceHolderImage.gameObject.SetActive(false);
		}

		public Toggle SetLegalTextWithCheckBox(string legalText, string legalID, LegalCheckboxesValidator validator)
		{
			legalText = enforceATSPolicy(legalText);
			checkboxesValidator = validator;
			LegalText.text = legalText;
			updateLinks = true;
			LegalCheckBox.gameObject.SetActive(true);
			PlaceHolderImage.gameObject.SetActive(false);
			LegalCheckBox.name = legalID;
			return LegalCheckBox;
		}

		public void onCheckBoxChanged()
		{
			checkboxesValidator.ResetValidation();
		}

		private void initLinks()
		{
			if (LegalText.links.Count > 0)
			{
				LegalText.AddButtons();
				foreach (string key in LegalText.links.Keys)
				{
					int num = 1;
					Transform transform;
					while ((transform = LegalText.transform.Find(key + num)) != null)
					{
						string[] linkInfo = LegalText.links[key];
						Button component = transform.GetComponent<Button>();
						component.onClick.AddListener(delegate
						{
							onLinkClicked(linkInfo);
						});
						num++;
					}
				}
			}
			updateLinks = false;
		}

		private void onLinkClicked(string[] linkInfo)
		{
			webView.Show(linkInfo[0], null, linkInfo[1]);
		}
	}
}
