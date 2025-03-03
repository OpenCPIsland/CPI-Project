using System;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class Asset
	{
		public string AssetName = "";

		public ResourceAssetType AssetType = ResourceAssetType.NONE;

		public ResourceEventHandler EventDelegate = null;

		public string Tag = "";

		public object LoadedObject = null;

		public Asset(string inName, ResourceEventHandler inCallback, ResourceAssetType inType, string tag)
		{
			AssetName = inName;
			AssetType = inType;
			EventDelegate = inCallback;
			Tag = tag;
		}

		public void AddCallback(ResourceEventHandler callback)
		{
			EventDelegate = (ResourceEventHandler)Delegate.Combine(EventDelegate, callback);
		}

		public bool ProcessResult(string fname)
		{
			Logger.LogInfo(this, "Loading cached res from file " + fname + " type = " + AssetType);
			LoadedObject = null;
			switch (AssetType)
			{
			case ResourceAssetType.ASSET_BUNDLE:
				Logger.LogWarning(this, "Wrong call!, should call void ProcessResult(bool success, AssetBundle bundle) ");
				break;
			case ResourceAssetType.BYTES:
				LoadedObject = FileManager.LoadBinaryFile(FileLocation.ABSOLUTE, fname);
				break;
			case ResourceAssetType.STRING:
				LoadedObject = FileManager.LoadTextFile(FileLocation.ABSOLUTE, fname);
				break;
			case ResourceAssetType.IMAGE:
			{
				Logger.LogInfo(this, "Creating image from file " + fname);
				byte[] array = FileManager.LoadBinaryFile(FileLocation.ABSOLUTE, fname);
				if (array != null)
				{
					Texture2D texture2D = new Texture2D(256, 256);
					texture2D.LoadImage(array);
					LoadedObject = texture2D;
				}
				break;
			}
			}
			if (EventDelegate != null)
			{
				EventDelegate(LoadedObject != null, Tag, LoadedObject);
			}
			return LoadedObject != null;
		}

		public bool ProcessResult(bool success, WWW inwww)
		{
			LoadedObject = null;
			if (success)
			{
				switch (AssetType)
				{
				case ResourceAssetType.AUDIO:
					LoadedObject = inwww.GetAudioClip();
					break;
				case ResourceAssetType.ASSET_BUNDLE:
					LoadedObject = inwww.assetBundle;
					break;
				case ResourceAssetType.BYTES:
					LoadedObject = inwww.bytes;
					break;
				case ResourceAssetType.IMAGE:
					LoadedObject = inwww.texture;
					break;
				case ResourceAssetType.NONSTREAMING_AUDIO:
					LoadedObject = inwww.GetAudioClip();
					break;
				case ResourceAssetType.TEXT_ASSET:
					LoadedObject = inwww.text;
					break;
				case ResourceAssetType.STRING:
					LoadedObject = inwww.text;
					break;
				}
			}
			if (EventDelegate != null)
			{
				EventDelegate(LoadedObject != null, Tag, LoadedObject);
			}
			return LoadedObject != null;
		}

		public bool ProcessResult(bool success, AssetBundle bundle)
		{
			bool success2 = success;
			LoadedObject = null;
			if (bundle != null)
			{
				if (success)
				{
					if (bundle != null)
					{
						if (AssetType != ResourceAssetType.ASSET_BUNDLE)
						{
							LoadedObject = bundle.LoadAsset(AssetName, GetAssetUnityType());
							if (AssetType == ResourceAssetType.BYTES)
							{
								TextAsset textAsset = LoadedObject as TextAsset;
								LoadedObject = null;
								if (textAsset != null)
								{
									LoadedObject = textAsset.bytes;
								}
							}
							else if (AssetType == ResourceAssetType.STRING)
							{
								TextAsset textAsset = LoadedObject as TextAsset;
								LoadedObject = null;
								if (textAsset != null)
								{
									LoadedObject = textAsset.text;
								}
							}
						}
						else
						{
							LoadedObject = bundle;
						}
					}
					else
					{
						Logger.LogWarning(this, "Resource not an AssetBundle: " + AssetName);
						success2 = false;
					}
				}
				else
				{
					Logger.LogWarning(this, "Resource async load failed : " + AssetName);
					success2 = false;
				}
			}
			else
			{
				Logger.LogWarning(this, "Resource async load failed bundle is null: " + AssetName);
				success2 = false;
			}
			if (EventDelegate != null)
			{
				EventDelegate(success2, Tag, LoadedObject);
			}
			return LoadedObject != null;
		}

		public void ReleaseResource()
		{
		}

		private Type GetAssetUnityType()
		{
			Type typeFromHandle = typeof(TextAsset);
			switch (AssetType)
			{
			case ResourceAssetType.AUDIO:
				typeFromHandle = typeof(AudioClip);
				break;
			case ResourceAssetType.ASSET_BUNDLE:
				typeFromHandle = typeof(AssetBundle);
				break;
			case ResourceAssetType.BYTES:
				typeFromHandle = typeof(TextAsset);
				break;
			case ResourceAssetType.IMAGE:
				typeFromHandle = typeof(Texture2D);
				break;
			case ResourceAssetType.NONSTREAMING_AUDIO:
				typeFromHandle = typeof(AudioClip);
				break;
			case ResourceAssetType.TEXT_ASSET:
				typeFromHandle = typeof(TextAsset);
				break;
			case ResourceAssetType.PREFAB:
				typeFromHandle = typeof(GameObject);
				break;
			case ResourceAssetType.STRING:
				typeFromHandle = typeof(string);
				break;
			}
			return typeFromHandle;
		}
	}
}
