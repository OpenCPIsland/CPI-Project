using System;
using System.Collections.Generic;
using System.Linq;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Domain.Scene;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.SceneLayoutSync
{
	// Token: 0x02000002 RID: 2
	public class SceneLayoutSyncService
	{
		// Token: 0x06000001 RID: 1 RVA: 0x0000205C File Offset: 0x0000025C
		public SceneLayoutSyncService(float syncPeriodSeconds)
		{
			this.saveIglooLayoutChangesTimer = new Timer(syncPeriodSeconds, true, delegate ()
			{
				this.saveIglooLayoutChanges(null, null);
			});
			CoroutineRunner.StartPersistent(this.saveIglooLayoutChangesTimer.Start(), this, "saveIglooLayoutChangesTimer");
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020C1 File Offset: 0x000002C1
		public void AddExtraLayoutInfoLoader(SceneLayoutSyncService.ExtraLayoutInfoLoader loader)
		{
			this.extraLayoutInfoLoaders.Add(loader);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020D1 File Offset: 0x000002D1
		public void RemoveExtraLayoutInfoLoader(SceneLayoutSyncService.ExtraLayoutInfoLoader loader)
		{
			this.extraLayoutInfoLoaders.Remove(loader);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020E4 File Offset: 0x000002E4
		public void StartSyncingSceneLayoutData(SceneLayoutData sceneLayoutData)
		{
			if (!this.sceneLayoutDataModified.ContainsKey(sceneLayoutData))
			{
				this.sceneLayoutDataModified[sceneLayoutData] = false;
				sceneLayoutData.SceneLayoutDataUpdated += this.onSceneLayoutDataUpdated;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000212C File Offset: 0x0000032C
		public void StopSyncingSceneLayoutData(SceneLayoutData sceneLayoutData, System.Action onSyncStopped, IIglooUpdateLayoutErrorHandler errorHandler = null)
		{
			if (!this.sceneLayoutDataModified.ContainsKey(sceneLayoutData))
			{
				onSyncStopped.InvokeSafe();
			}
			else
			{
				this.saveIglooLayoutChanges(onSyncStopped, errorHandler);
				sceneLayoutData.SceneLayoutDataUpdated -= this.onSceneLayoutDataUpdated;
				this.sceneLayoutDataModified.Remove(sceneLayoutData);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000217D File Offset: 0x0000037D
		private void onSceneLayoutDataUpdated(SceneLayoutData sceneLayoutData)
		{
			this.sceneLayoutDataModified[sceneLayoutData] = true;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021D8 File Offset: 0x000003D8
		private void saveIglooLayoutChanges(System.Action onSyncStopped = null, IIglooUpdateLayoutErrorHandler errorHandler = null)
		{
			bool flag = false;
			foreach (SceneLayoutData sceneLayoutData in this.sceneLayoutDataModified.Keys.ToList<SceneLayoutData>())
			{
				if (this.sceneLayoutDataModified[sceneLayoutData])
				{
					MutableSceneLayout mutableSceneLayout = new MutableSceneLayout();
					SceneLayoutSyncService.ConvertToMutableSceneLayout(mutableSceneLayout, sceneLayoutData);
					foreach (SceneLayoutSyncService.ExtraLayoutInfoLoader extraLayoutInfoLoader in this.extraLayoutInfoLoaders)
					{
						ExtraLayoutInfo extraLayoutInfo = extraLayoutInfoLoader();
						mutableSceneLayout.extraInfo[extraLayoutInfo.Key] = extraLayoutInfo.Value;
					}
					EventHandlerDelegate<IglooServiceEvents.IglooLayoutUpdated> successHandler = null;
					successHandler = delegate (IglooServiceEvents.IglooLayoutUpdated evt)
					{
						Service.Get<EventDispatcher>().RemoveListener<IglooServiceEvents.IglooLayoutUpdated>(successHandler);
						onSyncStopped.InvokeSafe();
						return false;
					};
					Service.Get<EventDispatcher>().AddListener<IglooServiceEvents.IglooLayoutUpdated>(successHandler, EventDispatcher.Priority.DEFAULT);
					SceneLayoutSyncService.IIglooUpdateLayoutErrorHandlerWrapper errorHandler2 = new SceneLayoutSyncService.IIglooUpdateLayoutErrorHandlerWrapper(successHandler, errorHandler);
					Service.Get<INetworkServicesManager>().IglooService.UpdateIglooLayout(sceneLayoutData.LayoutId, mutableSceneLayout, errorHandler2);
					this.sceneLayoutDataModified[sceneLayoutData] = false;
					flag = true;
				}
			}
			if (!flag)
			{
				onSyncStopped.InvokeSafe();
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002388 File Offset: 0x00000588
		public static void ConvertToMutableSceneLayout(MutableSceneLayout mutableSceneLayout, SceneLayoutData sceneLayoutData)
		{
			mutableSceneLayout.zoneId = sceneLayoutData.LotZoneName;
			mutableSceneLayout.lightingId = sceneLayoutData.LightingId;
			mutableSceneLayout.musicId = sceneLayoutData.MusicTrackId;
			mutableSceneLayout.extraInfo = sceneLayoutData.ExtraInfo;
			mutableSceneLayout.decorationsLayout = new List<DecorationLayout>();
			foreach (DecorationLayoutData decorationLayoutData in sceneLayoutData.GetLayoutEnumerator())
			{
				DecorationLayout item = default(DecorationLayout);
				DecorationLayoutData.ID id = decorationLayoutData.Id;
				item.id.name = id.Name;
				id = decorationLayoutData.Id;
				item.id.parent = id.ParentPath;
				item.type = ((decorationLayoutData.Type == DecorationLayoutData.DefinitionType.Decoration) ? DecorationType.Decoration : DecorationType.Structure);
				item.definitionId = (long)decorationLayoutData.DefinitionId;
				item.position = decorationLayoutData.Position;
				item.rotation = Quaternion.FromUnityQuaternion(decorationLayoutData.Rotation);
				item.scale = decorationLayoutData.UniformScale;
				item.customProperties = decorationLayoutData.CustomProperties;
				mutableSceneLayout.decorationsLayout.Add(item);
			}
		}

		// Token: 0x04000001 RID: 1
		private List<SceneLayoutSyncService.ExtraLayoutInfoLoader> extraLayoutInfoLoaders = new List<SceneLayoutSyncService.ExtraLayoutInfoLoader>();

		// Token: 0x04000002 RID: 2
		private Timer saveIglooLayoutChangesTimer;

		// Token: 0x04000003 RID: 3
		private Dictionary<SceneLayoutData, bool> sceneLayoutDataModified = new Dictionary<SceneLayoutData, bool>();

		// Token: 0x02000003 RID: 3
		// (Invoke) Token: 0x0600000B RID: 11
		public delegate ExtraLayoutInfo ExtraLayoutInfoLoader();

		// Token: 0x02000004 RID: 4
		private class IIglooUpdateLayoutErrorHandlerWrapper : IIglooUpdateLayoutErrorHandler
		{
			// Token: 0x0600000E RID: 14 RVA: 0x000024CC File Offset: 0x000006CC
			public IIglooUpdateLayoutErrorHandlerWrapper(EventHandlerDelegate<IglooServiceEvents.IglooLayoutUpdated> successHandler, IIglooUpdateLayoutErrorHandler errorHandler)
			{
				this.successHandler = successHandler;
				this.errorHandler = errorHandler;
			}

			// Token: 0x0600000F RID: 15 RVA: 0x000024E5 File Offset: 0x000006E5
			public void OnUpdateLayoutError()
			{
				Service.Get<EventDispatcher>().RemoveListener<IglooServiceEvents.IglooLayoutUpdated>(this.successHandler);
				this.errorHandler.OnUpdateLayoutError();
			}

			// Token: 0x04000004 RID: 4
			private IIglooUpdateLayoutErrorHandler errorHandler;

			// Token: 0x04000005 RID: 5
			private EventHandlerDelegate<IglooServiceEvents.IglooLayoutUpdated> successHandler;
		}
	}
}
