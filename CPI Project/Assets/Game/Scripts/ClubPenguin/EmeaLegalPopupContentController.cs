using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class EmeaLegalPopupContentController : MonoBehaviour
	{
		public Button ContinueButton;

		public Button CloseButton;

		private void Start()
		{
			ContinueButton.onClick.AddListener(OnContinueButtonClicked);
			CloseButton.onClick.AddListener(OnContinueButtonClicked);
		}

		public void OnDestroy()
		{
			if (ContinueButton != null)
			{
				ContinueButton.onClick.RemoveListener(OnContinueButtonClicked);
			}
			if (CloseButton != null)
			{
				CloseButton.onClick.RemoveListener(OnContinueButtonClicked);
			}
		}

		private void OnEnable()
		{
			ContinueButton.interactable = true;
		}

		private void OnDisable()
		{
			ContinueButton.interactable = false;
		}

		private void OnContinueButtonClicked()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
