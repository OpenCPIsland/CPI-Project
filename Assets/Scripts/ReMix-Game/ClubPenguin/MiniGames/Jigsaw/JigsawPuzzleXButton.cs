using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.MiniGames.Jigsaw
{
	[RequireComponent(typeof(Button))]
	public class JigsawPuzzleXButton : MonoBehaviour
	{
		private Button button;

		private EventDispatcher dispatcher;

		private void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(onButton);
			dispatcher = Service.Get<EventDispatcher>();
		}

		private void onButton()
		{
			dispatcher.DispatchEvent(default(JigsawEvents.CloseButton));
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(onButton);
		}
	}
}
