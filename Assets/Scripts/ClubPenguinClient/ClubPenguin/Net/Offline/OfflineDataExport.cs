using ClubPenguin.Net.Client;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Net.Offline
{
	public struct OfflineDataExport
	{
		public string UserName;

		public BreadcrumbCollection BreadCrumbs;

		public ClaimableRewardData ClaimableRewards;

		public ConsumableInventory ConsumableInventory;

		public CustomEquipmentCollection Equipment;

		public DailySpinData DailySpinData;

		public IglooEntity IglooData;

		public InRoomRewards RoomRewards;

		public PlayerAssets Assets;

		public PlayerOutfitDetails Outfit;

		public Profile Profile;

		public QuestStates QuestStates;

		public RegistrationProfile RegistrationProfile;

		public SceneLayoutEntity IglooLayouts;

		public TubeData Tube;

		public TutorialData Tutorials;

		[Invokable("Data.Export", Description = "Copy all of the account data for the given username into the clipboard")]
		[PublicTweak(2018, 12, 21)]
		public static void ExportData(string userName)
		{
			JsonService jsonService = Service.Get<JsonService>();
			string token = (!string.IsNullOrEmpty(userName)) ? RegistrationProfile.Id(userName) : Service.Get<OfflineDatabase>().AccessToken;
			OfflineDataExport offlineDataExport = default(OfflineDataExport);
			offlineDataExport.BreadCrumbs = OfflineDatabase.Read<BreadcrumbCollection>(token);
			offlineDataExport.ClaimableRewards = OfflineDatabase.Read<ClaimableRewardData>(token);
			offlineDataExport.ConsumableInventory = OfflineDatabase.Read<ConsumableInventory>(token);
			offlineDataExport.Equipment = OfflineDatabase.Read<CustomEquipmentCollection>(token);
			offlineDataExport.DailySpinData = OfflineDatabase.Read<DailySpinData>(token);
			offlineDataExport.IglooData = OfflineDatabase.Read<IglooEntity>(token);
			offlineDataExport.RoomRewards = OfflineDatabase.Read<InRoomRewards>(token);
			offlineDataExport.Assets = OfflineDatabase.Read<PlayerAssets>(token);
			offlineDataExport.Outfit = OfflineDatabase.Read<PlayerOutfitDetails>(token);
			offlineDataExport.Profile = OfflineDatabase.Read<Profile>(token);
			offlineDataExport.QuestStates = OfflineDatabase.Read<QuestStates>(token);
			offlineDataExport.RegistrationProfile = OfflineDatabase.Read<RegistrationProfile>(token);
			offlineDataExport.IglooLayouts = OfflineDatabase.Read<SceneLayoutEntity>(token);
			offlineDataExport.Tube = OfflineDatabase.Read<TubeData>(token);
			offlineDataExport.Tutorials = OfflineDatabase.Read<TutorialData>(token);
			OfflineDataExport objectToSerialize = offlineDataExport;
			objectToSerialize.UserName = objectToSerialize.RegistrationProfile.userName;
			TextEditor textEditor = new TextEditor();
			textEditor.text = jsonService.Serialize(objectToSerialize);
			textEditor.SelectAll();
			textEditor.Copy();
		}

		[PublicTweak(2018, 12, 21)]
		[Invokable("Data.Import", Description = "Sets all of the account data for the given username from the data in the clipboard")]
		public static void ImportData(string userName)
		{
			JsonService jsonService = Service.Get<JsonService>();
			TextEditor textEditor = new TextEditor();
			textEditor.Paste();
			OfflineDataExport offlineDataExport = jsonService.Deserialize<OfflineDataExport>(textEditor.text);
			if (string.IsNullOrEmpty(userName))
			{
				userName = offlineDataExport.UserName;
			}
			else
			{
				RegistrationProfile registrationProfile = OfflineDatabase.Read<RegistrationProfile>(RegistrationProfile.Id(userName));
				if (!string.IsNullOrEmpty(registrationProfile.userName))
				{
					offlineDataExport.RegistrationProfile.displayName = registrationProfile.displayName;
					offlineDataExport.RegistrationProfile.firstName = registrationProfile.firstName;
					offlineDataExport.RegistrationProfile.parentEmail = registrationProfile.parentEmail;
				}
				else
				{
					offlineDataExport.RegistrationProfile.displayName = userName;
				}
			}
			string token = RegistrationProfile.Id(userName);
			offlineDataExport.RegistrationProfile.userName = userName;
			OfflineDatabase.Write(offlineDataExport.BreadCrumbs, token);
			OfflineDatabase.Write(offlineDataExport.ClaimableRewards, token);
			OfflineDatabase.Write(offlineDataExport.ConsumableInventory, token);
			OfflineDatabase.Write(offlineDataExport.Equipment, token);
			OfflineDatabase.Write(offlineDataExport.DailySpinData, token);
			OfflineDatabase.Write(offlineDataExport.IglooData, token);
			OfflineDatabase.Write(offlineDataExport.RoomRewards, token);
			OfflineDatabase.Write(offlineDataExport.Assets, token);
			OfflineDatabase.Write(offlineDataExport.Outfit, token);
			OfflineDatabase.Write(offlineDataExport.Profile, token);
			OfflineDatabase.Write(offlineDataExport.QuestStates, token);
			OfflineDatabase.Write(offlineDataExport.RegistrationProfile, token);
			OfflineDatabase.Write(offlineDataExport.IglooLayouts, token);
			OfflineDatabase.Write(offlineDataExport.Tube, token);
			OfflineDatabase.Write(offlineDataExport.Tutorials, token);
		}
	}
}
