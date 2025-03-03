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
	public class DecorationImageBuilder : AbstractImageBuilder
	{
		private class DecorationRenderParams : RenderParams
		{
			public DecorationDefinition Definition;
		}

		private const int CLEAR_UNUSED_DECORATION_ASSET_LIMIT = 30;

		private static DecorationImageBuilder instance;

		private static int refCount = 0;

		private static int loadedItemCount = 0;

		private Dictionary<int, DecorationDefinition> decorationList;

		public static DecorationImageBuilder Acquire()
		{
			if (instance == null)
			{
				instance = new DecorationImageBuilder();
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

		private DecorationImageBuilder()
		{
			decorationList = Service.Get<GameData>().Get<Dictionary<int, DecorationDefinition>>();
		}

		public void RequestImage(long id, DecorationDefinition definition, RequestImageCallback callback, Color backgroundColor)
		{
			string hashedName = getHashedName(id);
			CallbackToken callbackToken = default(CallbackToken);
			callbackToken.Id = id;
			callbackToken.DefinitionId = definition.Id;
			DecorationRenderParams decorationRenderParams = new DecorationRenderParams();
			decorationRenderParams.CallbackToken = callbackToken;
			decorationRenderParams.Definition = definition;
			decorationRenderParams.BackgroundColor = backgroundColor;
			decorationRenderParams.ImageHash = hashedName;
			DecorationRenderParams renderParams = decorationRenderParams;
			if (RequestImage(renderParams, callback))
			{
				loadedItemCount++;
				if (loadedItemCount >= 30)
				{
					Resources.UnloadUnusedAssets();
					loadedItemCount = 0;
				}
			}
		}

		protected override IEnumerator processRequest(RenderParams renderParam)
		{
			DecorationRenderParams param = renderParam as DecorationRenderParams;
			DecorationDefinition decorationDefinition = null;
			if (!decorationList.TryGetValue(param.Definition.Id, out decorationDefinition))
			{
				Log.LogErrorFormatted(this, "Unable to locate decoration {0} in decoration definitions with id {1}.", param.Definition.Name, param.Definition.Id);
			}
			else
			{
				yield return loadDecorationPrefab(param);
			}
		}

		private string getHashedName(long id)
		{
			return string.Format("Decoration_{0}", id);
		}

		private IEnumerator loadDecorationPrefab(DecorationRenderParams param)
		{
			AssetRequest<GameObject> decorationRequest;
			try
			{
				decorationRequest = Content.LoadAsync(param.Definition.Prefab);
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "Could not load decoration prefab {0} with prefab key {1}", param.Definition.Name, param.Definition.Prefab.Key);
				yield break;
			}
			if (decorationRequest == null)
			{
				Log.LogErrorFormatted(this, "Something went wrong loading decoration {0}.", param.Definition.Prefab);
				yield break;
			}
			yield return decorationRequest;
			GameObject decorationGameObject = UnityEngine.Object.Instantiate(decorationRequest.Asset);
			LightCullingMaskHelper.SetLayerIncludingChildren(decorationGameObject.transform, "IconRender");
			yield return loadDecorationRenderData(param, decorationGameObject);
		}

		private IEnumerator loadDecorationRenderData(DecorationRenderParams param, GameObject decorationGameObject)
		{
			AssetRequest<DecorationRenderData> renderDataRequest = null;
			try
			{
				renderDataRequest = Content.LoadAsync(param.Definition.RenderData);
			}
			catch (Exception)
			{
				Log.LogErrorFormatted(this, "Could not load the render data for decation {0} with key {1}", param.Definition.Name, param.Definition.RenderData.Key);
			}
			if (renderDataRequest == null)
			{
				Log.LogErrorFormatted(this, "Something went wrong loading render data {0}.", param.Definition.RenderData);
				UnityEngine.Object.Destroy(decorationGameObject);
			}
			else
			{
				yield return renderDataRequest;
				DecorationRenderData decorationRenderData = renderDataRequest.Asset;
				yield return renderToTexture(param, decorationGameObject, decorationRenderData);
			}
		}

		private IEnumerator renderToTexture(RenderParams param, GameObject decorationGameObject, DecorationRenderData decorationRenderData)
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
			decorationGameObject.transform.rotation = decorationRenderData.ItemRotation;
			ModelRenderer modelRenderer = new ModelRenderer(config);
			modelRenderer.RotateCamera(decorationRenderData.CameraRotation.eulerAngles);
			yield return new WaitForEndOfFrame();
			param.OutputTexture = modelRenderer.Image;
			modelRenderer.Destroy();
		}
	}
}
