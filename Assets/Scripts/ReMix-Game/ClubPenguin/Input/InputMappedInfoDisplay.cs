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
			Service.Get<InputService>().PopulateInputInfo(inputInfo);
			display.text = inputInfo.PrimaryKey;
		}

		private void OnDisable()
		{
			if (chatToggle != null)
			{
				chatToggle.OnChatOpened -= onChatOpened;
			}
		}

		private void onChatOpened(bool chatOpen)
		{
			display.enabled = !chatOpen;
		}
	}
}
