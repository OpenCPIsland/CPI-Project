using ClubPenguin.DecorationInventory;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class StructureImageBuilder : AbstractImageBuilder
	{
		private class StructureRenderParams : RenderParams
		{
			public StructureDefinition Definition;
		}

		private static StructureImageBuilder instance;

		private static int refCount = 0;

		private Dictionary<int, StructureDefinition> structureList;

		public static StructureImageBuilder Acquire()
		{
			if (instance == null)
			{
				instance = new StructureImageBuilder();
			}
			refCount++;
			return instance;
		}

		public static void Release()
		{
			if (--refCount <= 0)
			{
				instance.destroy();
				instance = null;
			}
		}

		private StructureImageBuilder()
		{
			structureList = Service.Get<GameData>().Get<Dictionary<int, StructureDefinition>>();
		}

		public void RequestImage(long id, StructureDefinition definition, RequestImageCallback callback, Color backgroundColor)
		{
			string hashedName = getHashedName(id);
			CallbackToken callbackToken = default(CallbackToken);
			callbackToken.Id = id;
			callbackToken.DefinitionId = definition.Id;
			StructureRenderParams structureRenderParams = new StructureRenderParams();
			structureRenderParams.CallbackToken = callbackToken;
			structureRenderParams.Definition = definition;
			structureRenderParams.BackgroundColor = backgroundColor;
			structureRenderParams.ImageHash = hashedName;
			StructureRenderParams renderParams = structureRenderParams;
			RequestImage(renderParams, callback);
		}

		protected override IEnumerator processRequest(RenderParams renderParam)
		{
			StructureRenderParams param = renderParam as StructureRenderParams;
			StructureDefinition structureDefinition = null;
			if (!structureList.TryGetValue(param.Definition.Id, out structureDefinition))
			{
				Log.LogErrorFormatted(this, "Unable to locate structure {0} in structure definitions with id {1}.", param.Definition.Name, param.Definition.Id);
			}
			else
			{
				yield return loadStructurePrefab(param);
			}
		}

		private string getHashedName(long id)
		{
			return string.Format("Structure_{0}", id);
		}

		private IEnumerator loadStructurePrefab(StructureRenderParams param)
		{
			AssetRequest<GameObject> structureRequest;
			try
			{
				structureRequest = Content.LoadAsync(param.Definition.Prefab);
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "Could not load structure prefab {0} with prefab key {1}", param.Definition.Name, param.Definition.Prefab.Key);
				yield break;
			}
			if (structureRequest == null)
			{
				Log.LogErrorFormatted(this, "Something went wrong loading structure {0}.", param.Definition.Prefab);
				yield break;
			}
			yield return structureRequest;
			GameObject structureGameObject = UnityEngine.Object.Instantiate(structureRequest.Asset);
			LightCullingMaskHelper.SetLayerIncludingChildren(structureGameObject.transform, "IconRender");
			yield return loadStructureRenderData(param, structureGameObject);
		}

		private IEnumerator loadStructureRenderData(StructureRenderParams param, GameObject structureGameObject)
		{
			AssetRequest<StructureRenderData> renderDataRequest = null;
			try
			{
				renderDataRequest = Content.LoadAsync(param.Definition.RenderData);
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "Could not load the render data for structure {0} with key {1}", param.Definition.Name, param.Definition.RenderData.Key);
			}
			if (renderDataRequest == null)
			{
				Log.LogErrorFormatted(this, "Something went wrong loading render data {0}.", param.Definition.RenderData);
				UnityEngine.Object.Destroy(structureGameObject);
			}
			else
			{
				yield return renderDataRequest;
				StructureRenderData structureRenderData = renderDataRequest.Asset;
				yield return renderToTexture(param, structureGameObject, structureRenderData);
			}
		}

		private IEnumerator renderToTexture(RenderParams param, GameObject decorationGameObject, StructureRenderData decorationRenderData)
		{
			ModelRendererConfig config = new ModelRendererConfig(decorationGameObject.transform, decorationRenderData.ItemPosition, new Vector2(256f, 256f))
			{
				FieldOfView = decorationRenderData.CameraFOV,
				FrameObjectInCamera = false,
				UseOcclusionCulling = false,
				AutoDestroyObjectToRender = true
			};
			if (param.BackgroundColor != Color.clear)
			{
				config.CameraBackgroundColor = param.BackgroundColor;
				config.UseSolidBackground = true;
			}
			ModelRenderer modelRenderer = new ModelRenderer(config);
			modelRenderer.RotateCamera(decorationRenderData.CameraRotation.eulerAngles);
			yield return new WaitForEndOfFrame();
			param.OutputTexture = modelRenderer.Image;
			modelRenderer.Destroy();
		}
	}
}
