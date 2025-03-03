using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.MiniGames.BlockAndLock
{
	[RequireComponent(typeof(Button))]
	public class BlockAndLockXButton : MonoBehaviour
	{
		private Button button;

		private EventDispatcher dispatcher;

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
		}

		private void Start()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(onButton);
		}

		private void onButton()
		{
			dispatcher.DispatchEvent(default(BlockAndLockEvents.CloseButton));
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(onButton);
		}
	}
}
