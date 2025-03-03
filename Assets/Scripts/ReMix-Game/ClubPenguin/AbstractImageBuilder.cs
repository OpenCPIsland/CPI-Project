using ClubPenguin.Kelowna.Common.ImageCache;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public abstract class AbstractImageBuilder
	{
		public struct CallbackToken
		{
			public long Id;

			public int DefinitionId;
		}

		public delegate void RequestImageCallback(bool success, Texture2D texture, CallbackToken callbackToken);

		protected class RenderParams
		{
			public CallbackToken CallbackToken;

			public Color BackgroundColor;

			public Texture2D OutputTexture;

			public string ImageHash;
		}

		protected const int ICON_SIZE = 256;

		protected IconRenderLightingRig lightingRig;

		protected ImageCache imageCache;

		private Queue<RenderParams> renderQueue;

		private Dictionary<CallbackToken, List<RequestImageCallback>> completionCallbacks;

		protected AbstractImageBuilder()
		{
			renderQueue = new Queue<RenderParams>();
			completionCallbacks = new Dictionary<CallbackToken, List<RequestImageCallback>>();
			imageCache = Service.Get<ImageCache>();
			if (imageCache == null)
			{
				Log.LogError(this, "ImageCache service not started!");
			}
		}

		protected bool RequestImage(RenderParams renderParams, RequestImageCallback callback)
		{
			CallbackToken callbackToken = renderParams.CallbackToken;
			if (completionCallbacks.ContainsKey(callbackToken))
			{
				completionCallbacks[callbackToken].Add(callback);
				return false;
			}
			if (imageCache.ContainsImage(renderParams.ImageHash))
			{
				Texture2D textureFromCache = imageCache.GetTextureFromCache(renderParams.ImageHash);
				callback(true, textureFromCache, callbackToken);
				return true;
			}
			List<RequestImageCallback> list = new List<RequestImageCallback>();
			list.Add(callback);
			completionCallbacks.Add(callbackToken, list);
			renderQueue.Enqueue(renderParams);
			if (renderQueue.Count == 1)
			{
				CoroutineRunner.Start(setupAndProcessRequests(), this, "ProcessNextRequest");
			}
			return false;
		}

		private IEnumerator setupAndProcessRequests()
		{
			yield return acquireLightingRig();
			yield return processPendingRequests();
			IconRenderLightingRig.Release();
			lightingRig = null;
		}

		private IEnumerator acquireLightingRig()
		{
			lightingRig = IconRenderLightingRig.Acquire();
			while (!lightingRig.IsReady)
			{
				yield return null;
			}
		}

		protected virtual IEnumerator processPendingRequests()
		{
			while (renderQueue.Count > 0)
			{
				RenderParams param = renderQueue.Peek();
				param.OutputTexture = null;
				yield return processRequest(param);
				bool success = param.OutputTexture != null;
				if (success)
				{
					imageCache.SaveTextureToCache(param.ImageHash, param.OutputTexture);
				}
				List<RequestImageCallback> callbackList = completionCallbacks[param.CallbackToken];
				completionCallbacks.Remove(param.CallbackToken);
				for (int i = 0; i < callbackList.Count; i++)
				{
					callbackList[i](success, param.OutputTexture, param.CallbackToken);
				}
				renderQueue.Dequeue();
			}
		}

		protected abstract IEnumerator processRequest(RenderParams param);

		protected virtual void destroy()
		{
			foreach (KeyValuePair<CallbackToken, List<RequestImageCallback>> completionCallback in completionCallbacks)
			{
				List<RequestImageCallback> value = completionCallback.Value;
				for (int i = 0; i < value.Count; i++)
				{
					value[i](false, null, completionCallback.Key);
				}
			}
			completionCallbacks.Clear();
			CoroutineRunner.StopAllForOwner(this);
			imageCache.ClearSessionCachedImages();
		}
	}
}
