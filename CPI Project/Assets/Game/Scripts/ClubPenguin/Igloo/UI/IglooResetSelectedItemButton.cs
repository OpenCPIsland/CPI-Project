using ClubPenguin.ObjectManipulation;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	[RequireComponent(typeof(Button))]
	public class IglooResetSelectedItemButton : MonoBehaviour
	{
		private Button button;

		private void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(resetSelectedItem);
		}

		private void resetSelectedItem()
		{
			button.onClick.RemoveListener(resetSelectedItem);
			Service.Get<EventDispatcher>().DispatchEvent(default(ObjectManipulationEvents.ResetSelectedItem));
		}

		private void OnDestroy()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(resetSelectedItem);
			}
		}
	}
}
