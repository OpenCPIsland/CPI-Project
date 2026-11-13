using System;
using System.Collections;
using System.Collections.Generic;

namespace SwrveUnity
{
	public interface ISwrveAssetsManager
	{
		string CdnImages
		{
			get;
			set;
		}

		string CdnFonts
		{
			get;
			set;
		}

		HashSet<string> AssetsOnDisk
		{
			get;
			set;
		}

		IEnumerator DownloadAssets(HashSet<SwrveAssetsQueueItem> assetsQueue, Action callBack);
	}
}
