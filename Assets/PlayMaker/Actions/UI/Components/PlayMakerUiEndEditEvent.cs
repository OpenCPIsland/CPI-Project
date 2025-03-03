#if !PLAYMAKER_NO_UI

#if PLAYMAKER_TMPRO
using TMPro;
#endif

using UnityEngine.UI;
using UnityEngine;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI End Edit Event")]
    public class PlayMakerUiEndEditEvent : PlayMakerUiEventBase
    {
        public InputField inputField;

#if PLAYMAKER_TMPRO
        public TMP_InputField tmpInputField;
#endif

        protected override void Initialize()
        {
            if (initialized) return;
            initialized = true;

            if (inputField == null)
            {
                inputField = GetComponent<InputField>();
            }

            if (inputField != null)
            {
                inputField.onEndEdit.AddListener(DoOnEndEdit);
            }

#if PLAYMAKER_TMPRO

            if (inputField != null) return;

            if (tmpInputField == null)
            {
                tmpInputField = GetComponent<TMP_InputField>();
            }

            if (tmpInputField != null)
            {
                tmpInputField.onEndEdit.AddListener(DoOnEndEdit);
            }
#endif
        }

        protected void OnDisable()
        {
            initialized = false;

            if (inputField != null)
            {
                inputField.onEndEdit.RemoveListener(DoOnEndEdit);
            }

#if PLAYMAKER_TMPRO
            if (tmpInputField != null)
            {
                tmpInputField.onEndEdit.RemoveListener(DoOnEndEdit);
            }
#endif
        }

        private void DoOnEndEdit(string value)
        {
            Fsm.EventData.StringData = value;
            SendEvent(FsmEvent.UiEndEdit);
        }

    }
}

#endif