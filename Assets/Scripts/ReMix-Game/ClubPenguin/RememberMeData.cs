using ClubPenguin.Avatar;
using ClubPenguin.Net;
using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class RememberMeData : ScopedData
	{
		public RememberMeAccountData AccountData;

		public string GeneralErrorMessage;

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Application.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(RememberMeDataMonoBehaviour);
			}
		}

		public event Action<RememberMeAccountData> OnAccountDataUpdated;

		protected override void notifyWillBeDestroyed()
		{
			this.OnAccountDataUpdated = null;
		}

		public void DisplayNameChanged(string displayName)
		{
			if (isForCurrentData() && !string.IsNullOrEmpty(displayName) && AccountData.DisplayName != displayName)
			{
				AccountData.DisplayName = displayName;
				accountDataUpdated();
			}
		}

		public void MembershipChanged(bool isMember, MembershipType membershipType)
		{
			if (isForCurrentData())
			{
				bool flag = false;
				if (isMember != AccountData.IsMember)
				{
					AccountData.IsMember = isMember;
					flag = true;
				}
				if (membershipType != AccountData.MembershipType)
				{
					AccountData.MembershipType = membershipType;
					flag = true;
				}
				if (flag)
				{
					accountDataUpdated();
				}
			}
		}

		public void BodyColorChanged(Color bodyColor)
		{
			if (isForCurrentData() && bodyColor != AvatarService.DefaultBodyColor && AccountData.BodyColor != bodyColor)
			{
				AccountData.BodyColor = bodyColor;
				accountDataUpdated();
			}
		}

		public void OnAccountBanned(AlertType category, DateTime? expirationDate)
		{
			ILocalUser localUser = Service.Get<SessionManager>().LocalUser;
			if (localUser == null || isForCurrentData())
			{
				AccountData.Banned = new RememberMeAccountData.BannedInfo(category, expirationDate);
				accountDataUpdated();
			}
		}

		public void ResetAccountBan()
		{
			AccountData.Banned = null;
			accountDataUpdated();
		}

		public void OutfitChanged(DCustomEquipment[] outfit)
		{
			if (isForCurrentData() && hasOutfitChanged(outfit))
			{
				AccountData.Outfit = outfit;
				accountDataUpdated();
			}
		}

		private bool isForCurrentData()
		{
			string a = string.Empty;
			ILocalUser localUser = Service.Get<SessionManager>().LocalUser;
			if (localUser != null && localUser.RegistrationProfile != null)
			{
				a = localUser.RegistrationProfile.Username;
			}
			return AccountData != null && a == AccountData.Username;
		}

		private bool hasOutfitChanged(DCustomEquipment[] outfit)
		{
			bool flag = false;
			if (outfit != null)
			{
				flag = (AccountData.Outfit == null || outfit.Length != AccountData.Outfit.Length);
				if (!flag)
				{
					for (int i = 0; i < outfit.Length; i++)
					{
						flag = (AccountData.Outfit[i].GetFullHash() != outfit[i].GetFullHash());
						if (flag)
						{
							break;
						}
					}
				}
			}
			return flag;
		}

		private void accountDataUpdated()
		{
			if (this.OnAccountDataUpdated != null)
			{
				this.OnAccountDataUpdated(AccountData);
			}
		}
	}
}
