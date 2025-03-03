using ClubPenguin.ClothingDesigner.Inventory;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class EditableItemController : MonoBehaviour
	{
		private static readonly PrefabContentKey editableItemPrefabContentKey = new PrefabContentKey("Prefabs/EditableItems/EditableItemPrefab");

		public Text TitleText;

		public Button EditButton;

		public Button BackButton;

		public PooledCellsScrollRect EditableItemPooledScrollRect;

		public GameObject LoadingOverlay;

		private PrefabContentKey prefabContentKey;

		private bool isEditStateActive;

		private int initialSize;

		private GameObject editableItemInstance;

		private EventChannel eventChannel;

		private void Start()
		{
			eventChannel = new EventChannel(InventoryContext.EventBus);
			eventChannel.AddListener<InventoryUIEvents.StartedLoadingEquipment>(onStartLoadEquipment);
			eventChannel.AddListener<InventoryUIEvents.AllEquipmentLoaded>(onAllEquipmentLoaded);
			LoadingOverlay.SetActive(false);
		}

		private void OnDestroy()
		{
			EditButton.onClick.RemoveAllListeners();
			BackButton.onClick.RemoveAllListeners();
			EditableItemPooledScrollRect.ObjectAdded -= onObjectAdded;
			UnityEngine.Object.Destroy(editableItemInstance);
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		public void SetEditStateActive(bool isActive)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new EditableItemEvents.EditStateChanged(isActive));
			EditButton.gameObject.SetActive(!isActive);
			BackButton.gameObject.SetActive(isActive);
			isEditStateActive = isActive;
		}

		public void HideActionButtons()
		{
			EditButton.gameObject.SetActive(false);
			BackButton.gameObject.SetActive(false);
		}

		public void ShowActionButtons()
		{
			EditButton.gameObject.SetActive(!isEditStateActive);
			BackButton.gameObject.SetActive(isEditStateActive);
		}

		public void CreateEditableItemList(PrefabContentKey prefabContentKey, int size)
		{
			initialSize = size;
			this.prefabContentKey = prefabContentKey;
			Content.LoadAsync(onEditableItemPrefabLoaded, editableItemPrefabContentKey);
		}

		private void onEditableItemPrefabLoaded(string path, GameObject editableItemPrefab)
		{
			editableItemInstance = UnityEngine.Object.Instantiate(editableItemPrefab);
			EditableItem componentInChildren = editableItemInstance.GetComponentInChildren<EditableItem>();
			if (componentInChildren != null)
			{
				componentInChildren.OnContentLoaded = (Action<GameObject>)Delegate.Combine(componentInChildren.OnContentLoaded, new Action<GameObject>(onEditableItemContentLoaded));
				componentInChildren.LoadContentPrefab(prefabContentKey);
			}
		}

		private void onEditableItemContentLoaded(GameObject editableItem)
		{
			EditableItemPooledScrollRect.ObjectAdded += onObjectAdded;
			EditableItemPooledScrollRect.Init(initialSize, editableItem);
		}

		private void onObjectAdded(RectTransform item, int index)
		{
			EditableItem component = item.GetComponent<EditableItem>();
			component.ResetItem();
			component.SetEditable(isEditStateActive);
			Service.Get<EventDispatcher>().DispatchEvent(new EditableItemEvents.ItemReady(component, index));
		}

		public void ResetContent()
		{
			EditableItemPooledScrollRect.ResetContent();
		}

		public void Refresh(int size)
		{
			EditableItemPooledScrollRect.RefreshList(size);
		}

		public bool IsItemVisible(int index)
		{
			return EditableItemPooledScrollRect.IsIndexCellVisible(index);
		}

		public EditableItem GetItemByIndex(int index)
		{
			return EditableItemPooledScrollRect.GetCellAtIndex(index).GetComponent<EditableItem>();
		}

		public void Remove(int index)
		{
			if (EditableItemPooledScrollRect.IsIndexCellVisible(index))
			{
				EditableItemPooledScrollRect.GetCellAtIndex(index).GetComponent<EditableItem>().Remove();
			}
		}

		public void SetTitleText(string title)
		{
			TitleText.text = title;
		}

		private bool onStartLoadEquipment(InventoryUIEvents.StartedLoadingEquipment evt)
		{
			LoadingOverlay.SetActive(true);
			return false;
		}

		private bool onAllEquipmentLoaded(InventoryUIEvents.AllEquipmentLoaded evt)
		{
			LoadingOverlay.SetActive(false);
			return false;
		}
	}
}
