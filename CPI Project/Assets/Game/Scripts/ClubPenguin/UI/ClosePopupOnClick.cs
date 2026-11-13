using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class ClosePopupOnClick : MonoBehaviour
	{
		public AnimatedPopup Popup;

		public bool CloseImmediate = false;

		private Button buttonReference;

		private void Start()
		{
			buttonReference = GetComponent<Button>();
			buttonReference.onClick.AddListener(onClick);
			buttonReference.gameObject.AddComponent<BackButton>();
		}

		private void onClick()
		{
			Popup.ClosePopup(CloseImmediate);
		}

		public void OnDestroy()
		{
			if (buttonReference != null)
			{
				buttonReference.onClick.RemoveListener(onClick);
			}
		}
	}
}
