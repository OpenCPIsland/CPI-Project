using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.Mix.SDK.Internal;
using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Mix
{
	internal class OfflineAgeBandBuilder : IAgeBandBuilder
	{
		public void Build(SiteConfigurationData siteConfig, int age, string languageCode, bool registration, Action<IGetAgeBandResult> callback)
		{
			CoroutineRunner.StartPersistent(returnAdult(callback), this, "return adult");
		}

		public void Build(SiteConfigurationData siteConfig, string ageBandKey, int age, string languageCode, bool registration, Action<IGetAgeBandResult> callback)
		{
			CoroutineRunner.StartPersistent(returnAdult(callback), this, "return adult");
		}

		private IEnumerator returnAdult(Action<IGetAgeBandResult> callback)
		{
			yield return null;
			callback.InvokeSafe(new GetAgeBandResult(true, GenerateOfflineAgeBand()));
		}

		public static IAgeBand GenerateOfflineAgeBand()
		{
			return new AgeBand(null, new List<ILegalDocument>(), null, null, AgeBandType.Adult, "");
		}
	}
}
