using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[Serializable]
	public class TypedAssetContentKey<T> : AssetContentKey, ISerializationCallbackReceiver where T : class
	{
		[SerializeField]
		private string type;

		public TypedAssetContentKey()
		{
		}

		public TypedAssetContentKey(string key)
			: base(key)
		{
		}

		public TypedAssetContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}

		public void OnBeforeSerialize()
		{
			if (string.IsNullOrEmpty(type))
			{
				type = typeof(T).FullName + ", " + typeof(T).Assembly.GetName().Name;
			}
		}

		public void OnAfterDeserialize()
		{
		}

		public override string ToString()
		{
			return string.Format("[TypedAssetContentKey: {0} ({1}]", Key, type);
		}
	}
}
