using System;
using System.Text;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class AssetContentKey
	{
		public string Key;

		public AssetContentKey()
		{
		}

		public AssetContentKey(string key)
		{
			Key = key.ToLower();
		}

		public AssetContentKey(AssetContentKey key, params string[] args)
		{
			Key = key.Key.ToLower();
			Key = Expand(args);
		}

		public string Expand(params string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 0; i < Key.Length; i++)
			{
				if (Key[i] == '*')
				{
					num++;
				}
			}
			if (num != args.Length)
			{
				throw new ArgumentException(string.Format("Number of arguments passed to Expand ({0}) is not equal to number of wildcards ({1}) in key '{2}'", args.Length, num, Key));
			}
			int num2 = 0;
			for (int i = 0; i < Key.Length; i++)
			{
				if (Key[i] == '*')
				{
					stringBuilder.Append(args[num2++]);
				}
				else
				{
					stringBuilder.Append(Key[i]);
				}
			}
			return stringBuilder.ToString().ToLower();
		}

		public override string ToString()
		{
			return string.Format("[AssetContentKey: {0}]", Key);
		}

		public static string GetAssetName(AssetContentKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Cannot get asset name for a null AssetContentKey");
			}
			if (string.IsNullOrEmpty(key.Key))
			{
				throw new ArgumentException("AssetContentKey key string was null", "key");
			}
			return key.Key.Substring(key.Key.LastIndexOf("/") + 1);
		}
	}
}
