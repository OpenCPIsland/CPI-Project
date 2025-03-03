using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitFibreServiceAction))]
	[RequireComponent(typeof(InitGameDataAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	public class InitAvatarSystemAction : InitActionComponent
	{
		public string PenguinPrefabPrefix = "Penguin_";

		public AvatarDefinitionContentKey AvatarDefinitionKey;

		public AvatarDefinitionContentKey MannequinAvatarDefinitionKey;

		public bool UseGpuSkinning;

		public bool CombineLocal = true;

		public bool CombineRemote = true;

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
			AssetRequest<AvatarDefinition> avatarRequest = Content.LoadAsync(AvatarDefinitionKey);
			yield return avatarRequest;
			AssetRequest<AvatarDefinition> mannequinAvatarRequest = Content.LoadAsync(MannequinAvatarDefinitionKey);
			yield return mannequinAvatarRequest;
			AvatarColorDefinition defaultColorDefinition = Service.Get<GameData>().Get<Dictionary<int, AvatarColorDefinition>>()[0];
			Color bodyColor;
			if (ColorUtility.TryParseHtmlString("#" + defaultColorDefinition.Color, out bodyColor))
			{
				avatarRequest.Asset.BodyColor.BodyColor = bodyColor;
			}
			AvatarService avatarService = new AvatarService(new AvatarDefinition[2]
			{
				avatarRequest.Asset,
				mannequinAvatarRequest.Asset
			});
			Service.Set(avatarService);
			AvatarOutfitCombineManager.UseGpuSkinning = UseGpuSkinning;
			AvatarOutfitCombineManager.CombineLocal = CombineLocal;
			AvatarOutfitCombineManager.CombineRemote = CombineRemote;
			ManifestContentKey key = StaticGameDataUtils.GetManifestContentKey(typeof(TemporaryHeadStatusDefinition));
			AssetRequest<Manifest> assetRequest = Content.LoadAsync(key);
			yield return assetRequest;
			Manifest manifest = assetRequest.Asset;
			ScriptableObject[] assets = manifest.Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
				TemporaryHeadStatusDefinition temporaryHeadStatusDefinition = (TemporaryHeadStatusDefinition)scriptableObject;
				HeadStatusView.HeadStatusToDefinition[temporaryHeadStatusDefinition.Type] = temporaryHeadStatusDefinition;
			}
		}
	}
}
