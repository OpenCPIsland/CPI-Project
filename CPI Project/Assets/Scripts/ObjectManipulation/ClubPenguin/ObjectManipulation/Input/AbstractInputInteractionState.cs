#if UNITY_ANDROID || UNITY_IOS
using Disney.Kelowna.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin.ObjectManipulation.Input
{
	public abstract class AbstractInputInteractionState
	{
		public LayerMask TargetLayerMask = -1;

		protected InteractionState state;

		protected Vector3 lastMousePositionWhenDown = Vector3.zero;

		protected float MinTimeToMoveInput;

		public InteractionState State
		{
			get
			{
				return state;
			}
			private set
			{
			}
		}

		public virtual void EnterState(LayerMask targetLayerMask, float minTimeToMoveInput)
		{
			MinTimeToMoveInput = minTimeToMoveInput;
			TargetLayerMask = targetLayerMask;
		}

		public virtual void ExitState()
		{
		}

        public virtual int Update()
        {
            int num = InputWrapper.touchCount; 

            if (num == 1)
            {
                TouchEquivalent touchEq = InputWrapper.GetTouch(0);
                if (touchEq.Phase != UnityEngine.TouchPhase.Canceled) 
                {
                    processOneTouch(touchEq);
                }
                return num; 
            }
            else if (num == 2) 
            {
                return num;
            }

            if (InputWrapper.GetMouseButton(0)) 
            {
                if (!EventSystem.current.IsPointerOverGameObject() && !IsScreenPointOverUI(InputWrapper.mousePosition))
                {
                    processOneTouch(TouchEquivalent.FromLeftMouseButton(lastMousePositionWhenDown));
                    lastMousePositionWhenDown = InputWrapper.mousePosition;
                }
                num = 1; 
            }
            else if (InputWrapper.GetMouseButtonUp(0)) 
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    processOneTouch(TouchEquivalent.FromLeftMouseButton(lastMousePositionWhenDown));
                    lastMousePositionWhenDown = Vector3.zero;
                }
                num = 0; 
            }
            else
            {
            }

            return num;
        }



        private static bool IsScreenPointOverUI(Vector2 position)
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = position;
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, list);
			return list.Count > 0;
		}

		protected abstract void processOneTouch(TouchEquivalent touch);

		protected GameObject raycastScreenPointToObject(Vector2 screenPosition, LayerMask mask)
		{
			GameObject result = null;
			Ray ray = Camera.main.ScreenPointToRay(screenPosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, mask, QueryTriggerInteraction.Collide))
			{
				result = hitInfo.transform.gameObject;
			}
			return result;
		}
	}
}
#else
using Disney.Kelowna.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace ClubPenguin.ObjectManipulation.Input
{
    public abstract class AbstractInputInteractionState
    {
        public LayerMask TargetLayerMask = -1;

        protected InteractionState state;

        protected Vector3 lastMousePositionWhenDown = Vector3.zero;

        protected float MinTimeToMoveInput;

        private static bool enhancedTouchInitialized = false;

        public InteractionState State
        {
            get
            {
                return state;
            }
            private set
            {
            }
        }

        public virtual void EnterState(LayerMask targetLayerMask, float minTimeToMoveInput)
        {
            MinTimeToMoveInput = minTimeToMoveInput;
            TargetLayerMask = targetLayerMask;
        }

        public virtual void ExitState()
        {
        }

        public virtual int Update()
        {
            
            if (!enhancedTouchInitialized)
            {
                if (!EnhancedTouchSupport.enabled)
                {
                    EnhancedTouchSupport.Enable();
                }
                enhancedTouchInitialized = true;
            }

            int num = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
            if (num == 1)
            {
                
            }

            
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                num = 1;
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (!EventSystem.current.IsPointerOverGameObject() && !IsScreenPointOverUI(mousePos))
                {
                    processOneTouch(TouchEquivalent.FromLeftMouseButton(lastMousePositionWhenDown));
                    lastMousePositionWhenDown = mousePos;
                }
            }
            else if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame && !EventSystem.current.IsPointerOverGameObject())
            {
                processOneTouch(TouchEquivalent.FromLeftMouseButton(lastMousePositionWhenDown));
                lastMousePositionWhenDown = Vector3.zero;
            }

            return num;
        }

        private static bool IsScreenPointOverUI(Vector2 position)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = position;
            List<RaycastResult> list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, list);
            return list.Count > 0;
        }

        protected abstract void processOneTouch(TouchEquivalent touch);

        protected GameObject raycastScreenPointToObject(Vector2 screenPosition, LayerMask mask)
        {
            GameObject result = null;
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, mask, QueryTriggerInteraction.Collide))
            {
                result = hitInfo.transform.gameObject;
            }
            return result;
        }
    }
}
#endif
