using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Analytics
{
	[RequireComponent(typeof(Button))]
	public class LogGameActionOnClick : MonoBehaviour
	{
		public string Context;

		public string Action;

		public string Message;

		public string Type;

		private Button button;

		private void Start()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(onButtonClick);
		}

		private void onButtonClick()
		{
			string text = Context + "." + Action;
			if (!string.IsNullOrEmpty(Message))
			{
				text = text + "." + Message;
			}
			if (!string.IsNullOrEmpty(Type))
			{
				text = text + "." + Type;
			}
			Service.Get<ICPSwrveService>().NavigationAction(text);
		}

		private void OnDestroy()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(onButtonClick);
			}
		}
	}
}
