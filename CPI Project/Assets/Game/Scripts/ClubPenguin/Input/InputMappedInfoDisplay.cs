using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Input
{
	[RequireComponent(typeof(Text))]
	public class InputMappedInfoDisplay : MonoBehaviour
	{
		[SerializeField]
		private SingleControlInputInfo.Actions action = SingleControlInputInfo.Actions.Jump;

		private Text display;

		private ChatDisplayToggle chatToggle;

		private SingleControlInputInfo inputInfo;

		private void Awake()
		{
			display = GetComponent<Text>();
			chatToggle = GetComponentInParent<ChatDisplayToggle>();
			inputInfo = new SingleControlInputInfo
			{
				ControlAction = action
			};
		}

		private void OnEnable()
		{
			if (chatToggle != null)
			{
				chatToggle.OnChatOpened += onChatOpened;
				display.enabled = !chatToggle.ChatOpen;
			}

			ActiveInputDevice.OnChanged += onActiveDeviceChanged;
			Refresh();
		}

		private void OnDisable()
		{
			if (chatToggle != null)
			{
				chatToggle.OnChatOpened -= onChatOpened;
			}
			ActiveInputDevice.OnChanged -= onActiveDeviceChanged;
		}

		private void onActiveDeviceChanged(ActiveInputDevice.Kind kind)
		{
			Refresh();
		}

		private void Refresh()
		{
			Service.Get<InputService>().PopulateInputInfo(inputInfo);
			display.text = inputInfo.PrimaryKey;
		}

		private void onChatOpened(bool chatOpen)
		{
			display.enabled = !chatOpen;
		}
	}
}
