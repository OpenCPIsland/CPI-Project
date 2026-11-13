using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.TutorialUI
{
	[RequireComponent(typeof(Canvas))]
	internal class TutorialPopupManager : MonoBehaviour
	{
		private Dictionary<string, TutorialPopup> popupMap;

		private EventChannel eventChannel;

		private void Start()
		{
			popupMap = new Dictionary<string, TutorialPopup>();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<TutorialUIEvents.ShowTutorialPopup>(onShowPopup);
			eventChannel.AddListener<TutorialUIEvents.ShowTutorialPopupAtPosition>(onShowPopupAtPosition);
			eventChannel.AddListener<TutorialUIEvents.HideTutorialPopup>(onHidePopup);
			eventChannel.AddListener<TutorialUIEvents.HideAllTutorialPopups>(onHideAllPopups);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		private void showPopup(string popupID, GameObject popup, RectTransform target, Vector2 offset, float scale)
		{
			Vector2 vector = new Vector2(target.position.x, target.position.y);
			Canvas componentInParent = target.GetComponentInParent<Canvas>();
			if (componentInParent.renderMode == RenderMode.ScreenSpaceCamera)
			{
				vector = RectTransformUtility.WorldToScreenPoint(componentInParent.worldCamera, vector);
			}
			vector += offset;
			showPopupAtPosition(popupID, popup, vector, scale);
		}

		private void showPopupAtPosition(string popupID, GameObject popup, Vector2 position, float scale)
		{
			TutorialPopup component = popup.GetComponent<TutorialPopup>();
			if (component != null)
			{
				if (popupMap.ContainsKey(popupID))
				{
					hidePopup(popupID);
				}
				popupMap[popupID] = component;
				Transform parent = base.transform.Find("VertLayout/Layout");
				popup.transform.SetParent(parent, false);
				popup.GetComponent<RectTransform>().anchoredPosition = position;
				popup.transform.localScale = new Vector3(scale, scale, 1f);
			}
		}

		private void hidePopup(string popupID)
		{
			if (popupMap.ContainsKey(popupID))
			{
				popupMap[popupID].ClosePopup();
			}
			popupMap.Remove(popupID);
		}

		private void hideAllPopups()
		{
			foreach (TutorialPopup value in popupMap.Values)
			{
				value.ClosePopup();
			}
			popupMap.Clear();
		}

		private bool onShowPopup(TutorialUIEvents.ShowTutorialPopup evt)
		{
			showPopup(evt.PopupID, evt.Popup, evt.Target, evt.Offset, evt.Scale);
			return false;
		}

		private bool onShowPopupAtPosition(TutorialUIEvents.ShowTutorialPopupAtPosition evt)
		{
			showPopupAtPosition(evt.PopupID, evt.Popup, evt.Position, evt.Scale);
			return false;
		}

		private bool onHidePopup(TutorialUIEvents.HideTutorialPopup evt)
		{
			hidePopup(evt.PopupID);
			return false;
		}

		private bool onHideAllPopups(TutorialUIEvents.HideAllTutorialPopups evt)
		{
			hideAllPopups();
			return false;
		}
	}
}
