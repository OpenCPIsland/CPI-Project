using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.ScheduledEvent;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using Disney.Manimal.Common.Util;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	[HttpPOST]
	[HttpPath("cp-api-base-uri", "/event/v1/cfc/{$coins}")]
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	public class PostCFCDonationOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("coins")]
		public int Coins;

		[HttpResponseJsonBody]
		public DonationResult ResponseBody;

		public PostCFCDonationOperation(int coins)
		{
			Coins = coins;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			CFCDonationData cfcData = offlineDatabase.Read<CFCDonationData>();
			
			PlayerAssets playerAssets = offlineDatabase.Read<PlayerAssets>();
			
			long globalCfcTotal = GetDeviceWideCFCTotal();
			globalCfcTotal += Coins;
			SetDeviceWideCFCTotal(globalCfcTotal);
			
			cfcData.personalContribution += Coins;
			cfcData.donationCount++;
			cfcData.lastDonationTime = DateTime.UtcNow.GetTimeInMilliseconds();
			
			playerAssets.Assets.coins = System.Math.Max(0, playerAssets.Assets.coins - Coins);
			cfcData.coinBalance = playerAssets.Assets.coins;
			
			offlineDatabase.Write(cfcData);
			offlineDatabase.Write(playerAssets);
			
			ResponseBody = new DonationResult
			{
				cfcTotal = globalCfcTotal,
				remainingCoinBalance = playerAssets.Assets.coins,
				reward = null
			};
		}

		private static long GetDeviceWideCFCTotal()
		{
			string storedValue = PlayerPrefs.GetString(GetPlatformKey("ol.CFCDonationTotal.device"), "0");
			if (long.TryParse(storedValue, out long total))
			{
				return total;
			}
			return 0;
		}

		private static void SetDeviceWideCFCTotal(long total)
		{
			if (total > 999999999)
			{
				total = 999999999;
			}
			PlayerPrefs.SetString(GetPlatformKey("ol.CFCDonationTotal.device"), total.ToString());
			PlayerPrefs.Save();
		}

		private static string GetPlatformKey(string key)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (UnityEngine.Application.isEditor)
			{
				return "Editor_" + key;
			}
#endif
			return key;
		}
	}
}
