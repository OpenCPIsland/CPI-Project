using System.Collections;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class InitActionFileManager : CrossPlatformInitAction
	{
		public InitActionFileManager()
			: base(typeof(FileManager), true)
		{
			AddType(RuntimePlatform.IPhonePlayer, typeof(FileIOSManager));
			AddType(RuntimePlatform.Android, typeof(FileAndroidManager));
		}

		public override IEnumerator Perform()
		{
			CreateInstance();
			OnComplete();
			yield return null;
		}
	}
}
