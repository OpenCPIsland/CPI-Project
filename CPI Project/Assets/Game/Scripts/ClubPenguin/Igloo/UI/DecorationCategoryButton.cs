using ClubPenguin.Gui;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Igloo.UI
{
	[RequireComponent(typeof(Button))]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TintToggleGroupButton))]
	public class DecorationCategoryButton : MonoBehaviour
	{
		public Action<int> ButtonClicked;

		private Button ButtonComponent;

		private int buttonIndex;

		public void Initialize(int index, string displayName)
		{
			buttonIndex = index;
			ButtonComponent = GetComponent<Button>();
			ButtonComponent.GetComponentInChildren<Text>().text = Service.Get<Localizer>().GetTokenTranslation(displayName);
			ButtonComponent.onClick.AddListener(OnButtonClicked);
		}

		public void OnDestroy()
		{
			if (ButtonComponent != null)
			{
				ButtonComponent.onClick.RemoveAllListeners();
			}
			ButtonClicked = null;
		}

		public void OnButtonClicked()
		{
			if (ButtonClicked != null)
			{
				ButtonClicked(buttonIndex);
			}
		}
	}
}
