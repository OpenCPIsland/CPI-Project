using ClubPenguin.Adventure;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.Task;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	public class InitAdventureSystemAction : InitActionComponent
	{
		public PersistentBreadcrumbTypeDefinitionKey BreadcrumbType;

		private ManifestContentKey mascotsContentKey = new ManifestContentKey("Adventure/Mascots.Manifest");

		private ManifestContentKey questsContentKey = new ManifestContentKey("Adventure/Quests.Manifest");

		private ManifestContentKey tasksContentKey = new ManifestContentKey("Adventure/Tasks.Manifest");

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
			AssetRequest<Manifest> manifest1 = Content.LoadAsync(mascotsContentKey);
			AssetRequest<Manifest> manifest2 = Content.LoadAsync(questsContentKey);
			AssetRequest<Manifest> manifest3 = Content.LoadAsync(tasksContentKey);
			yield return manifest1;
			yield return manifest2;
			yield return manifest3;
			Service.Set(new MascotService(manifest1.Asset));
			Service.Set(new QuestService(manifest2.Asset, BreadcrumbType));
			Service.Set(new TaskService(manifest3.Asset));
			Service.Set(new ZonePathing());
		}
	}
}
