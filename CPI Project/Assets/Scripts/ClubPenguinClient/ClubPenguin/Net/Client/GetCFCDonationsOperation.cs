using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.ScheduledEvent;
using ClubPenguin.Net.Offline;
using Disney.Kelowna.Common;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
	[HttpAccept("application/json")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	[HttpPath("cp-api-base-uri", "/event/v1/cfc")]
	public class GetCFCDonationsOperation : CPAPIHttpOperation
	{
		[HttpResponseJsonBody]
		public CFCDonations ResponseBody;

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			long globalCfcTotal = GetDeviceWideCFCTotal();
			
			CFCDonationData cfcData = offlineDatabase.Read<CFCDonationData>();
			
			ResponseBody = new CFCDonations
			{
				cfcTotal = globalCfcTotal,
				contribution = cfcData.personalContribution
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
