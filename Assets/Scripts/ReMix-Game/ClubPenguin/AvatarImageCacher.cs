using ClubPenguin.Core;
using ClubPenguin.Kelowna.Common.ImageCache;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class AvatarImageCacher
	{
		private ImageCache imageCache;

		private DataEntityCollection dataEntityCollection;

		public AvatarImageCacher()
		{
			imageCache = Service.Get<ImageCache>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		public bool TryGetCachedImage(DataEntityHandle handle, out Texture2D icon, string context)
		{
			icon = null;
			string hashName = createHash(handle, context);
			if (imageCache.ContainsImage(hashName))
			{
				icon = imageCache.GetTextureFromCache(hashName);
				return true;
			}
			return false;
		}

		public bool ContainsImage(DataEntityHandle handle, string context)
		{
			string hashName = createHash(handle, context);
			return imageCache.ContainsImage(hashName);
		}

		private string createHash(DataEntityHandle handle, string context)
		{
			string text = null;
			AvatarDetailsData component;
			if (dataEntityCollection.TryGetComponent(handle, out component) && component.Outfit != null)
			{
				string displayName = dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName;
				AvatarDetailsHashable data = new AvatarDetailsHashable(component, displayName, context);
				return MD5HashUtil.GetHash(data);
			}
			throw new ArgumentException("Data entity handle did not have valid avatar details");
		}

		public void SaveTextureToCache(DataEntityHandle handle, string context, Texture2D icon)
		{
			string hashName = createHash(handle, context);
			if (icon == null)
			{
				throw new ArgumentNullException("icon");
			}
			imageCache.SaveTextureToCache(hashName, icon);
		}

		public void RemoveOutfitFromCache(DataEntityHandle handle, string context = null)
		{
			string hashName = createHash(handle, context);
			if (imageCache.ContainsImage(hashName))
			{
				imageCache.RemoveImage(hashName);
			}
		}

		public void ClearCache()
		{
			imageCache.ClearSessionCachedImages();
		}
	}
}
