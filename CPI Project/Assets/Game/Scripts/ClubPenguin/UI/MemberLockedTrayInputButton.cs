using ClubPenguin.Core;
using ClubPenguin.Input;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MemberLockedTrayInputButton : MonoBehaviour
	{
		public PrefabContentKey MemberLockContentKey;

		public Sprite IconSprite;

		[SerializeField]
		private bool isLocked;

		private TrayInputButton trayInputButton;

		private MembershipData membershipData;

		public Image IconImage
		{
			get;
			private set;
		}

		public bool IsLocked
		{
			get
			{
				return isLocked;
			}
			set
			{
				if (isLocked == value)
				{
					return;
				}
				isLocked = value;
				if (MemberLock != null)
				{
					MemberLock.SetActive(isLocked);
				}
				if (!(trayInputButton != null))
				{
					return;
				}
				if (isLocked)
				{
					trayInputButton.Lock(TrayInputButton.ButtonState.Disabled);
					return;
				}
				UIElementDisabler componentInParent = GetComponentInParent<UIElementDisabler>();
				if (componentInParent == null || componentInParent.IsEnabled)
				{
					trayInputButton.Unlock();
				}
			}
		}

		public bool IsPlayerAMember
		{
			get;
			private set;
		}

		public GameObject MemberLock
		{
			get;
			private set;
		}

		private void Start()
		{
			trayInputButton = GetComponentInParent<TrayInputButton>();
			if (trayInputButton == null)
			{
				Log.LogError(this, "Could not find TrayInputButton in parent");
			}
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out membershipData))
			{
				IsPlayerAMember = membershipData.IsMember;
				IsLocked = !membershipData.IsMember;
				membershipData.MembershipDataUpdated += onMembershipDataUpdated;
				if (IsLocked)
				{
					Content.LoadAsync(onMemberLockLoaded, MemberLockContentKey);
				}
			}
			else
			{
				trayInputButton.Lock(TrayInputButton.ButtonState.Disabled);
				Content.LoadAsync(onMemberLockLoaded, MemberLockContentKey);
			}
		}

		private void onMembershipDataUpdated(MembershipData updatedMembershipData)
		{
			IsPlayerAMember = updatedMembershipData.IsMember;
			IsLocked = !updatedMembershipData.IsMember;
			if (!updatedMembershipData.IsMember && MemberLock == null)
			{
				Content.LoadAsync(onMemberLockLoaded, MemberLockContentKey);
			}
		}

		private void onMemberLockLoaded(string path, GameObject memberLockPrefab)
		{
			if (trayInputButton != null)
			{
				MemberLock = Object.Instantiate(memberLockPrefab, trayInputButton.transform.parent);
				MemberLock.SetActive(isLocked);
				MemberLock.GetComponent<ButtonClickListener>().OnClick.AddListener(onClicked);
				IconImage = MemberLock.transform.Find("Icon").GetComponent<Image>();
				IconImage.sprite = IconSprite;
				IconImage.enabled = true;
				if (!isLocked)
				{
					trayInputButton.Unlock();
				}
			}
		}

		private void onClicked(ButtonClickListener.ClickType clickType)
		{
			MemberAccess component = GetComponent<MemberAccess>();
			if (component == null || component.IsMemberLocked)
			{
				string trigger = base.gameObject.name.Replace("(Clone)", "");
				Service.Get<GameStateController>().ShowAccountSystemMembership(trigger);
			}
		}

		private void OnDestroy()
		{
			if (membershipData != null)
			{
				membershipData.MembershipDataUpdated -= onMembershipDataUpdated;
			}
			Object.Destroy(MemberLock);
		}
	}
}
