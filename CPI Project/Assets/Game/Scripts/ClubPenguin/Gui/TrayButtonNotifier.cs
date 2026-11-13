using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Gui
{
	[RequireComponent(typeof(Button))]
	public class TrayButtonNotifier : TrayNotifier
	{
		private Button button;

		public override void Awake()
		{
			base.Awake();
			button = GetComponent<Button>();
		}

		private void OnClick()
		{
			controller.OnTrayButtonClicked(button);
		}

		public void OnEnable()
		{
			button.onClick.AddListener(OnClick);
		}

		public void OnDisable()
		{
			button.onClick.RemoveListener(OnClick);
		}
	}
}
