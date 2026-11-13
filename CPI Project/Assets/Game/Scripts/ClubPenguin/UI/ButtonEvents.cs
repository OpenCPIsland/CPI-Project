namespace ClubPenguin.UI
{
	public static class ButtonEvents
	{
		public struct ClickEvent
		{
			public readonly DButton ButtonData;

			public ClickEvent(DButton buttonData)
			{
				ButtonData = buttonData;
			}
		}

		public struct DisableButtonPanel
		{
			public string ButtonPanelName;

			public DisableButtonPanel(string buttonPanelName)
			{
				ButtonPanelName = buttonPanelName;
			}
		}

		public struct EnableButtonPanel
		{
			public string ButtonPanelName;

			public EnableButtonPanel(string buttonPanelName)
			{
				ButtonPanelName = buttonPanelName;
			}
		}

		public struct SetButtonInteractable
		{
			public string PanelName;

			public string ButtonName;

			public bool IsInteractable;

			public bool HideOnDisable;

			public SetButtonInteractable(string panelName = "", string buttonName = "", bool isInteractable = true, bool hideOnDisable = false)
			{
				PanelName = panelName;
				ButtonName = buttonName;
				IsInteractable = isInteractable;
				HideOnDisable = hideOnDisable;
			}
		}
	}
}
