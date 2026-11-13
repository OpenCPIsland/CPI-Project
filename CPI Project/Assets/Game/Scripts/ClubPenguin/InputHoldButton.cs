using System.Collections;
using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class InputHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		private const float INITIAL_REPEAT_DELAY = 0.5f;

		private const float REPEAT_INTERVAL = 0.17f;

		public InputEvents.ChargeActions Action;

		public bool RepeatButtonClick;

		private Button button;

		private EventDispatcher dispatcher;

		private float holdTime;

		private bool buttonDown;

		private bool prevInteractable = false;

		private Coroutine repeatCoroutine;

		private bool hasRepeated;

		private bool repeatOnly;

		public static void AttachToQuantityButtons(GameObject root)
		{
			Button[] buttons = root.GetComponentsInChildren<Button>(true);
			foreach (Button quantityButton in buttons)
			{
				bool isQuantityButton = false;
				for (int i = 0; i < quantityButton.onClick.GetPersistentEventCount(); i++)
				{
					string methodName = quantityButton.onClick.GetPersistentMethodName(i);
					if (methodName == "OnCountIncreaseClick" || methodName == "OnCountDecreaseClick")
					{
						isQuantityButton = true;
						break;
					}
				}
				if (isQuantityButton)
				{
					InputHoldButton inputHoldButton = quantityButton.GetComponent<InputHoldButton>();
					if (inputHoldButton == null)
					{
						inputHoldButton = quantityButton.gameObject.AddComponent<InputHoldButton>();
					}
					inputHoldButton.repeatOnly = true;
					inputHoldButton.RepeatButtonClick = true;
				}
			}
		}

		private void Awake()
		{
			dispatcher = Service.Get<EventDispatcher>();
			button = GetComponent<Button>();
			prevInteractable = button.IsInteractable();
		}

		public void OnDestroy()
		{
			if (RepeatButtonClick)
			{
				bool shouldRelease = buttonDown && !repeatOnly;
				stopRepeating();
				if (shouldRelease)
				{
					release(holdTime);
				}
			}
			else if (buttonDown)
			{
				release(holdTime);
			}
		}

		public void OnPointerDown(PointerEventData data)
		{
			if (button.IsInteractable() && !buttonDown)
			{
				if (RepeatButtonClick)
				{
					buttonDown = true;
					hasRepeated = false;
					if (repeatOnly && data != null)
					{
						data.eligibleForClick = false;
					}
					repeatCoroutine = StartCoroutine(repeatButtonClick());
					if (repeatOnly)
					{
						return;
					}
				}
				holdTime = 0f;
				buttonDown = true;
				dispatcher.DispatchEvent(new InputEvents.ChargeActionEvent(Action, true, holdTime));
			}
		}

		public void OnPointerUp(PointerEventData data)
		{
			if (RepeatButtonClick)
			{
				if (repeatOnly)
				{
					if (data != null)
					{
						data.eligibleForClick = false;
					}
					bool shouldClick = !hasRepeated && button.IsInteractable() && isReleasedOnButton(data);
					stopRepeating();
					if (shouldClick)
					{
						button.onClick.Invoke();
					}
					return;
				}
				if (hasRepeated && data != null)
				{
					data.eligibleForClick = false;
				}
				if (repeatCoroutine != null)
				{
					StopCoroutine(repeatCoroutine);
					repeatCoroutine = null;
				}
				hasRepeated = false;
			}
			if (buttonDown)
			{
				release(holdTime);
			}
		}

		private bool isReleasedOnButton(PointerEventData data)
		{
			return data != null &&
				data.button == PointerEventData.InputButton.Left &&
				ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerCurrentRaycast.gameObject) == button.gameObject;
		}

		private IEnumerator repeatButtonClick()
		{
			yield return new WaitForSecondsRealtime(INITIAL_REPEAT_DELAY);
			while (buttonDown && button.IsInteractable())
			{
				hasRepeated = true;
				button.onClick.Invoke();
				yield return new WaitForSecondsRealtime(REPEAT_INTERVAL);
			}
		}

		private void stopRepeating()
		{
			buttonDown = false;
			if (repeatCoroutine != null)
			{
				StopCoroutine(repeatCoroutine);
				repeatCoroutine = null;
			}
			hasRepeated = false;
		}

		private void release(float _holdTime)
		{
			dispatcher.DispatchEvent(new InputEvents.ChargeActionEvent(Action, false, _holdTime));
			buttonDown = false;
		}

		public void Update()
		{
			if (RepeatButtonClick && repeatOnly)
			{
				if (buttonDown && !button.IsInteractable())
				{
					stopRepeating();
				}
				return;
			}
			if (buttonDown)
			{
				holdTime += Time.deltaTime;
			}
			if (prevInteractable != button.IsInteractable())
			{
				prevInteractable = button.IsInteractable();
				if (!button.IsInteractable() && buttonDown)
				{
					release(holdTime);
				}
			}
		}
	}
}
