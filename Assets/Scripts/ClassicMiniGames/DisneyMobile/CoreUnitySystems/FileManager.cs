using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class FileManager : MonoBehaviour, IConfigurable
	{
		public string mDefaultKeyChainGroup;

		public ArrayList mKeyChainGroups;

		protected Dictionary<string, string> mKeyChainDict = new Dictionary<string, string>();

		protected static FileManager m_instance = null;

		public static FileManager Instance
		{
			get
			{
				return m_instance;
			}
		}

		public void Configure(IDictionary<string, object> dictionary)
		{
			AutoConfigurable.AutoConfigureObject(this, dictionary);
			CreateKeyChainDict();
		}

		public void Reconfigure(IDictionary<string, object> dictionary)
		{
			Configure(dictionary);
		}

		private void Awake()
		{
			m_instance = this;
		}

		protected virtual void _CreateKeyChainDict()
		{
		}

		private void CreateKeyChainDict()
		{
			Instance._CreateKeyChainDict();
		}

		protected virtual string _GetKeyChainAccessGroup(string group)
		{
			return "";
		}

		public static string GetKeyChainAccessGroup(string group)
		{
			return Instance._GetKeyChainAccessGroup(group);
		}

		public static string GetAppDataPathCache()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return Application.persistentDataPath.Replace("Documents", "Library") + Path.DirectorySeparatorChar + "Caches";
			}
			if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Caches"))
			{
				Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Caches");
			}
			return Application.persistentDataPath + Path.DirectorySeparatorChar + "Caches";
		}

		public static string GetFullPath(FileLocation location, string filenamelocalpath)
		{
			string result = "";
			switch (location)
			{
			case FileLocation.PERMANENT:
				result = Application.persistentDataPath + Path.DirectorySeparatorChar + filenamelocalpath;
				break;
			case FileLocation.CACHE:
				result = GetAppDataPathCache() + Path.DirectorySeparatorChar + filenamelocalpath;
				break;
			case FileLocation.SHARED:
				result = Application.persistentDataPath + Path.DirectorySeparatorChar + "Shared" + Path.DirectorySeparatorChar + filenamelocalpath;
				break;
			case FileLocation.ABSOLUTE:
				result = filenamelocalpath;
				break;
			}
			return result;
		}

		protected virtual string _LoadTextFile(FileLocation location, string filenamelocalpath, string accessgroup)
		{
			return LoadTextFile(FileLocation.PERMANENT, filenamelocalpath);
		}

		public static string LoadTextFile(FileLocation location, string filenamelocalpath, string accessgroup)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._LoadTextFile(location, filenamelocalpath, accessgroup);
			}
			return null;
		}

		public static string LoadTextFile(FileLocation location, string filenamelocalpath)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._LoadTextFile(location, filenamelocalpath, null);
			}
			string fullPath = GetFullPath(location, filenamelocalpath);
			if (!File.Exists(fullPath))
			{
				Logger.LogWarning(m_instance, "Load text file dosn't exist " + fullPath);
				return null;
			}
			try
			{
				return File.ReadAllText(fullPath);
			}
			catch (Exception arg)
			{
				Logger.LogFatal(m_instance, "Load text file Exception " + arg);
			}
			return null;
		}

		protected virtual bool _SaveTextFile(FileLocation location, string filenamelocalpath, string textdata, string accessgroup)
		{
			return SaveTextFile(FileLocation.PERMANENT, filenamelocalpath, textdata);
		}

		public static bool SaveTextFile(FileLocation location, string filenamelocalpath, string textdata, string accessgroup)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._SaveTextFile(location, filenamelocalpath, textdata, accessgroup);
			}
			return false;
		}

		public static bool SaveTextFile(FileLocation location, string filenamelocalpath, string textdata)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._SaveTextFile(location, filenamelocalpath, textdata, null);
			}
			string fullPath = GetFullPath(location, filenamelocalpath);
			try
			{
				File.WriteAllText(fullPath, textdata);
				return true;
			}
			catch (Exception arg)
			{
				Logger.LogFatal(m_instance, "Text File saved Exception " + arg);
			}
			return false;
		}

		protected virtual byte[] _LoadBinaryFile(FileLocation location, string filenamelocalpath, string accessgroup)
		{
			return LoadBinaryFile(FileLocation.PERMANENT, filenamelocalpath);
		}

		public static byte[] LoadBinaryFile(FileLocation location, string filenamelocalpath, string accessgroup)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._LoadBinaryFile(location, filenamelocalpath, accessgroup);
			}
			return null;
		}

		public static byte[] LoadBinaryFile(FileLocation location, string filenamelocalpath)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._LoadBinaryFile(location, filenamelocalpath, null);
			}
			string fullPath = GetFullPath(location, filenamelocalpath);
			if (!File.Exists(fullPath))
			{
				Logger.LogWarning(m_instance, "Load binary file dosn't exist " + fullPath);
				return null;
			}
			try
			{
				return File.ReadAllBytes(fullPath);
			}
			catch (Exception arg)
			{
				Logger.LogFatal(m_instance, "Load binary file Exception " + arg);
			}
			return null;
		}

		protected virtual bool _SaveBinaryFile(FileLocation location, string filenamelocalpath, byte[] bytes, string accessgroup)
		{
			return SaveBinaryFile(FileLocation.PERMANENT, filenamelocalpath, bytes);
		}

		public static bool SaveBinaryFile(FileLocation location, string filenamelocalpath, byte[] bytes, string accessgroup)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._SaveBinaryFile(location, filenamelocalpath, bytes, accessgroup);
			}
			return false;
		}

		public static bool SaveBinaryFile(FileLocation location, string filenamelocalpath, byte[] bytes)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._SaveBinaryFile(location, filenamelocalpath, bytes, null);
			}
			string fullPath = GetFullPath(location, filenamelocalpath);
			try
			{
				File.WriteAllBytes(fullPath, bytes);
				Logger.LogInfo(m_instance, "Binary File successfully saved to " + fullPath);
				return true;
			}
			catch (Exception arg)
			{
				Logger.LogFatal(m_instance, "Binary File saved Exception " + arg);
			}
			return false;
		}

		public static bool SaveTextureToFile(Texture2D texture, FileLocation location, string filenamelocalpath)
		{
			if (texture == null)
			{
				return false;
			}
			byte[] bytes = texture.EncodeToPNG();
			return SaveBinaryFile(location, filenamelocalpath, bytes);
		}

		public static Texture2D LoadTextureFromFile(FileLocation location, string filenamelocalpath, int w, int h)
		{
			Texture2D texture2D = null;
			byte[] array = LoadBinaryFile(location, filenamelocalpath);
			if (array != null)
			{
				texture2D = new Texture2D(w, h);
				texture2D.LoadImage(array);
			}
			return texture2D;
		}

		protected virtual bool _DeleteFile(FileLocation location, string filenamelocalpath, string accessgroup)
		{
			return DeleteFile(FileLocation.PERMANENT, filenamelocalpath);
		}

		public static bool DeleteFile(FileLocation location, string filenamelocalpath, string accessgroup)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._DeleteFile(location, filenamelocalpath, accessgroup);
			}
			return false;
		}

		public static bool DeleteFile(FileLocation location, string filenamelocalpath)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._DeleteFile(location, filenamelocalpath, null);
			}
			string fullPath = GetFullPath(location, filenamelocalpath);
			if (!File.Exists(fullPath))
			{
				Logger.LogWarning(m_instance, "remove file dosn't exist " + fullPath);
				return false;
			}
			try
			{
				File.Delete(fullPath);
				return true;
			}
			catch (Exception arg)
			{
				Logger.LogFatal(m_instance, "Remove file Exception " + arg);
			}
			return false;
		}

		protected virtual bool _DeleteAllFiles(FileLocation location, string accessgroup)
		{
			return false;
		}

		public static bool DeleteAllFiles(FileLocation location, string accessgroup)
		{
			if (location == FileLocation.SHARED)
			{
				return Instance._DeleteAllFiles(location, accessgroup);
			}
			return false;
		}

		public static bool FileExists(FileLocation location, string filenamelocalpath)
		{
			string fullPath = GetFullPath(location, filenamelocalpath);
			return File.Exists(fullPath);
		}

		public static string GetExtension(string fname, string defaultext)
		{
			string result = defaultext;
			int num = fname.LastIndexOf('.') + 1;
			if (num < fname.Length)
			{
				string text = fname.Substring(num);
				if (!text.Contains("."))
				{
					result = text.ToLower();
				}
			}
			return result;
		}
	}
}
