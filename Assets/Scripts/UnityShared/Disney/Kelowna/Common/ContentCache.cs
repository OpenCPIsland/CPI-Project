using Disney.LaunchPadFramework;
using Foundation.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class ContentCache<T> where T : UnityEngine.Object
	{
		private class Item
		{
			public T Value;

			public int RefCount = 0;

			public override string ToString()
			{
				return string.Format("[Item] Value='{0}', Refcount={1}", Value, RefCount);
			}
		}

		private class SubItem
		{
			public UnityEngine.Object Value;

			public int RefCount = 0;

			public override string ToString()
			{
				return string.Format("[SubItem] Value='{0}', Refcount={1}", Value, RefCount);
			}
		}

		private readonly Dictionary<string, Item> cache = new Dictionary<string, Item>();

		private readonly Dictionary<int, SubItem> subCache = new Dictionary<int, SubItem>();

		private readonly Action<T, List<UnityEngine.Object>> releaseDelegate;

		public ContentCache()
		{
			releaseDelegate = DefaultReleaseDelegate;
		}

		public ContentCache(Action<T, List<UnityEngine.Object>> releaseDelegate)
		{
			this.releaseDelegate = releaseDelegate;
		}

		public static void DefaultReleaseDelegate(T value, List<UnityEngine.Object> subObjects)
		{
			ComponentExtensions.DestroyResource(value);
			foreach (UnityEngine.Object subObject in subObjects)
			{
				ComponentExtensions.DestroyResource(subObject);
			}
		}

		public IEnumerator Acquire(TypedAssetContentKey<T> content, Action<T> onAcquired)
		{
			if (Content.ContainsKey(content.Key))
			{
				Item item;
				if (cache.TryGetValue(content.Key, out item))
				{
					if ((UnityEngine.Object)item.Value == (UnityEngine.Object)null)
					{
					}
					item.RefCount++;
					while ((UnityEngine.Object)item.Value == (UnityEngine.Object)null && item.RefCount > 0)
					{
						yield return null;
					}
				}
				else
				{
					item = new Item();
					cache.Add(content.Key, item);
					item.RefCount = 1;
					AssetRequest<T> assetRequest = Content.LoadAsync(content);
					while (!assetRequest.Finished)
					{
						yield return null;
					}
					if (item.RefCount <= 0)
					{
						releaseDelegate.InvokeSafe(assetRequest.Asset, new List<UnityEngine.Object>());
						onAcquired(null);
						yield break;
					}
					item.Value = assetRequest.Asset;
					if (object.ReferenceEquals(item.Value, null))
					{
						Log.LogErrorFormatted(this, "Acquiring asset '{0}' from Content system returned a null!", content);
					}
					else if ((UnityEngine.Object)item.Value == (UnityEngine.Object)null)
					{
						Log.LogErrorFormatted(this, "Acquiring asset '{0}' from Content system returned a destroyed object!", content);
					}
				}
				onAcquired(item.Value);
				if (item.Value is ICacheableContent && (UnityEngine.Object)item.Value != (UnityEngine.Object)null)
				{
					addSubCacheReferences(item.Value as ICacheableContent);
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Asset with key '{0}' does not exist in this content version!", content.Key);
				onAcquired(null);
			}
		}

		private void addSubCacheReferences(ICacheableContent cacheableContent)
		{
			List<UnityEngine.Object> list = cacheableContent.InternalReferences();
			if (list != null)
			{
				foreach (UnityEngine.Object item in list)
				{
					if (!(item == null))
					{
						int instanceID = item.GetInstanceID();
						if (!subCache.ContainsKey(instanceID))
						{
							SubItem subItem = new SubItem();
							subItem.Value = item;
							subCache.Add(instanceID, subItem);
						}
						subCache[instanceID].RefCount++;
					}
				}
			}
		}

		public bool Release(TypedAssetContentKey<T> content)
		{
			bool result = false;
			Item value;
			if (cache.TryGetValue(content.Key, out value))
			{
				if (value.RefCount <= 0)
				{
					Log.LogErrorFormatted(this, "Releasing a key '{0}' with refcount <= 0!", content.Key);
				}
				value.RefCount--;
				List<UnityEngine.Object> arg = new List<UnityEngine.Object>();
				if (value.Value is ICacheableContent && (UnityEngine.Object)value.Value != (UnityEngine.Object)null)
				{
					arg = releaseSubCacheReferences(value.Value as ICacheableContent);
				}
				if (value.RefCount <= 0)
				{
					if (!object.ReferenceEquals(value.Value, null) && (UnityEngine.Object)value.Value == (UnityEngine.Object)null)
					{
						Log.LogErrorFormatted(this, "Releasing asset '{0}': Value is a destroyed object!", content);
					}
					releaseDelegate.InvokeSafe(value.Value, arg);
					value.Value = null;
					cache.Remove(content.Key);
					result = true;
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Could not release key '{0}' because it was not in the cache!", content.Key);
			}
			return result;
		}

		private List<UnityEngine.Object> releaseSubCacheReferences(ICacheableContent cacheableContent)
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			List<UnityEngine.Object> list2 = cacheableContent.InternalReferences();
			if (list2 != null)
			{
				foreach (UnityEngine.Object item in list2)
				{
					if (!(item == null))
					{
						int instanceID = item.GetInstanceID();
						SubItem value;
						if (subCache.TryGetValue(instanceID, out value))
						{
							value.RefCount--;
							if (value.RefCount <= 0)
							{
								subCache.Remove(instanceID);
								list.Add(value.Value);
							}
						}
						else
						{
							Log.LogErrorFormatted(item, "Instance Id '{0}' not found in the sub cache!", instanceID);
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		public void DumpLog()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("===============================================");
			stringBuilder.AppendLine("Cache:");
			foreach (KeyValuePair<string, Item> item in cache)
			{
				stringBuilder.AppendFormat("{0}: {1}\n", item.Key, item.Value.RefCount);
			}
			stringBuilder.AppendLine("Sub Cache:");
			foreach (KeyValuePair<int, SubItem> item2 in subCache)
			{
				stringBuilder.AppendFormat("{0}: {1}\n", item2.Value.Value, item2.Value.RefCount);
			}
		}
	}
}
