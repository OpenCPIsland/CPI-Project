#if !PLAYMAKER_NO_UI

#if PLAYMAKER_TMPRO
using TMPro;
#endif

using UnityEngine.UI;
using UnityEngine;

namespace HutongGames.PlayMaker
{
    [AddComponentMenu("PlayMaker/UI/UI Int Value Changed Event")]
    public class PlayMakerUiIntValueChangedEvent : PlayMakerUiEventBase
    {
        public Dropdown dropdown;

#if PLAYMAKER_TMPRO
        public TMP_Dropdown tmpDropdown;
#endif

        protected override void Initialize()
        {
            if (initialized) return;
            initialized = true;

            if (dropdown == null)
            {
                dropdown = GetComponent<Dropdown>();
            }

            if (dropdown != null)
            {
                dropdown.onValueChanged.AddListener(OnValueChanged);
            }

#if PLAYMAKER_TMPRO

            if (dropdown != null) return;

            if (tmpDropdown == null)
            {
                tmpDropdown = GetComponent<TMP_Dropdown>();
            }

            if (tmpDropdown != null)
            {
                tmpDropdown.onValueChanged.AddListener(OnValueChanged);
            }
#endif
        }

        protected void OnDisable()
        {
            initialized = false;

            if (dropdown != null)
            {
                dropdown.onValueChanged.RemoveListener(OnValueChanged);
            }

#if PLAYMAKER_TMPRO
            if (tmpDropdown != null)
            {
                tmpDropdown.onValueChanged.RemoveListener(OnValueChanged);
            }
#endif
        }

        private void OnValueChanged(int value)
        {
            Fsm.EventData.IntData = value;
            SendEvent(FsmEvent.UiIntValueChanged);
        }

    }
}

#endif