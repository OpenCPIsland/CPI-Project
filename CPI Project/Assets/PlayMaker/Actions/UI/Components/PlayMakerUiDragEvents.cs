#if !PLAYMAKER_NO_UI

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
#define NEW_INPUT_SYSTEM_ONLY
#endif

using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI Drag Events")]
    public class PlayMakerUiDragEvents : PlayMakerUiEventBase, 
        IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            SendEvent(FsmEvent.UiBeginDrag);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            SendEvent(FsmEvent.UiDrag);
            
#if NEW_INPUT_SYSTEM_ONLY
            SendEvent(FsmEvent.MouseDrag);
#endif
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            UiGetLastPointerDataInfo.lastPointerEventData = eventData;
            SendEvent(FsmEvent.UiEndDrag);
        }
    }
}

#endif