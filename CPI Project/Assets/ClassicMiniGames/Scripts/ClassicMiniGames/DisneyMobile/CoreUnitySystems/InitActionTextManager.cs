using System.Collections;

namespace DisneyMobile.CoreUnitySystems
{
	public class InitActionTextManager : CrossPlatformInitAction
	{
		public InitActionTextManager()
			: base(typeof(TextManager), true)
		{
		}

		public override IEnumerator Perform()
		{
			CreateInstance();
			if (base.Instance != null)
			{
				TextManager textManager = base.Instance as TextManager;
				textManager.Initialize();
			}
			OnComplete();
			yield return null;
		}
	}
}
