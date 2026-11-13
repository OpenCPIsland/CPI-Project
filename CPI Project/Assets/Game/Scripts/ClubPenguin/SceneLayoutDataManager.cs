using ClubPenguin.Core;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Domain.Scene;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class SceneLayoutDataManager
	{
		private CPDataEntityCollection dataEntityCollection;

		public SceneLayoutDataManager()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		public DataEntityHandle GetActiveHandle()
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			if (dataEntityHandle.IsNull)
			{
				dataEntityHandle = dataEntityCollection.AddEntity("ActiveSceneData");
			}
			return dataEntityHandle;
		}

		private DataEntityHandle GetCacheHandle()
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("CachedSceneData");
			if (dataEntityHandle.IsNull)
			{
				dataEntityHandle = dataEntityCollection.AddEntity("CachedSceneData");
			}
			return dataEntityHandle;
		}

		public SceneLayoutData AddNewActiveLayout()
		{
			DataEntityHandle activeHandle = GetActiveHandle();
			if (dataEntityCollection.HasComponent<SceneLayoutData>(activeHandle))
			{
				return null;
			}
			return dataEntityCollection.AddComponent<SceneLayoutData>(activeHandle);
		}

		public bool AddNewSceneLayoutData(SceneLayoutData sceneLayoutData)
		{
			DataEntityHandle activeHandle = GetActiveHandle();
			if (dataEntityCollection.HasComponent<SceneLayoutData>(activeHandle))
			{
				Log.LogError(this, "Cannot add SceneLayoutData as one already exists. Remove the exiting one first");
				return false;
			}
			dataEntityCollection.AddComponent(activeHandle, sceneLayoutData);
			return true;
		}

		public bool UpdateActiveLayoutFromData(long layoutId, SceneLayout sceneLayout)
		{
			try
			{
				SceneLayoutData activeSceneLayoutData = GetActiveSceneLayoutData();
				UpdateSceneLayoutData(layoutId, sceneLayout, activeSceneLayoutData);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public bool RemoveActiveSceneLayout()
		{
			DataEntityHandle activeHandle = GetActiveHandle();
			return dataEntityCollection.RemoveComponent<SceneLayoutData>(activeHandle);
		}

		public bool RemoveCachedSceneLayout()
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("CachedSceneData");
			if (dataEntityHandle.IsNull)
			{
				return true;
			}
			return dataEntityCollection.RemoveComponent<SceneLayoutData>(dataEntityHandle);
		}

		public bool HasCachedLayoutData()
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("CachedSceneData");
			bool result = false;
			if (!dataEntityHandle.IsNull)
			{
				result = dataEntityCollection.HasComponent<SceneLayoutData>(dataEntityHandle);
			}
			return result;
		}

		public bool IsLayoutActive(long layoutId)
		{
			SceneLayoutData activeSceneLayoutData = GetActiveSceneLayoutData();
			return activeSceneLayoutData != null && activeSceneLayoutData.LayoutId == layoutId;
		}

		public SceneLayoutData GetActiveSceneLayoutData()
		{
			DataEntityHandle activeHandle = GetActiveHandle();
			SceneLayoutData component = null;
			if (!dataEntityCollection.TryGetComponent(activeHandle, out component))
			{
			}
			return component;
		}

		public SceneLayoutData GetCachedSceneLayoutData()
		{
			DataEntityHandle cacheHandle = GetCacheHandle();
			SceneLayoutData component = null;
			if (!dataEntityCollection.TryGetComponent(cacheHandle, out component))
			{
			}
			return component;
		}

		public SceneLayoutData ResetActiveSceneLayout()
		{
			DataEntityHandle activeHandle = GetActiveHandle();
			RemoveActiveSceneLayout();
			return dataEntityCollection.AddComponent<SceneLayoutData>(activeHandle);
		}

		public bool CacheActiveLayout()
		{
			DataEntityHandle activeHandle = GetActiveHandle();
			DataEntityHandle cacheHandle = GetCacheHandle();
			if (!dataEntityCollection.HasComponent<SceneLayoutData>(activeHandle))
			{
				return false;
			}
			if (dataEntityCollection.HasComponent<SceneLayoutData>(cacheHandle))
			{
				dataEntityCollection.RemoveComponent<SceneLayoutData>(cacheHandle);
			}
			SceneLayoutData component;
			if (dataEntityCollection.TryGetComponent(activeHandle, out component))
			{
				dataEntityCollection.AddComponent(cacheHandle, component);
				return true;
			}
			return false;
		}

		public SceneLayoutData CacheLayoutFromSceneLayout(long layoutId, SceneLayout sceneLayout)
		{
			return addLayoutToEntity(GetCacheHandle(), layoutId, sceneLayout);
		}

		public void CacheLayoutFromSceneLayoutData(SceneLayoutData sceneLayoutData)
		{
			DataEntityHandle cacheHandle = GetCacheHandle();
			if (dataEntityCollection.HasComponent<SceneLayoutData>(cacheHandle))
			{
				Log.LogErrorFormatted(this, "Cannot add SceneLayoutData to {0} as one already exists. Remove the exiting one first.", cacheHandle);
			}
			else
			{
				dataEntityCollection.AddComponent(cacheHandle, sceneLayoutData);
			}
		}

		public SceneLayoutData RestoreCachedLayout()
		{
			DataEntityHandle activeHandle = GetActiveHandle();
			DataEntityHandle cacheHandle = GetCacheHandle();
			SceneLayoutData component = null;
			RemoveActiveSceneLayout();
			if (dataEntityCollection.TryGetComponent(cacheHandle, out component))
			{
				dataEntityCollection.RemoveComponent<SceneLayoutData>(cacheHandle);
				dataEntityCollection.AddComponent(activeHandle, component);
			}
			return component;
		}

		public bool TryRestoreCachedLayout(out SceneLayoutData sceneLayoutData)
		{
			sceneLayoutData = RestoreCachedLayout();
			if (sceneLayoutData != null)
			{
				return true;
			}
			return false;
		}

		private SceneLayoutData addLayoutToEntity(DataEntityHandle handle, long layoutId, SceneLayout sceneLayout)
		{
			if (dataEntityCollection.HasComponent<SceneLayoutData>(handle))
			{
				Log.LogErrorFormatted(this, "Cannot add SceneLayoutData to {0} as one already exists. Remove the exiting one first.", handle);
				return null;
			}
			SceneLayoutData sceneLayoutData = new SceneLayoutData();
			try
			{
				UpdateSceneLayoutData(layoutId, sceneLayout, sceneLayoutData);
				dataEntityCollection.AddComponent(handle, sceneLayoutData);
			}
			catch (Exception ex)
			{
				Log.LogErrorFormatted(this, "Problem creating scene layout from cached data. Message: {0}", ex.Message);
			}
			return sceneLayoutData;
		}

		public bool IsInOwnIgloo()
		{
			bool result = false;
			DataEntityHandle activeHandle = GetActiveHandle();
			SceneOwnerData component;
			if (dataEntityCollection.TryGetComponent(activeHandle, out component))
			{
				result = component.IsOwner;
			}
			return result;
		}

		public void UpdateSceneLayoutData(long layoutId, SceneLayout sceneLayout, SceneLayoutData sceneLayoutData)
		{
			long createdDate = sceneLayout.createdDate.HasValue ? sceneLayout.createdDate.Value : 0;
			long lastModifiedDate = sceneLayout.lastModifiedDate.HasValue ? sceneLayout.lastModifiedDate.Value : 0;
			List<DecorationLayoutData> layout = ToDecorationLayoutData(sceneLayout.decorationsLayout);
			sceneLayoutData.UpdateData(layoutId, sceneLayout.zoneId, createdDate, lastModifiedDate, sceneLayout.memberOnly, sceneLayout.lightingId, sceneLayout.musicId, sceneLayout.extraInfo, layout);
			sceneLayoutData.IsDirty = false;
		}

		public static List<DecorationLayoutData> ToDecorationLayoutData(List<DecorationLayout> decorationLayout)
		{
			List<DecorationLayoutData> list = new List<DecorationLayoutData>();
			if (decorationLayout != null)
			{
				for (int i = 0; i < decorationLayout.Count; i++)
				{
					try
					{
						DecorationLayoutData item = default(DecorationLayoutData);
						item.Id.Name = decorationLayout[i].id.name;
						item.Id.ParentPath = decorationLayout[i].id.parent;
						item.Position = decorationLayout[i].position;
						item.DefinitionId = Convert.ToInt32(decorationLayout[i].definitionId);
						item.Rotation = Quaternion.ToUnityQuaternion(decorationLayout[i].rotation);
						item.Type = ToDecorationLayoutDataDefinitionType(decorationLayout[i].type);
						item.UniformScale = decorationLayout[i].scale;
						item.CustomProperties = decorationLayout[i].customProperties;
						list.Add(item);
					}
					catch (Exception)
					{
					}
				}
			}
			return list;
		}

		public static DecorationLayoutData.DefinitionType ToDecorationLayoutDataDefinitionType(DecorationType layoutType)
		{
			switch (layoutType)
			{
			case DecorationType.Decoration:
				return DecorationLayoutData.DefinitionType.Decoration;
			case DecorationType.Structure:
				return DecorationLayoutData.DefinitionType.Structure;
			default:
				Log.LogErrorFormatted(typeof(IglooMediator), "Unknown decoration type {0}", layoutType);
				return DecorationLayoutData.DefinitionType.Decoration;
			}
		}
	}
}
