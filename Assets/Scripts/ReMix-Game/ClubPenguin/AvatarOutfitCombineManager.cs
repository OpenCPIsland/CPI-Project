using ClubPenguin.Avatar;
using ClubPenguin.Configuration;
using ClubPenguin.Core;
using ClubPenguin.Tags;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public class AvatarOutfitCombineManager : AvatarController
	{
		public class EquipmentNameGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
				if (Service.IsSet<GameData>())
				{
					Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
					foreach (KeyValuePair<int, TemplateDefinition> item in dictionary)
					{
						list.Add(new NamedToggleValueAttribute.NamedToggleValue(item.Value.AssetName, item.Value.Id));
					}
				}
				return list.OrderBy((NamedToggleValueAttribute.NamedToggleValue x) => x.Name).ToList();
			}
		}

		[Tweakable("Avatar.Gpu Skinning")]
		public static bool UseGpuSkinning;

		[Tweakable("Avatar.Combine Local")]
		public static bool CombineLocal;

		[Tweakable("Avatar.Combine Remote")]
		public static bool CombineRemote;

		private bool isLocal;

		private CPDataEntityCollection dataEntityCollection;

		private AvatarView avatarView;

		private TagsManager tagsManager;

		private AvatarDetailsData avatarDetailsData;

		private LoadingController loadingController;

		private DCustomOutfit currentOutfit;

		[PublicTweak]
		[Invokable("Avatar.Outfit.Change Local Outfit", Description = "This updates the local penguin outfit removing everything but the specified template. This will not persist.")]
		public static void ChangePenguinOutfit([NamedToggleValue(typeof(EquipmentNameGenerator), 0u)] int definitionId)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			AvatarDetailsData component;
			if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
				TemplateDefinition templateDefinition = dictionary[definitionId];
				AvatarDefinition definitionByName = Service.Get<AvatarService>().GetDefinitionByName("penguinAvatar");
				EquipmentModelDefinition equipmentDefinition = definitionByName.GetEquipmentDefinition(templateDefinition.AssetName);
				DCustomEquipment dCustomEquipment = default(DCustomEquipment);
				dCustomEquipment.DefinitionId = templateDefinition.Id;
				dCustomEquipment.Name = equipmentDefinition.Name;
				dCustomEquipment.Id = 1L + (long)definitionId;
				dCustomEquipment.Parts = new DCustomEquipmentPart[equipmentDefinition.Parts.Length];
				for (int i = 0; i < equipmentDefinition.Parts.Length; i++)
				{
					dCustomEquipment.Parts[i].SlotIndex = equipmentDefinition.Parts[i].SlotIndex;
					DCustomEquipmentDecal[] decals = new DCustomEquipmentDecal[0];
					dCustomEquipment.Parts[i].Decals = decals;
				}
				component.Outfit = new DCustomEquipment[1]
				{
					dCustomEquipment
				};
			}
		}

		public override void Awake()
		{
			base.Awake();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			tagsManager = Service.Get<TagsManager>();
			loadingController = Service.Get<LoadingController>();
		}

		public void OnDestroy()
		{
			if (avatarDetailsData != null)
			{
				avatarDetailsData.PlayerOutfitChanged -= onPlayerOutfitChanged;
				avatarDetailsData.PlayerColorChanged -= onPlayerColorChanged;
			}
			if (avatarView != null)
			{
				avatarView.OnBusy -= avatarView_OnBusy;
				avatarView.OnReady -= avatarView_OnReady;
			}
			if (loadingController != null && loadingController.HasLoadingSystem(this))
			{
				loadingController.RemoveLoadingSystem(this);
			}
		}

		public void Start()
		{
			ConditionalConfiguration conditionalConfiguration = Service.Get<ConditionalConfiguration>();
			Model.LodLevel = conditionalConfiguration.Get("AvatarSystem.LODLevel.property", 0);
			ZoneLocalPlayerManager zoneLocalPlayerManager = SceneRefs.ZoneLocalPlayerManager;
			isLocal = (zoneLocalPlayerManager.LocalPlayerGameObject == base.gameObject);
			if (isLocal ? CombineLocal : CombineRemote)
			{
				base.gameObject.SetActive(false);
				AvatarViewCombined avatarViewCombined = base.gameObject.AddComponent<AvatarViewCombined>();
				avatarViewCombined.UseGpuSkinning = UseGpuSkinning;
				avatarViewCombined.MaxAtlasDimension = conditionalConfiguration.Get("AvatarSystem.MaxAtlasDimension.property", 512);
				base.gameObject.SetActive(true);
				avatarView = avatarViewCombined;
			}
			else
			{
				avatarView = base.gameObject.AddComponent<AvatarViewDistinct>();
			}
			if (isLocal)
			{
				zoneLocalPlayerManager.AssignAvatarView(avatarView);
			}
			base.gameObject.AddComponent<PlayerMediator>();
			avatarView.OnBusy += avatarView_OnBusy;
			avatarView.OnReady += avatarView_OnReady;
			bool flag = true;
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && dataEntityCollection.TryGetComponent(handle, out avatarDetailsData))
			{
				processAvatarDetails(handle);
				flag = false;
			}
			if (flag)
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<AvatarDetailsData>>(onAvatarDetailsAdded);
				Model.ClearAllEquipment();
			}
		}

		private bool onAvatarDetailsAdded(DataEntityEvents.ComponentAddedEvent<AvatarDetailsData> evt)
		{
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && evt.Handle == handle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<AvatarDetailsData>>(onAvatarDetailsAdded);
				avatarDetailsData = evt.Component;
				processAvatarDetails(handle);
			}
			return false;
		}

		private void processAvatarDetails(DataEntityHandle handle)
		{
			if (avatarDetailsData.Outfit != null)
			{
				onPlayerOutfitChanged(avatarDetailsData.Outfit);
			}
			onPlayerColorChanged(avatarDetailsData.BodyColor);
			avatarDetailsData.PlayerOutfitChanged += onPlayerOutfitChanged;
			avatarDetailsData.PlayerColorChanged += onPlayerColorChanged;
			if (handle != dataEntityCollection.LocalPlayerHandle)
			{
				SceneRefs.ZoneRemotePlayerManager.AssignAvatarView(avatarView, handle, base.gameObject);
			}
		}

		private void onPlayerOutfitChanged(DCustomEquipment[] outfit)
		{
			currentOutfit = default(DCustomOutfit);
			currentOutfit.Equipment = outfit;
			Model.ClearAllEquipment();
			Model.ApplyOutfit(currentOutfit);
			List<TagDefinition[]> list = new List<TagDefinition[]>();
			if (outfit != null)
			{
				for (int i = 0; i < outfit.Length; i++)
				{
					TagDefinition[] equipmentTags = tagsManager.GetEquipmentTags(outfit[i]);
					if (equipmentTags.Length > 0)
					{
						list.Add(equipmentTags);
					}
				}
			}
			tagsManager.MakeTagsData(base.gameObject, list.ToArray());
		}

		private void onPlayerColorChanged(Color color)
		{
			Model.BodyColor = color;
		}

		private void avatarView_OnReady(AvatarBaseAsync view)
		{
			if (loadingController.HasLoadingSystem(this))
			{
				loadingController.RemoveLoadingSystem(this);
			}
		}

		private void avatarView_OnBusy(AvatarBaseAsync view)
		{
			if (isLocal && loadingController.IsVisible)
			{
				loadingController.AddLoadingSystem(this);
			}
		}

		[Conditional("DUMP_OUTFITS")]
		private void dumpOutfit()
		{
			string text = Path.Combine(Application.persistentDataPath, "Outfits");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string path = currentOutfit.GetFullHash().ToString("x8") + "." + base.name + ".json";
			string path2 = Path.Combine(text, path);
			File.WriteAllText(path2, JsonUtility.ToJson(currentOutfit));
			Material sharedMaterial = avatarView.GetComponent<Renderer>().sharedMaterial;
			string path3 = Path.ChangeExtension(path2, "png");
			if (sharedMaterial.shader.name == "CpRemix/Combined Avatar")
			{
				RenderTexture renderTexture = sharedMaterial.mainTexture as RenderTexture;
				Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = renderTexture;
				texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
				texture2D.Apply();
				RenderTexture.active = active;
				File.WriteAllBytes(path3, texture2D.EncodeToPNG());
				Object.DestroyImmediate(texture2D);
			}
			else if (File.Exists(path3))
			{
				File.Delete(path3);
			}
		}
	}
}
