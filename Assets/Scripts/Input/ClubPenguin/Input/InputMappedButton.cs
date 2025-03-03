using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.Input
{
	[RequireComponent(typeof(ButtonClickListener))]
	public class InputMappedButton : MonoBehaviour
	{
		private ButtonClickListener clickListener;

		private bool isPressed;

		private void Awake()
		{
			clickListener = GetComponent<ButtonClickListener>();
		}

		private void OnEnable()
		{
			releaseButton();
		}

		public void HandleMappedInput(ButtonInputResult buttonInput = null)
		{
			if (clickListener.Interactable)
			{
				if (buttonInput == null)
				{
					releaseButton();
				}
				else if (buttonInput.WasJustPressed)
				{
					pressButton();
				}
				else if (buttonInput.WasJustReleased && isPressed)
				{
					releaseButton();
					clickListener.InvokeClick(ButtonClickListener.ClickType.InputMap);
				}
			}
		}

		private void pressButton()
		{
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
			ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerDownHandler);
			isPressed = true;
		}

		private void releaseButton()
		{
			if (isPressed)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerUpHandler);
				ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerExitHandler);
				isPressed = false;
			}
		}
	}
}
