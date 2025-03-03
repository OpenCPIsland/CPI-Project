using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IAssetLoader
	{
		void Load(string url, Action<LoadAssetResult> callback);
	}
}
