using ClubPenguin.Input;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ButtonClickListener))]
	public class MainNavMapButton : MonoBehaviour
	{
		[SerializeField]
		private PrefabContentKey mapContentKey = new PrefabContentKey("Prefabs/WorldMap");

		[SerializeField]
		private string accessibilityKey = "Accessibility.Popup.Title.Map";

		private ButtonClickListener buttonClickListener;

		private bool isLoadingPrefab = false;

		private void Awake()
		{
			buttonClickListener = GetComponent<ButtonClickListener>();
		}

		private void OnEnable()
		{
			buttonClickListener.OnClick.AddListener(onButtonClicked);
		}

		private void OnDisable()
		{
			buttonClickListener.OnClick.RemoveListener(onButtonClicked);
		}

		private void onButtonClicked(ButtonClickListener.ClickType clickType)
		{
			if (base.isActiveAndEnabled && !isLoadingPrefab)
			{
				isLoadingPrefab = true;
				SceneRefs.FullScreenPopupManager.CreatePopup(mapContentKey, accessibilityKey, true, onPrefabCreated);
			}
		}

		private void onPrefabCreated(PrefabContentKey key, GameObject instance)
		{
			isLoadingPrefab = false;
		}
	}
}
