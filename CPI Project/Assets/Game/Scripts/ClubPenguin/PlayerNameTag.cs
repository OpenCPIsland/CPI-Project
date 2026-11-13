using ClubPenguin.UI;
using Disney.Kelowna.Common.DataModel;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class PlayerNameTag : MonoBehaviour
	{
		public enum Type
		{
			LocalPlayer,
			RemotePlayer,
			Friend
		}

		public Text NameText;

		public Image StatusIcon;

		public Image FriendIcon;

		public Color LocalPlayerTextColor;

		public Color RemotePlayerTextColor;

		public Color FriendPlayerTextColor;

		private bool isActive = true;

		private bool isIconActive;

		public Type CurrentType
		{
			get;
			private set;
		}

		public DataEntityHandle Handle
		{
			get;
			set;
		}

		public bool IsActive
		{
			get
			{
				return isActive;
			}
		}

		public void SetNameText(string playerName)
		{
			NameText.text = playerName;
		}

		public void SetNameTagType(Type type)
		{
			Outline component = NameText.GetComponent<Outline>();
			FriendIcon.gameObject.SetActive(false);
			switch (type)
			{
			case Type.LocalPlayer:
				component.effectColor = LocalPlayerTextColor;
				break;
			case Type.RemotePlayer:
				component.effectColor = RemotePlayerTextColor;
				break;
			case Type.Friend:
				component.effectColor = FriendPlayerTextColor;
				FriendIcon.gameObject.SetActive(isActive);
				break;
			}
			CurrentType = type;
		}

		public void SetStatusIcon(Sprite icon)
		{
			StatusIcon.sprite = icon;
			StatusIcon.gameObject.SetActive(isActive);
			isIconActive = true;
			FriendIcon.gameObject.SetActive(false);
		}

		public void HideStatusIcon()
		{
			StatusIcon.gameObject.SetActive(false);
			isIconActive = false;
			FriendIcon.gameObject.SetActive(CurrentType == Type.Friend && isActive);
		}

		public void SetActive(bool isActive)
		{
			this.isActive = isActive;
			NameText.gameObject.SetActive(isActive);
			StatusIcon.gameObject.SetActive(isActive && isIconActive);
			FriendIcon.gameObject.SetActive(isActive && !isIconActive && CurrentType == Type.Friend);
		}

		public void OpenPlayerCard()
		{
			if (!Handle.IsNull)
			{
				OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(Handle);
				openPlayerCardCommand.Execute();
			}
		}
	}
}
