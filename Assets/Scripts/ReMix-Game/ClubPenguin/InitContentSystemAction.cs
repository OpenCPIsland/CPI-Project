using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitEnvironmentManagerAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitApplicationServiceAction))]
	[RequireComponent(typeof(InitLocalizerSetupAction))]
	[RequireComponent(typeof(InitGuiAction))]
	public class InitContentSystemAction : InitActionComponent
	{
		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			IGcsAccessTokenService gcsAccessTokenService = new GcsAccessTokenService(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountName"), new GcsP12AssetFileLoader(ConfigHelper.GetEnvironmentProperty<string>("GcsServiceAccountFile")));
			Service.Set(gcsAccessTokenService);
			ICPipeManifestService cpipeManifestService = new CPipeManifestService(ContentHelper.GetCdnUrl(), ContentHelper.GetCpipeMappingFilename(), gcsAccessTokenService);
			Service.Set(cpipeManifestService);
			ContentSystemManager contentSystemManager = new ContentSystemManager(Service.Get<ApplicationService>());
			Service.Set(contentSystemManager);
			yield return contentSystemManager.InitContentSystem();
		}
	}
}
