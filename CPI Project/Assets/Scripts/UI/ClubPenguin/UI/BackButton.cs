using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class BackButton : MonoBehaviour
	{
		private Button button;

		private void Awake()
		{
			button = GetComponent<Button>();
		}

		private void OnEnable()
		{
			addListeners();
		}

		private void OnDisable()
		{
			removeListeners();
		}

		protected virtual void addListeners()
		{
			Service.Get<BackButtonController>().Add(onBackButtonClicked);
		}

		protected virtual void removeListeners()
		{
			Service.Get<BackButtonController>().Remove(onBackButtonClicked);
		}

		protected void onBackButtonClicked()
		{
			if (button.IsInteractable() && button.enabled)
			{
				button.onClick.Invoke();
			}
			else
			{
				addListeners();
			}
		}
	}
}
