using System.Collections;

namespace DisneyMobile.CoreUnitySystems
{
	public class InitActionInputManager : CrossPlatformInitAction
	{
		public InitActionInputManager()
			: base(typeof(InputManager), true)
		{
		}

		public override IEnumerator Perform()
		{
			CreateInstance();
			if (base.Instance != null)
			{
			}
			OnComplete();
			yield return null;
		}
	}
}
