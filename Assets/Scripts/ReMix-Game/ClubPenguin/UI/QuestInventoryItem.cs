using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestInventoryItem : MonoBehaviour
	{
		public Image ItemImage;

		public Image MoreInfoIcon;

		public GameObject ItemCountIcon;

		public Text ItemCountText;

		public NotificationBreadcrumb Breadcrumb;

		public TutorialBreadcrumb TutorialBreadcrumb;

		private QuestItem item;

		public PrefabContentKey imagePopupContentKey = new PrefabContentKey("Prefabs/Popups/ImagePopup");

		public void LoadQuestItem(QuestItem item)
		{
			this.item = item;
			MoreInfoIcon.gameObject.SetActive(item.State == QuestItem.QuestItemState.Interactive);
			if (item.ItemCount <= 1)
			{
				ItemCountIcon.SetActive(false);
			}
			else
			{
				ItemCountIcon.SetActive(true);
				ItemCountText.text = item.ItemCount.ToString();
			}
			if (item.DataModel.ItemInventoryContentKey != null && !string.IsNullOrEmpty(item.DataModel.ItemInventoryContentKey.Key))
			{
				CoroutineRunner.Start(loadImage(item.DataModel.ItemInventoryContentKey), this, "loadItemIcon");
			}
			Breadcrumb.SetBreadcrumbId(string.Format("QuestItem_{0}", item.DataModel.Name));
			TutorialBreadcrumb.SetBreadcrumbId(string.Format("QuestItem_{0}", item.DataModel.Name));
		}

		private IEnumerator loadImage(SpriteContentKey assetContentKey)
		{
			AssetRequest<Sprite> assetRequest = Content.LoadAsync(assetContentKey);
			yield return assetRequest;
			ItemImage.sprite = assetRequest.Asset;
		}

		public void OnUseButtonClick()
		{
			switch (item.DataModel.ClickAction)
			{
			case QuestDefinition.DQuestItem.QuestItemClickAction.loadPopup:
				loadItemPopup();
				break;
			case QuestDefinition.DQuestItem.QuestItemClickAction.showTrayScreen:
				showItemTrayScreen();
				break;
			case QuestDefinition.DQuestItem.QuestItemClickAction.openChat:
				openChatEmotes();
				break;
			case QuestDefinition.DQuestItem.QuestItemClickAction.useConsumable:
				useConsumable();
				break;
			case QuestDefinition.DQuestItem.QuestItemClickAction.showImagePopup:
				showImagePopup();
				break;
			}
			Service.Get<NotificationBreadcrumbController>().ResetBreadcrumbs(Breadcrumb.BreadcrumbId);
			Service.Get<TutorialBreadcrumbController>().RemoveBreadcrumb(TutorialBreadcrumb.BreadcrumbId);
			Service.Get<ICPSwrveService>().Action("view.adventure_log", Service.Get<QuestService>().ActiveQuest.Definition.name);
		}

		private void useConsumable()
		{
			Service.Get<PropService>().LocalPlayerRetrieveProp(item.DataModel.ClickActionArg);
		}

		private void openChatEmotes()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(ChatEvents.OpenChatBar));
		}

		private void showItemTrayScreen()
		{
			string clickActionArg = item.DataModel.ClickActionArg;
			TrayEvents.SelectTrayScreen evt;
			if (!clickActionArg.Contains("|"))
			{
				evt = new TrayEvents.SelectTrayScreen(clickActionArg);
			}
			else
			{
				clickActionArg = clickActionArg.Substring(0, clickActionArg.IndexOf("|"));
				string subScreenName = clickActionArg.Substring(clickActionArg.IndexOf("|") + 1);
				evt = new TrayEvents.SelectTrayScreen(clickActionArg, subScreenName);
			}
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void loadItemPopup()
		{
			Content.LoadAsync(onItemPopupLoaded, item.DataModel.PopupContentKey);
		}

		private void onItemPopupLoaded(string path, GameObject prefab)
		{
			GameObject popup = Object.Instantiate(prefab);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(popup, false, true, "Accessibility.Popup.Title.QuestItem"));
		}

		private void showImagePopup()
		{
			Content.LoadAsync(onImagePrefabLoaded, imagePopupContentKey);
		}

		private void onImagePrefabLoaded(string path, GameObject prefab)
		{
			DImagePopup dImagePopup = new DImagePopup();
			dImagePopup.ImageContentKey = item.DataModel.PopupImageContentKey;
			dImagePopup.Text = "";
			GameObject gameObject = Object.Instantiate(prefab);
			ImagePopup component = gameObject.GetComponent<ImagePopup>();
			if (component != null)
			{
				if (item.DataModel.PopupImageContentKey != null && !string.IsNullOrEmpty(item.DataModel.PopupImageContentKey.Key))
				{
					component.SetData(dImagePopup);
				}
				component.EnableCloseButtons(false, true);
				component.ShowBackground = true;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(gameObject, false, true, "Accessibility.Popup.Title.QuestItem"));
		}
	}
}
