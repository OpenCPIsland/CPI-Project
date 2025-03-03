using Disney.Kelowna.Common;
using Foundation.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ClubPenguin.DCE
{
	public class DceLoadingService
	{
		public class Request<T> where T : UnityEngine.Object
		{
			public readonly List<TypedAssetContentKey<T>> Loaded = new List<TypedAssetContentKey<T>>();

			public readonly ContentCache<T> Cache;

			internal int numLoaded;

			public bool Finished;

			public Request(ContentCache<T> cache)
			{
				Cache = cache;
			}

			[Conditional("FALSE")]
			public void Validate()
			{
				int num = 0;
				foreach (TypedAssetContentKey<T> item in Loaded)
				{
					if (item != null)
					{
						num++;
					}
				}
				if (num < Loaded.Count)
				{
					UnityEngine.Debug.LogWarningFormat("Request: {0} of {1} items non-null", num, Loaded.Count);
				}
			}
		}

		public readonly ContentCache<Texture2D> DecalCache = new ContentCache<Texture2D>();

		private readonly LinkedList<IEnumerator> loaders = new LinkedList<IEnumerator>();

		public DceLoadingService(FibreService fibreService)
		{
			fibreService.AddFibre("OutfitService:New", 0.5f, fibre);
		}

		public IEnumerator<bool> fibre()
		{
			while (true)
			{
				LinkedListNode<IEnumerator> node = loaders.First;
				while (node != null)
				{
					IEnumerator loader = node.Value;
					if (loader.MoveNext())
					{
						node = node.Next;
						yield return true;
					}
					else
					{
						LinkedListNode<IEnumerator> node2 = node;
						node = node.Next;
						loaders.Remove(node2);
					}
				}
				yield return false;
			}
		}

		private void loadLast(IEnumerator loader)
		{
			if (loader.MoveNext())
			{
				loaders.AddLast(loader);
			}
		}

		private void loadFirst(IEnumerator loader)
		{
			if (loader.MoveNext())
			{
				loaders.AddFirst(loader);
			}
		}

		public Request<T> Load<T>(IEnumerable<KeyValuePair<TypedAssetContentKey<T>, Action<T>>> content, ContentCache<T> cache, Action<Request<T>> onFinished = null) where T : UnityEngine.Object
		{
			Request<T> request = new Request<T>(cache);
			foreach (KeyValuePair<TypedAssetContentKey<T>, Action<T>> item in content)
			{
				TypedAssetContentKey<T> key = item.Key;
				Action<T> handler = item.Value;
				request.Loaded.Add(key);
				loadFirst(cache.Acquire(key, delegate(T x)
				{
					request.numLoaded++;
					handler(x);
				}));
			}
			loadLast(waitRequest(request, onFinished));
			return request;
		}

		public void Unload<T>(Request<T> request) where T : UnityEngine.Object
		{
			for (int i = 0; i < request.Loaded.Count; i++)
			{
				TypedAssetContentKey<T> typedAssetContentKey = request.Loaded[i];
				if (typedAssetContentKey != null)
				{
					request.Cache.Release(typedAssetContentKey);
					request.Loaded[i] = null;
				}
			}
			request.Finished = false;
		}

		private static IEnumerator waitRequest<T>(Request<T> request, Action<Request<T>> onFinished = null) where T : UnityEngine.Object
		{
			while (request.numLoaded < request.Loaded.Count)
			{
				yield return null;
			}
			request.Finished = true;
			if (onFinished != null)
			{
				onFinished(request);
			}
		}
	}
}
