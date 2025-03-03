using ClubPenguin.Chat;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class FullScreenChatBlock : MonoBehaviour
	{
		public Action<long> OnClicked;

		public Text MessageText;

		public Text NameText;

		public RawImage PenguinIconImage;

		public Image MembershipSprite;

		public GameObject ActiveTypingPanel;

		public GameObject BlockedTextPanel;

		public GameObject Preloader;

		public Text PlayerCardText;

		public string AccessibilityPlayerCardSpeechToken;

		public LayoutGroup PaddingLayoutGroup;

		private int emoteReduction = 6;

		public Material FontMaterialDefault;

		public Material FontMaterialWaiting;

		[SerializeField]
		private int maxEmoteString = 5;

		[SerializeField]
		private int FontSizeDefault = 24;

		[SerializeField]
		private int FontSizeSingleEmote = 72;

		[SerializeField]
		private RectOffset PaddingDefault;

		[SerializeField]
		private RectOffset PaddingSingleEmote;

		public long SessionId
		{
			get;
			set;
		}

		public void Awake()
		{
			if (PlayerCardText != null)
			{
				PlayerCardText.text = Service.Get<Localizer>().GetTokenTranslation(AccessibilityPlayerCardSpeechToken);
			}
		}

		public void SetChatMessage(string playerName, string message, bool isChatActivity, bool isAwaitingModeration = false, bool isChatBlocked = false)
		{
			if (NameText != null)
			{
				NameText.text = playerName;
			}
			if (isChatActivity)
			{
				showActiveChat();
				return;
			}
			hideActiveChat();
			showBlockedTextPanel(isChatBlocked);
			MessageText.gameObject.SetActive(!isChatBlocked);
			if (isChatBlocked)
			{
				return;
			}
			bool flag = message.Length <= maxEmoteString;
			bool flag2 = true;
			if (flag)
			{
				foreach (char character in message)
				{
					if (!EmoteManager.IsEmoteCharacter(character))
					{
						flag2 = false;
						break;
					}
				}
			}
			if (flag2 && flag)
			{
				PaddingLayoutGroup.padding = PaddingSingleEmote;
				MessageText.fontSize = FontSizeSingleEmote - (message.Length - 1) * emoteReduction;
			}
			else
			{
				PaddingLayoutGroup.padding = PaddingDefault;
				MessageText.fontSize = FontSizeDefault;
			}
			MessageText.material = FontMaterialDefault;
			if (isAwaitingModeration)
			{
				MessageText.material = FontMaterialWaiting;
			}
			MessageText.text = message;
			if (MessageText.transform.parent != null)
			{
				AccessibilitySettings component = MessageText.transform.parent.GetComponent<AccessibilitySettings>();
				if (component != null)
				{
					component.DynamicText = EmoteManager.GetMessageWithLocalizedEmotes(MessageText.text);
				}
			}
		}

		private void showActiveChat()
		{
			MessageText.gameObject.SetActive(false);
			ActiveTypingPanel.SetActive(true);
			showBlockedTextPanel(false);
		}

		private void hideActiveChat()
		{
			MessageText.gameObject.SetActive(true);
			ActiveTypingPanel.SetActive(false);
			showBlockedTextPanel(false);
		}

		private void showBlockedTextPanel(bool show)
		{
			if (BlockedTextPanel != null)
			{
				BlockedTextPanel.SetActive(show);
			}
		}

		public void SetIcon(Texture2D icon)
		{
			PenguinIconImage.texture = icon;
			PenguinIconImage.gameObject.SetActive(true);
			if (MembershipSprite != null)
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				DataEntityHandle handle = findPlayerHandle(SessionId, cPDataEntityCollection);
				MembershipData component;
				if (cPDataEntityCollection.TryGetComponent(handle, out component))
				{
					showMembershipKey(component);
				}
			}
			Preloader.SetActive(false);
		}

		public void showMembershipKey(MembershipData membershipData)
		{
			MembershipSprite.gameObject.SetActive(membershipData.IsMember ? true : false);
		}

		private DataEntityHandle findPlayerHandle(long sessionId, CPDataEntityCollection dataEntityCollection)
		{
			return dataEntityCollection.FindEntity<SessionIdData, long>(sessionId);
		}

		public void EnablePreloader()
		{
			PenguinIconImage.gameObject.SetActive(false);
			Preloader.SetActive(true);
		}

		public void OnFullScreenChatBlockClicked()
		{
			if (OnClicked != null)
			{
				OnClicked(SessionId);
			}
		}
	}
}
