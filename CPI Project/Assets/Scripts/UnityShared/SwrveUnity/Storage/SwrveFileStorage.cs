using SwrveUnity.Helpers;
using System;

namespace SwrveUnity.Storage
{
	public class SwrveFileStorage : ISwrveStorage
	{
		public const string SIGNATURE_SUFFIX = "_SGT";

		protected string swrvePath;

		protected string uniqueKey;

		protected Action callback;

		public SwrveFileStorage(string swrvePath, string uniqueKey)
		{
			this.swrvePath = swrvePath;
			this.uniqueKey = uniqueKey;
		}

		public virtual void Save(string tag, string data, string userId = null)
		{
			if (!string.IsNullOrEmpty(data))
			{
				bool flag = false;
				try
				{
					string fileName = GetFileName(tag, userId);
					SwrveLog.Log("Saving: " + fileName, "storage");
					CrossPlatformFile.SaveText(fileName, data);
					flag = true;
				}
				catch (Exception ex)
				{
					SwrveLog.LogError(ex.ToString(), "storage");
				}
				if (!flag)
				{
					SwrveLog.LogWarning(tag + " not saved!", "storage");
				}
			}
		}

		public virtual void SaveSecure(string tag, string data, string userId = null)
		{
			if (!string.IsNullOrEmpty(data))
			{
				bool flag = false;
				try
				{
					string fileName = GetFileName(tag, userId);
					SwrveLog.Log("Saving: " + fileName, "storage");
					CrossPlatformFile.SaveText(fileName, data);
					string path = fileName + "_SGT";
					string data2 = SwrveHelper.CreateHMACMD5(data, uniqueKey);
					CrossPlatformFile.SaveText(path, data2);
					flag = true;
				}
				catch (Exception ex)
				{
					SwrveLog.LogError(ex.ToString(), "storage");
				}
				if (!flag)
				{
					SwrveLog.LogWarning(tag + " not saved!", "storage");
				}
			}
		}

		public virtual string Load(string tag, string userId = null)
		{
			string result = null;
			try
			{
				string fileName = GetFileName(tag, userId);
				if (CrossPlatformFile.Exists(fileName))
				{
					result = CrossPlatformFile.LoadText(fileName);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError(ex.ToString(), "storage");
			}
			return result;
		}

		public virtual void Remove(string tag, string userId = null)
		{
			try
			{
				string fileName = GetFileName(tag, userId);
				if (CrossPlatformFile.Exists(fileName))
				{
					SwrveLog.Log("Removing: " + fileName, "storage");
					CrossPlatformFile.Delete(fileName);
				}
				string path = fileName + "_SGT";
				if (CrossPlatformFile.Exists(path))
				{
					CrossPlatformFile.Delete(path);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError(ex.ToString(), "storage");
			}
		}

		public void SetSecureFailedListener(Action callback)
		{
			this.callback = callback;
		}

		public virtual string LoadSecure(string tag, string userId = null)
		{
			string text = null;
			string fileName = GetFileName(tag, userId);
			try
			{
				string text2 = fileName + "_SGT";
				if (CrossPlatformFile.Exists(fileName))
				{
					text = CrossPlatformFile.LoadText(fileName);
					if (!string.IsNullOrEmpty(text))
					{
						string text3 = null;
						if (CrossPlatformFile.Exists(text2))
						{
							text3 = CrossPlatformFile.LoadText(text2);
						}
						else
						{
							SwrveLog.LogError("Could not read signature file: " + text2);
							text = null;
						}
						if (!string.IsNullOrEmpty(text3))
						{
							string value = SwrveHelper.CreateHMACMD5(text, uniqueKey);
							if (string.IsNullOrEmpty(value))
							{
								SwrveLog.LogError("Could not compute signature for data in file " + fileName);
								text = null;
							}
							else if (!text3.Equals(value))
							{
								if (callback != null)
								{
									callback();
								}
								SwrveLog.LogError("Signature validation failed for " + fileName);
								text = null;
							}
						}
					}
					else
					{
						SwrveLog.LogError("Could not read file " + fileName);
					}
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError(ex.ToString(), "storage");
			}
			return text;
		}

		private string GetFileName(string tag, string userId)
		{
			string str = (swrvePath.Length > 0) ? "/" : string.Empty;
			return swrvePath + str + tag + ((userId == null) ? string.Empty : userId);
		}
	}
}
