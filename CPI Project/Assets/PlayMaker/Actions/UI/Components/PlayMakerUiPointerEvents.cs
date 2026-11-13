#if !PLAYMAKER_NO_UI

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI Pointer Events")]
    public class PlayMakerUiPointerEvents : PlayMakerUiEventBase,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerUpHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            SendEvent(FsmEvent.UiPointerClick);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            SendEvent(FsmEvent.UiPointerDown);
            
#if NEW_INPUT_SYSTEM_ONLY
            SendEvent(FsmEvent.MouseDown);
#endif
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            SendEvent(FsmEvent.UiPointerEnter);
            
#if NEW_INPUT_SYSTEM_ONLY
            SendEvent(FsmEvent.MouseEnter);
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            SendEvent(FsmEvent.UiPointerExit);
            
#if NEW_INPUT_SYSTEM_ONLY
            SendEvent(FsmEvent.MouseExit);
#endif
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            SendEvent(FsmEvent.UiPointerUp);
            
#if NEW_INPUT_SYSTEM_ONLY
            SendEvent(FsmEvent.MouseUp);
#endif
        }
    }
}

#endif