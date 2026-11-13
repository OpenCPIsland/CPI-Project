using ClubPenguin.Input;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class ButtonEventSource : AbstractEventSource
	{
		private Button button;

		private ButtonClickListener clickListener;

		private void Awake()
		{
			button = GetComponent<Button>();
			clickListener = GetComponent<ButtonClickListener>();
		}

		private void OnEnable()
		{
			if (clickListener != null)
			{
				clickListener.OnClick.AddListener(onListenerClicked);
			}
			else
			{
				button.onClick.AddListener(onButtonClicked);
			}
		}

		protected virtual void OnDisable()
		{
			if (clickListener != null)
			{
				clickListener.OnClick.RemoveListener(onListenerClicked);
			}
			else
			{
				button.onClick.RemoveListener(onButtonClicked);
			}
		}

		private void onListenerClicked(ButtonClickListener.ClickType interactedType)
		{
			handleEvent();
		}

		private void onButtonClicked()
		{
			handleEvent();
		}

		protected virtual void handleEvent()
		{
			sendEvent();
		}
	}
}
