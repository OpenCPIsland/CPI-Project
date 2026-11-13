using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Prototype
{
	[RequireComponent(typeof(Button))]
	public class PrototypeTogglePanel : MonoBehaviour
	{
		public enum ToggleFunctionalityEnum
		{
			TOGGLE,
			TURN_ALL_OFF,
			TURN_ALL_ON
		}

		public ToggleFunctionalityEnum Functionality;

		public GameObject[] GameObjectsToToggle;

		private Button buttonRef;

		private void Start()
		{
			buttonRef = GetComponent<Button>();
			buttonRef.onClick.AddListener(onButtonClick);
		}

		private void OnDestroy()
		{
			buttonRef.onClick.RemoveListener(onButtonClick);
		}

		private void onButtonClick()
		{
			for (int i = 0; i < GameObjectsToToggle.Length; i++)
			{
				switch (Functionality)
				{
				case ToggleFunctionalityEnum.TOGGLE:
					GameObjectsToToggle[i].SetActive(!GameObjectsToToggle[i].activeSelf);
					break;
				case ToggleFunctionalityEnum.TURN_ALL_OFF:
					GameObjectsToToggle[i].SetActive(false);
					break;
				case ToggleFunctionalityEnum.TURN_ALL_ON:
					GameObjectsToToggle[i].SetActive(true);
					break;
				}
			}
		}
	}
}
