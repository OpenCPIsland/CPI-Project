using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	[RequireComponent(typeof(Button))]
	public class CellPhoneHomeButton : MonoBehaviour
	{
		private Button button;

		private void Start()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(onClick);
		}

		private void onClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.ReturnToHomeScreen));
		}

		private void OnDestroy()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(onClick);
			}
		}
	}
}
