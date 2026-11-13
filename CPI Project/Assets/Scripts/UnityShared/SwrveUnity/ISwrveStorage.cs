using System;

namespace SwrveUnity
{
	public interface ISwrveStorage
	{
		void Save(string tag, string data, string userId = null);

		string Load(string tag, string userId = null);

		void Remove(string tag, string userId = null);

		void SetSecureFailedListener(Action callback);

		void SaveSecure(string tag, string data, string userId = null);

		string LoadSecure(string tag, string userId = null);
	}
}
