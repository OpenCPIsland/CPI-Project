using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ButtonPanelEnabler : MonoBehaviour
	{
		public string PanelName = "";

		private Button[] buttons;

		private void Start()
		{
		}

		public void Awake()
		{
			buttons = GetComponentsInChildren<Button>(true);
			Service.Get<EventDispatcher>().AddListener<ButtonEvents.EnableButtonPanel>(onEnableButtonPanel);
			Service.Get<EventDispatcher>().AddListener<ButtonEvents.DisableButtonPanel>(onDisableButtonPanel);
			Service.Get<EventDispatcher>().AddListener<ButtonEvents.SetButtonInteractable>(onSetButtonInteractable);
		}

		public void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<ButtonEvents.EnableButtonPanel>(onEnableButtonPanel);
			Service.Get<EventDispatcher>().RemoveListener<ButtonEvents.DisableButtonPanel>(onDisableButtonPanel);
			Service.Get<EventDispatcher>().RemoveListener<ButtonEvents.SetButtonInteractable>(onSetButtonInteractable);
		}

		private bool onEnableButtonPanel(ButtonEvents.EnableButtonPanel evt)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = true;
			}
			return false;
		}

		private bool onDisableButtonPanel(ButtonEvents.DisableButtonPanel evt)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].interactable = false;
			}
			return false;
		}

		private bool onSetButtonInteractable(ButtonEvents.SetButtonInteractable evt)
		{
			if (evt.PanelName != "" && evt.PanelName != PanelName)
			{
				return false;
			}
			for (int i = 0; i < buttons.Length; i++)
			{
				if (buttons[i].name == evt.ButtonName)
				{
					buttons[i].interactable = evt.IsInteractable;
				}
			}
			return false;
		}
	}
}
