using ClubPenguin.Net.Client;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Net.Offline
{
	public struct OfflineDataExport
	{
		private const int JETPACK_GOAL_TYPE_COUNT = 7;

		private const int JETPACK_GOAL_DURATION_COUNT = 2;

		private const int JETPACK_ACTIVE_GOAL_COUNT = 3;

		private static readonly string[] JETPACK_GOAL_TYPES =
		{
			"DISTANCE_TRAVELLED",
			"COLLECT_COINS",
			"USE_TURBO",
			"DESTROY_OBSTACLES",
			"PLAY_GAMES",
			"COLLECT_ROBOTS",
			"LOSE_ROBOTS"
		};

		private static readonly string[] JETPACK_GOAL_DURATIONS =
		{
			"SINGLE_RUN",
			"MULTIPLE_RUNS"
		};

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

		public DecorationInventoryEntity DecorationInventory;

		public TubeData Tube;

		public TutorialData Tutorials;

		public PlayTimeData Playtime;

		public DailyTaskProgress DailyTaskProgress;

		public ClothingCatalogOfflineData ClothingCatalog;

		public long FishingDailyLimitDay;

		public int FishingDailyLimitPlayCount;

		public JetpackRebootOfflineData JetpackReboot;

		public string SavedLocalOutfits;

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
			offlineDataExport.DailyTaskProgress = OfflineDatabase.Read<DailyTaskProgress>(token);
			offlineDataExport.ClothingCatalog = OfflineDatabase.Read<ClothingCatalogOfflineData>("clothing_catalog_offline");
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
			offlineDataExport.DecorationInventory = OfflineDatabase.Read<DecorationInventoryEntity>(token);
			offlineDataExport.Tube = OfflineDatabase.Read<TubeData>(token);
			offlineDataExport.Tutorials = OfflineDatabase.Read<TutorialData>(token);
			offlineDataExport.Playtime = OfflineDatabase.Read<PlayTimeData>(token);
			OfflineDataExport objectToSerialize = offlineDataExport;
			objectToSerialize.UserName = !string.IsNullOrEmpty(objectToSerialize.RegistrationProfile.userName)
				? objectToSerialize.RegistrationProfile.userName
				: userName;
			if (!string.IsNullOrEmpty(objectToSerialize.UserName))
			{
				string outfitsKey = GetLocalOutfitsPlayerPrefsKey(objectToSerialize.UserName);
				if (PlayerPrefs.HasKey(outfitsKey))
				{
					objectToSerialize.SavedLocalOutfits = PlayerPrefs.GetString(outfitsKey);
				}
				string dayKey = GetPlatformKey("OfflineMinigameProgress_Day_" + objectToSerialize.UserName);
				string countKey = GetPlatformKey("OfflineMinigameProgress_fishing_" + objectToSerialize.UserName);
				long.TryParse(PlayerPrefs.GetString(dayKey, "0"), out objectToSerialize.FishingDailyLimitDay);
				objectToSerialize.FishingDailyLimitPlayCount = PlayerPrefs.GetInt(countKey, 0);
				objectToSerialize.JetpackReboot = ReadJetpackRebootData(objectToSerialize.UserName);
			}
#if UNITY_WEBGL
            ClipboardBridge.Instance.Copy(jsonService.Serialize(objectToSerialize));
#else
			TextEditor textEditor = new TextEditor();
			textEditor.text = jsonService.Serialize(objectToSerialize);
			textEditor.SelectAll();
			textEditor.Copy();
#endif
        }

		[PublicTweak(2018, 12, 21)]
		[Invokable("Data.Import", Description = "Sets all of the account data for the given username from the data in the clipboard")]
		public static void ImportData(string userName)
		{
#if UNITY_WEBGL
            if (ClipboardBridge.Instance == null)
            {
                Debug.LogError("ClipboardBridge missing");
                return;
            }

            ClipboardBridge.Instance.RequestPaste(text =>
            {
                if (string.IsNullOrEmpty(text))
                {
                    Debug.LogError("Clipboard empty");
                    return;
                }

                try
                {
                    OnClipboardReceived(text, userName);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            });
#else
			JsonService jsonService = Service.Get<JsonService>();
			TextEditor textEditor = new TextEditor();
			textEditor.Paste();
			            OfflineDataExport offlineDataExport = jsonService.Deserialize<OfflineDataExport>(textEditor.text);
			WriteImportedData(offlineDataExport, userName);
#endif
        }

#if UNITY_WEBGL
        private static void OnClipboardReceived(string text, string userName)
        {
            JsonService jsonService = Service.Get<JsonService>();

            OfflineDataExport offlineDataExport =
                jsonService.Deserialize<OfflineDataExport>(text);

            WriteImportedData(offlineDataExport, userName);
        }
#else
#endif

		private static void WriteImportedData(OfflineDataExport offlineDataExport, string userName)
		{
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
			OfflineDatabase.Write(offlineDataExport.DecorationInventory, token);
			OfflineDatabase.Write(offlineDataExport.Tube, token);
			OfflineDatabase.Write(offlineDataExport.Tutorials, token);
			OfflineDatabase.Write(offlineDataExport.Playtime, token);

			if (offlineDataExport.DailyTaskProgress.Tasks != null)
			{
				OfflineDatabase.Write(offlineDataExport.DailyTaskProgress, token);
			}
			if (offlineDataExport.ClothingCatalog.MarketplaceItems != null ||
				offlineDataExport.ClothingCatalog.SubmittedItemByChallenge != null ||
				offlineDataExport.ClothingCatalog.UserCurrentSubmissionByChallenge != null)
			{
				ClothingCatalogOfflineData clothingCatalog = offlineDataExport.ClothingCatalog;
				ClothingCatalogOfflineData.EnsureSchema(ref clothingCatalog);
				if (!string.IsNullOrEmpty(offlineDataExport.UserName) &&
					!string.Equals(offlineDataExport.UserName, userName, StringComparison.Ordinal))
				{
					string oldPrefix = offlineDataExport.UserName + ":";
					string newPrefix = userName + ":";
					Dictionary<string, long> remappedSubmissions = new Dictionary<string, long>();
					foreach (KeyValuePair<string, long> submission in clothingCatalog.UserCurrentSubmissionByChallenge)
					{
						if (submission.Key.StartsWith(oldPrefix, StringComparison.Ordinal))
						{
							remappedSubmissions[newPrefix + submission.Key.Substring(oldPrefix.Length)] = submission.Value;
						}
					}
					foreach (KeyValuePair<string, long> submission in remappedSubmissions)
					{
						clothingCatalog.UserCurrentSubmissionByChallenge[submission.Key] = submission.Value;
					}
				}
				OfflineDatabase.Write(clothingCatalog, "clothing_catalog_offline");
			}
			if (offlineDataExport.FishingDailyLimitDay > 0L)
			{
				PlayerPrefs.SetString(GetPlatformKey("OfflineMinigameProgress_Day_" + userName), offlineDataExport.FishingDailyLimitDay.ToString());
				PlayerPrefs.SetInt(GetPlatformKey("OfflineMinigameProgress_fishing_" + userName), offlineDataExport.FishingDailyLimitPlayCount);
			}
			if (!string.IsNullOrEmpty(offlineDataExport.SavedLocalOutfits))
			{
				PlayerPrefs.SetString(GetLocalOutfitsPlayerPrefsKey(userName), offlineDataExport.SavedLocalOutfits);
			}
			WriteJetpackRebootData(offlineDataExport.JetpackReboot, userName);
			PlayerPrefs.Save();
		}

		private static JetpackRebootOfflineData ReadJetpackRebootData(string userName)
		{
			JetpackRebootOfflineData data = new JetpackRebootOfflineData
			{
				GoalLevels = new int[JETPACK_GOAL_TYPE_COUNT * JETPACK_GOAL_DURATION_COUNT],
				GoalProgress = new float[JETPACK_GOAL_TYPE_COUNT * JETPACK_GOAL_DURATION_COUNT],
				ActiveGoalDurations = new int[JETPACK_ACTIVE_GOAL_COUNT],
				ActiveGoalTypes = new int[JETPACK_ACTIVE_GOAL_COUNT]
			};

			data.HasData |= TryReadJetpackInt(userName, "mg_jr_BestDistance", "mg_jr_BestDistance", out data.BestDistance);
			data.HasData |= TryReadJetpackInt(userName, "mg_jr_MostCoins", "mg_jr_MostCoins", out data.MostCoins);
			for (int goalType = 0; goalType < JETPACK_GOAL_TYPE_COUNT; goalType++)
			{
				for (int duration = 0; duration < JETPACK_GOAL_DURATION_COUNT; duration++)
				{
					string goalName = JETPACK_GOAL_TYPES[goalType] + "_" + JETPACK_GOAL_DURATIONS[duration];
					int index = goalType * JETPACK_GOAL_DURATION_COUNT + duration;
					data.HasData |= TryReadJetpackInt(userName, goalName + "_level", "mg_jr_" + goalName + "_level", out data.GoalLevels[index]);
					data.HasData |= TryReadJetpackFloat(userName, goalName + "_progress", "mg_jr_" + goalName + "_progress", out data.GoalProgress[index]);
				}
			}
			for (int i = 0; i < JETPACK_ACTIVE_GOAL_COUNT; i++)
			{
				string durationKey = "activeGoal_number_" + i + "_duration";
				string typeKey = "activeGoal_number_" + i + "_progress";
				data.HasData |= TryReadJetpackInt(userName, durationKey, "mg_jr_" + durationKey, out data.ActiveGoalDurations[i]);
				data.HasData |= TryReadJetpackInt(userName, typeKey, "mg_jr_" + typeKey, out data.ActiveGoalTypes[i]);
			}
			return data;
		}

		private static void WriteJetpackRebootData(JetpackRebootOfflineData data, string userName)
		{
			if (!data.HasData)
			{
				return;
			}
			if (string.IsNullOrEmpty(userName))
			{
				Debug.LogError("Jetpack Reboot data requires an account username; it was not imported.");
				return;
			}

			if (data.GoalLevels == null || data.GoalLevels.Length != JETPACK_GOAL_TYPE_COUNT * JETPACK_GOAL_DURATION_COUNT ||
				data.GoalProgress == null || data.GoalProgress.Length != JETPACK_GOAL_TYPE_COUNT * JETPACK_GOAL_DURATION_COUNT ||
				data.ActiveGoalDurations == null || data.ActiveGoalDurations.Length != JETPACK_ACTIVE_GOAL_COUNT ||
				data.ActiveGoalTypes == null || data.ActiveGoalTypes.Length != JETPACK_ACTIVE_GOAL_COUNT)
			{
				Debug.LogError("Jetpack Reboot data is incomplete or invalid; it was not imported.");
				return;
			}

			PlayerPrefs.SetInt(GetJetpackPlayerPrefsKey(userName, "mg_jr_BestDistance"), data.BestDistance);
			PlayerPrefs.SetInt(GetJetpackPlayerPrefsKey(userName, "mg_jr_MostCoins"), data.MostCoins);
			for (int goalType = 0; goalType < JETPACK_GOAL_TYPE_COUNT; goalType++)
			{
				for (int duration = 0; duration < JETPACK_GOAL_DURATION_COUNT; duration++)
				{
					string goalName = JETPACK_GOAL_TYPES[goalType] + "_" + JETPACK_GOAL_DURATIONS[duration];
					int index = goalType * JETPACK_GOAL_DURATION_COUNT + duration;
					PlayerPrefs.SetInt(GetJetpackPlayerPrefsKey(userName, goalName + "_level"), data.GoalLevels[index]);
					PlayerPrefs.SetFloat(GetJetpackPlayerPrefsKey(userName, goalName + "_progress"), data.GoalProgress[index]);
				}
			}
			for (int i = 0; i < JETPACK_ACTIVE_GOAL_COUNT; i++)
			{
				PlayerPrefs.SetInt(GetJetpackPlayerPrefsKey(userName, "activeGoal_number_" + i + "_duration"), data.ActiveGoalDurations[i]);
				PlayerPrefs.SetInt(GetJetpackPlayerPrefsKey(userName, "activeGoal_number_" + i + "_progress"), data.ActiveGoalTypes[i]);
			}
			PlayerPrefs.SetInt(GetJetpackPlayerPrefsKey(userName, "mg_jr_PlayerScopedMigration_v2"), 1);
		}

		private static bool TryReadJetpackInt(string userName, string scopedKey, string legacyKey, out int value)
		{
			string playerKey = GetJetpackPlayerPrefsKey(userName, scopedKey);
			if (PlayerPrefs.HasKey(playerKey))
			{
				value = PlayerPrefs.GetInt(playerKey);
				return true;
			}
			string oldKey = GetPlatformKey(legacyKey);
			if (PlayerPrefs.HasKey(oldKey))
			{
				value = PlayerPrefs.GetInt(oldKey);
				return true;
			}
			value = 0;
			return false;
		}

		private static bool TryReadJetpackFloat(string userName, string scopedKey, string legacyKey, out float value)
		{
			string playerKey = GetJetpackPlayerPrefsKey(userName, scopedKey);
			if (PlayerPrefs.HasKey(playerKey))
			{
				value = PlayerPrefs.GetFloat(playerKey);
				return true;
			}
			string oldKey = GetPlatformKey(legacyKey);
			if (PlayerPrefs.HasKey(oldKey))
			{
				value = PlayerPrefs.GetFloat(oldKey);
				return true;
			}
			value = 0f;
			return false;
		}

		private static string GetJetpackPlayerPrefsKey(string userName, string key)
		{
			string hash;
			using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
			{
				byte[] bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userName.ToLowerInvariant()));
				hash = BitConverter.ToString(bytes).Replace("-", string.Empty);
			}
			return GetPlatformKey("mg_jr_" + hash + "_" + key);
		}

		private static string GetPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}

		private static string GetLocalOutfitsPlayerPrefsKey(string userName)
		{
			return "AvatarOutfits:" + userName.Trim().ToLowerInvariant();
		}
	}

	[Serializable]
	public struct JetpackRebootOfflineData
	{
		public bool HasData;

		public int BestDistance;

		public int MostCoins;

		public int[] GoalLevels;

		public float[] GoalProgress;

		public int[] ActiveGoalDurations;

		public int[] ActiveGoalTypes;
	}
}
