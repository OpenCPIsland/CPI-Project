using DevonLocalization.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevonLocalization.TestTools
{
	[RequireComponent(typeof(Toggle))]
	public class LanguageToggleBehaviour : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public delegate void OnToggleClickedDelegate(Language language);

		public Language Language;

		public OnToggleClickedDelegate OnToggleClicked;

		public void OnPointerClick(PointerEventData eventData)
		{
			Toggle component = GetComponent<Toggle>();
			if (component.isOn && OnToggleClicked != null)
			{
				OnToggleClicked(Language);
			}
		}
	}
}
