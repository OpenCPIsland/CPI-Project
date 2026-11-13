using ClubPenguin.Avatar;
using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using LitJson;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class RememberMeAccountData
	{
		[Serializable]
		public struct BannedInfo
		{
			public AlertType Category;

			public DateTime? ExpirationDate;

			public BannedInfo(AlertType category, DateTime? expirationDate)
			{
				Category = category;
				ExpirationDate = expirationDate;
			}
		}

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public CacheableType<string> StoredData;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public string Username;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public KeyChainCacheableString KeychainCredential;

		public string DisplayName;

		public bool IsMember;

		public MembershipType MembershipType;

		public float _bodyColor_r = AvatarService.DefaultBodyColor.r;

		public float _bodyColor_g = AvatarService.DefaultBodyColor.g;

		public float _bodyColor_b = AvatarService.DefaultBodyColor.b;

		public DCustomEquipment[] Outfit = new DCustomEquipment[0];

		public BannedInfo? Banned;

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public string Password
		{
			get
			{
				return KeychainCredential.GetValue();
			}
			set
			{
				KeychainCredential.SetValue(value);
			}
		}

		[JsonIgnore(JsonIgnoreWhen.Serializing | JsonIgnoreWhen.Deserializing)]
		public Color BodyColor
		{
			get
			{
				return new Color(_bodyColor_r, _bodyColor_g, _bodyColor_b);
			}
			set
			{
				_bodyColor_r = value.r;
				_bodyColor_g = value.g;
				_bodyColor_b = value.b;
			}
		}

		public void Reset()
		{
			Password = string.Empty;
			DisplayName = string.Empty;
			IsMember = false;
			BodyColor = AvatarService.DefaultBodyColor;
			Outfit = new DCustomEquipment[0];
			Banned = null;
		}
	}
}
