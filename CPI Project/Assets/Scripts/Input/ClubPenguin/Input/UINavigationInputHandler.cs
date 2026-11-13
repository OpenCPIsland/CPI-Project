using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.Input
{
	public class UINavigationInputHandler : InputMapHandler<UINavigationInputMap.Result>
	{
		[SerializeField]
		private Selectable initialSelectable = null;

		protected override void OnEnable()
		{
			if (initialSelectable != null)
			{
				CoroutineRunner.Start(autoSelectInitial(), this, "UINavigationInputHandler.autoSelectInitial");
			}
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			CoroutineRunner.StopAllForOwner(this);
			base.OnDisable();
		}

		private IEnumerator autoSelectInitial()
		{
			yield return null;
			selectSelectable(initialSelectable);
		}

		protected override void onHandle(UINavigationInputMap.Result inputResult)
		{
			if (inputResult.Navigate.WasJustReleased)
			{
				navigate(inputResult.NavigateBackwards.IsHeld);
			}
			if (!inputResult.Submit.WasJustPressed && !inputResult.Submit.WasJustReleased)
			{
				return;
			}
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null && currentSelectedGameObject.GetComponent<InputField>() == null)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				if (inputResult.Submit.WasJustPressed)
				{
					ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
					ExecuteEvents.Execute(base.gameObject, eventData, ExecuteEvents.pointerDownHandler);
				}
				else
				{
					ExecuteEvents.Execute(currentSelectedGameObject, eventData, ExecuteEvents.pointerUpHandler);
					ExecuteEvents.Execute(currentSelectedGameObject, eventData, ExecuteEvents.pointerExitHandler);
					ExecuteEvents.Execute(currentSelectedGameObject, eventData, ExecuteEvents.submitHandler);
				}
			}
		}

		protected override void onReset()
		{
		}

		private void selectSelectable(Selectable selectableToSelect)
		{
			if (selectableToSelect != null)
			{
				selectableToSelect.Select();
				InputField inputField = selectableToSelect as InputField;
				if (inputField != null)
				{
					inputField.ActivateInputField();
				}
			}
		}

		private void navigate(bool isNavigateBackward)
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null && currentSelectedGameObject.activeInHierarchy)
			{
				Selectable component = currentSelectedGameObject.GetComponent<Selectable>();
				Selectable selectable = findNextSelectable(component, isNavigateBackward);
				selectSelectable(selectable ?? initialSelectable);
			}
			else
			{
				selectSelectable(initialSelectable);
			}
		}

		private Selectable findNextSelectable(Selectable currentSelectable, bool isNavigateBackward)
		{
			Selectable selectable = null;
			if (currentSelectable != null)
			{
				selectable = ((!isNavigateBackward) ? (currentSelectable.FindSelectableOnRight() ?? currentSelectable.FindSelectableOnDown()) : (currentSelectable.FindSelectableOnLeft() ?? currentSelectable.FindSelectableOnUp()));
			}
			if (selectable != null && !selectable.isActiveAndEnabled)
			{
				selectable = findNextSelectable(selectable, isNavigateBackward);
			}
			return selectable;
		}
	}
}
