using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class GcsP12AssetFileLoader : GcsP12FileLoader
	{
		private string gcsServiceAccountFile;

		public GcsP12AssetFileLoader(string gcsServiceAccountFile)
		{
			this.gcsServiceAccountFile = gcsServiceAccountFile;
		}

		public byte[] Load()
		{
			TextAsset textAsset = Resources.Load<TextAsset>(gcsServiceAccountFile);
			if (textAsset == null)
			{
				Log.LogErrorFormatted(this, "GetOAuthRequest(): {0} NOT FOUND!", gcsServiceAccountFile);
			}
			return Convert.FromBase64String(textAsset.text);
		}
	}
}
