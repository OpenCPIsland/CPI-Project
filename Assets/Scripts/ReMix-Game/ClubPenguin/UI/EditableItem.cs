using ClubPenguin.ClothingDesigner.Inventory;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class EditableItem : MonoBehaviour
	{
		public enum ActionType
		{
			Delete,
			Hide,
			Show
		}

		private static Color32 SHOWN_ITEM_COLOR = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		private static Color32 HIDDEN_ITEM_COLOR = new Color32(84, 120, 189, 168);

		public Action<GameObject> OnContentLoaded;

		public GameObject ActionButton;

		public Transform ItemContent;

		public Animator ContentAnimator;

		private bool isDisappeared;

		private SpriteSelector actionSpriteSelector;

		private EquipmentIcon iconItem;

		private bool showActionButton = true;

		private ActionType action;

		public bool ShowActionButton
		{
			get
			{
				return showActionButton;
			}
			set
			{
				showActionButton = value;
				if (!value)
				{
					ContentAnimator.SetBool("IsEditable", false);
					ActionButton.SetActive(false);
				}
			}
		}

		public ActionType Action
		{
			get
			{
				return action;
			}
			set
			{
				action = value;
				setActionState();
			}
		}

		private void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<EditableItemEvents.EditStateChanged>(onEditStateChanged);
		}

		private void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<EditableItemEvents.EditStateChanged>(onEditStateChanged);
			ContentAnimator.SetBool("IsEditable", true);
			showActionButton = true;
		}

		public void LoadContentPrefab(PrefabContentKey prefabContentKey)
		{
			Content.LoadAsync(onContentPrefabLoaded, prefabContentKey);
		}

		private void onContentPrefabLoaded(string path, GameObject contentPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(contentPrefab);
			gameObject.transform.SetParent(ItemContent);
			if (OnContentLoaded != null)
			{
				OnContentLoaded(base.gameObject);
				OnContentLoaded = null;
			}
		}

		public void ActionButtonClicked()
		{
			if (Action == ActionType.Hide)
			{
				Action = ActionType.Show;
				setActionState();
			}
			else if (Action == ActionType.Show)
			{
				Action = ActionType.Hide;
				setActionState();
			}
			Service.Get<EventDispatcher>().DispatchEvent(new EditableItemEvents.ActionButtonClicked(this, Action));
		}

		public void Remove()
		{
			isDisappeared = true;
			ContentAnimator.SetTrigger("Removed");
		}

		public void ResetItem()
		{
			if (isDisappeared)
			{
				ContentAnimator.SetTrigger("Reset");
				isDisappeared = false;
			}
		}

		public void OnDisappearAnimationComplete()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new EditableItemEvents.ItemDisappeared(this));
		}

		public int GetCurrentIndex()
		{
			return base.transform.parent.GetSiblingIndex();
		}

		public void SetEditable(bool isEditable)
		{
			bool flag = isEditable;
			if (!showActionButton)
			{
				flag = false;
			}
			ContentAnimator.SetBool("IsEditable", flag);
			ActionButton.SetActive(flag);
		}

		private bool onEditStateChanged(EditableItemEvents.EditStateChanged evt)
		{
			SetEditable(evt.IsEditStateActive);
			setActionState();
			return false;
		}

		private void setActionState()
		{
			if (actionSpriteSelector == null)
			{
				actionSpriteSelector = ActionButton.GetComponentInChildren<SpriteSelector>();
			}
			if (iconItem == null)
			{
				iconItem = GetComponentInChildren<EquipmentIcon>();
			}
			if (Action == ActionType.Hide)
			{
				iconItem.ItemIcon.color = HIDDEN_ITEM_COLOR;
				actionSpriteSelector.SelectSprite(2);
			}
			else if (Action == ActionType.Show)
			{
				actionSpriteSelector.SelectSprite(1);
				iconItem.ItemIcon.color = SHOWN_ITEM_COLOR;
			}
			else
			{
				actionSpriteSelector.SelectSprite(0);
				iconItem.ItemIcon.color = SHOWN_ITEM_COLOR;
			}
		}
	}
}
