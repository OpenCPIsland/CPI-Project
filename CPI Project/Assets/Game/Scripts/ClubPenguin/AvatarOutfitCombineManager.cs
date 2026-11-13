using ClubPenguin.Avatar;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Configuration;
using ClubPenguin.Core;
using ClubPenguin.Net;
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
		private enum LocalOutfitActionType
		{
			Load,
			Override,
			Delete
		}

		[System.Serializable]
		private class LocalOutfit
		{
			public string Name;

			public DCustomEquipment[] Equipment;
		}

		[System.Serializable]
		private class LocalOutfitCollection
		{
			public LocalOutfit[] Outfits = new LocalOutfit[0];
		}

		private class LocalOutfitSlotAction
		{
			private readonly string outfitName;

			private readonly LocalOutfitActionType actionType;

			public IInvokable Invokable;

			public LocalOutfitSlotAction(string outfitName, LocalOutfitActionType actionType)
			{
				this.outfitName = outfitName;
				this.actionType = actionType;
			}

			public void Invoke()
			{
				switch (actionType)
				{
					case LocalOutfitActionType.Load:
						LoadSavedLocalOutfit(outfitName);
						break;
					case LocalOutfitActionType.Override:
						SaveLocalOutfit(outfitName);
						break;
					case LocalOutfitActionType.Delete:
						DeleteSavedLocalOutfit(outfitName);
						break;
				}
			}
		}

		private static readonly List<LocalOutfitSlotAction> localOutfitSlotActions = new List<LocalOutfitSlotAction>();

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

		[Tweakable("Avatar.GpuSkinning")]
		public static bool UseGpuSkinning;

		[Tweakable("Avatar.CombineLocal")]
		public static bool CombineLocal;

		[Tweakable("Avatar.CombineRemote")]
		public static bool CombineRemote;

		private bool isLocal;

		private CPDataEntityCollection dataEntityCollection;

		private AvatarView avatarView;

		private TagsManager tagsManager;

		private AvatarDetailsData avatarDetailsData;

		private LoadingController loadingController;

		private DCustomOutfit currentOutfit;

		[PublicTweak]
		[Invokable("Avatar.Outfit.SaveOutfit", Description = "Saves your penguin's outfit. It will save under the outfit name that you choose.")]
		public static void SaveLocalOutfit(string name)
		{
			if (!TryGetOutfitName(name, out name))
			{
				return;
			}
			string key;
			if (!TryGetLocalOutfitsKey(out key))
			{
				return;
			}
			AvatarDetailsData avatarDetailsData;
			if (!TryGetLocalAvatarDetails(out avatarDetailsData))
			{
				return;
			}
			LocalOutfitCollection collection = LoadLocalOutfitCollection(key);
			LocalOutfit savedOutfit = collection.Outfits.FirstOrDefault((LocalOutfit outfit) => string.Equals(outfit.Name, name, System.StringComparison.OrdinalIgnoreCase));
			if (savedOutfit == null)
			{
				savedOutfit = new LocalOutfit();
				List<LocalOutfit> outfits = collection.Outfits.ToList();
				outfits.Add(savedOutfit);
				collection.Outfits = outfits.ToArray();
			}
			savedOutfit.Name = name;
			savedOutfit.Equipment = avatarDetailsData.Outfit;
			SaveLocalOutfitCollection(key, collection);
			RefreshLocalOutfitInvokables();
			UnityEngine.Debug.Log("Saved local outfit '" + name + "'.");
		}

		private static void LoadSavedLocalOutfit(string name)
		{
			string key;
			if (!TryGetLocalOutfitsKey(out key))
			{
				return;
			}
			LocalOutfit savedOutfit = LoadLocalOutfitCollection(key).Outfits.FirstOrDefault((LocalOutfit outfit) => string.Equals(outfit.Name, name, System.StringComparison.OrdinalIgnoreCase));
			if (savedOutfit == null)
			{
				UnityEngine.Debug.LogWarning("No local outfit named '" + name + "' was found for the current username.");
				return;
			}
			AvatarDetailsData avatarDetailsData;
			if (!TryGetLocalAvatarDetails(out avatarDetailsData))
			{
				return;
			}
			DCustomEquipment[] equipment = savedOutfit.Equipment ?? new DCustomEquipment[0];
			long[] equipmentIds = equipment.Select((DCustomEquipment item) => item.Id).ToArray();
			new SaveOutfitToWearCMD(equipmentIds, equipment).Execute();
			UnityEngine.Debug.Log("Loaded local outfit '" + savedOutfit.Name + "'.");
		}

		private static void DeleteSavedLocalOutfit(string name)
		{
			string key;
			if (!TryGetLocalOutfitsKey(out key))
			{
				return;
			}
			LocalOutfitCollection collection = LoadLocalOutfitCollection(key);
			List<LocalOutfit> outfits = collection.Outfits.ToList();
			int removed = outfits.RemoveAll((LocalOutfit outfit) => string.Equals(outfit.Name, name, System.StringComparison.OrdinalIgnoreCase));
			if (removed == 0)
			{
				UnityEngine.Debug.LogWarning("No local outfit named '" + name + "' was found for the current username.");
				return;
			}
			collection.Outfits = outfits.ToArray();
			SaveLocalOutfitCollection(key, collection);
			RefreshLocalOutfitInvokables();
			UnityEngine.Debug.Log("Deleted local outfit '" + name + "'.");
		}

		[PublicTweak]
		[Invokable("Avatar.Outfit.ChangeLocalOutfit", Description = "This updates the local penguin outfit removing everything but the specified template. This will not persist.")]
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

		private static bool TryGetOutfitName(string name, out string trimmedName)
		{
			trimmedName = name == null ? null : name.Trim();
			if (string.IsNullOrEmpty(trimmedName))
			{
				UnityEngine.Debug.LogWarning("An outfit name is required.");
				return false;
			}
			return true;
		}

		private static bool TryGetLocalAvatarDetails(out AvatarDetailsData details)
		{
			if (!Service.IsSet<CPDataEntityCollection>())
			{
				UnityEngine.Debug.LogWarning("Cannot access the local player's outfit right now.");
				details = null;
				return false;
			}
			CPDataEntityCollection collection = Service.Get<CPDataEntityCollection>();
			if (collection.TryGetComponent(collection.LocalPlayerHandle, out details))
			{
				return true;
			}
			UnityEngine.Debug.LogWarning("Cannot access the local player's outfit right now.");
			return false;
		}

		private static bool TryGetLocalOutfitsKey(out string key)
		{
			if (!Service.IsSet<SessionManager>())
			{
				key = null;
				UnityEngine.Debug.LogWarning("Cannot access local outfits without a current username.");
				return false;
			}
			SessionManager sessionManager = Service.Get<SessionManager>();
			if (sessionManager.LocalUser == null || sessionManager.LocalUser.RegistrationProfile == null)
			{
				key = null;
				UnityEngine.Debug.LogWarning("Cannot access local outfits without a current username.");
				return false;
			}
			string username = sessionManager.LocalUser.RegistrationProfile.Username;
			if (string.IsNullOrEmpty(username))
			{
				key = null;
				UnityEngine.Debug.LogWarning("Cannot access local outfits without a current username.");
				return false;
			}
			key = "AvatarOutfits:" + username.Trim().ToLowerInvariant();
			return true;
		}

		private static LocalOutfitCollection LoadLocalOutfitCollection(string key)
		{
			if (!PlayerPrefs.HasKey(key))
			{
				return new LocalOutfitCollection();
			}
			LocalOutfitCollection collection = JsonUtility.FromJson<LocalOutfitCollection>(PlayerPrefs.GetString(key));
			if (collection == null || collection.Outfits == null)
			{
				UnityEngine.Debug.LogWarning("Saved local outfits could not be read; starting with an empty outfit list.");
				return new LocalOutfitCollection();
			}
			return collection;
		}

		private static void SaveLocalOutfitCollection(string key, LocalOutfitCollection collection)
		{
			PlayerPrefs.SetString(key, JsonUtility.ToJson(collection));
			PlayerPrefs.Save();
		}

		private static void RefreshLocalOutfitInvokables()
		{
			if (!Service.IsSet<Tweaker.UI.TweakerConsoleController>())
			{
				return;
			}
			Tweaker.UI.TweakerConsoleController console = Service.Get<Tweaker.UI.TweakerConsoleController>();
			if (console.Tweaker == null)
			{
				return;
			}
			IInvokableManager manager = console.Tweaker.Invokables;
			string key;
			if (!TryGetLocalOutfitsKey(out key))
			{
				UnregisterLocalOutfitInvokables();
				console.Refresh();
				return;
			}
			UnregisterLocalOutfitInvokables();
			LocalOutfit[] outfits = LoadLocalOutfitCollection(key).Outfits;
			for (int i = 0; i < outfits.Length; i++)
			{
				if (outfits[i] == null || string.IsNullOrEmpty(outfits[i].Name))
				{
					UnityEngine.Debug.LogWarning("Skipping a saved outfit slot without a name.");
					continue;
				}
				string itemName = (i + 1) + "_" + outfits[i].Name.Replace(".", "_");
				RegisterLocalOutfitAction(manager, itemName + ".Load", outfits[i].Name, LocalOutfitActionType.Load);
				RegisterLocalOutfitAction(manager, itemName + ".Override", outfits[i].Name, LocalOutfitActionType.Override);
				RegisterLocalOutfitAction(manager, itemName + ".Delete", outfits[i].Name, LocalOutfitActionType.Delete);
			}
			console.Refresh();
		}

		private static void RegisterLocalOutfitAction(IInvokableManager manager, string actionName, string outfitName, LocalOutfitActionType actionType)
		{
			LocalOutfitSlotAction action = new LocalOutfitSlotAction(outfitName, actionType);
			InvokableInfo info = new InvokableInfo(
				"Avatar.Outfit.Saved." + actionName,
				0u,
				new ICustomTweakerAttribute[] { new PublicTweak() },
				actionType.ToString() + " saved outfit '" + outfitName + "'.");
			action.Invokable = manager.RegisterInvokable(info, (System.Action)action.Invoke);
			localOutfitSlotActions.Add(action);
		}

		private static void UnregisterLocalOutfitInvokables()
		{
			foreach (LocalOutfitSlotAction action in localOutfitSlotActions)
			{
				if (action.Invokable != null && action.Invokable.Manager != null)
				{
					action.Invokable.Manager.UnregisterInvokable(action.Invokable);
				}
			}
			localOutfitSlotActions.Clear();
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
				avatarViewCombined.MaxAtlasDimension = conditionalConfiguration.Get("AvatarSystem.MaxAtlasDimension.property", 4096);
				if (PlatformUtils.IsWebGL())
				{
					avatarViewCombined.MaxAtlasDimension = Mathf.Max(avatarViewCombined.MaxAtlasDimension, 4096);
				}
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
			if (handle == dataEntityCollection.LocalPlayerHandle)
			{
				RefreshLocalOutfitInvokables();
			}
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
