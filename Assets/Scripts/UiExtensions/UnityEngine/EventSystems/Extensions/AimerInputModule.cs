namespace UnityEngine.EventSystems.Extensions
{
	[RequireComponent(typeof(EventSystem))]
	[AddComponentMenu("Event/Extensions/Aimer Input Module")]
	public class AimerInputModule : PointerInputModule
	{
		public string activateAxis = "Submit";

		public Vector2 aimerOffset = new Vector2(0f, 0f);

		public static GameObject objectUnderAimer;

		protected AimerInputModule()
		{
		}

		public override void ActivateModule()
		{
			StandaloneInputModule component = GetComponent<StandaloneInputModule>();
			if (component != null && component.enabled)
			{
				Debug.LogError("Aimer Input Module is incompatible with the StandAloneInputSystem, please remove it from the Event System in this scene or disable it when this module is in use");
			}
		}

		public override void Process()
		{
			bool buttonDown = Input.GetButtonDown(activateAxis);
			bool buttonUp = Input.GetButtonUp(activateAxis);
			PointerEventData aimerPointerEventData = GetAimerPointerEventData();
			ProcessInteraction(aimerPointerEventData, buttonDown, buttonUp);
			if (!buttonUp)
			{
				ProcessMove(aimerPointerEventData);
			}
			else
			{
				RemovePointerData(aimerPointerEventData);
			}
		}

		protected virtual PointerEventData GetAimerPointerEventData()
		{
			PointerEventData data;
			GetPointerData(-2, out data, true);
			data.Reset();
			data.position = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f) + aimerOffset;
			base.eventSystem.RaycastAll(data, m_RaycastResultCache);
			RaycastResult raycastResult2 = data.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
			m_RaycastResultCache.Clear();
			return data;
		}

		private void ProcessInteraction(PointerEventData pointer, bool pressed, bool released)
		{
			GameObject gameObject = pointer.pointerCurrentRaycast.gameObject;
			objectUnderAimer = ExecuteEvents.GetEventHandler<ISubmitHandler>(gameObject);
			if (pressed)
			{
				pointer.eligibleForClick = true;
				pointer.delta = Vector2.zero;
				pointer.pressPosition = pointer.position;
				pointer.pointerPressRaycast = pointer.pointerCurrentRaycast;
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy(gameObject, pointer, ExecuteEvents.submitHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.ExecuteHierarchy(gameObject, pointer, ExecuteEvents.pointerDownHandler);
					if (gameObject2 == null)
					{
						gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
					}
				}
				else
				{
					pointer.eligibleForClick = false;
				}
				if (gameObject2 != pointer.pointerPress)
				{
					pointer.pointerPress = gameObject2;
					pointer.rawPointerPress = gameObject;
					pointer.clickCount = 0;
				}
				pointer.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (pointer.pointerDrag != null)
				{
					ExecuteEvents.Execute(pointer.pointerDrag, pointer, ExecuteEvents.beginDragHandler);
				}
			}
			if (!released)
			{
				return;
			}
			ExecuteEvents.Execute(pointer.pointerPress, pointer, ExecuteEvents.pointerUpHandler);
			GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
			if (pointer.pointerPress == eventHandler && pointer.eligibleForClick)
			{
				float unscaledTime = Time.unscaledTime;
				if (unscaledTime - pointer.clickTime < 0.3f)
				{
					pointer.clickCount++;
				}
				else
				{
					pointer.clickCount = 1;
				}
				pointer.clickTime = unscaledTime;
				ExecuteEvents.Execute(pointer.pointerPress, pointer, ExecuteEvents.pointerClickHandler);
			}
			else if (pointer.pointerDrag != null)
			{
				ExecuteEvents.ExecuteHierarchy(gameObject, pointer, ExecuteEvents.dropHandler);
			}
			pointer.eligibleForClick = false;
			pointer.pointerPress = null;
			pointer.rawPointerPress = null;
			if (pointer.pointerDrag != null)
			{
				ExecuteEvents.Execute(pointer.pointerDrag, pointer, ExecuteEvents.endDragHandler);
			}
			pointer.pointerDrag = null;
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			ClearSelection();
		}
	}
}
