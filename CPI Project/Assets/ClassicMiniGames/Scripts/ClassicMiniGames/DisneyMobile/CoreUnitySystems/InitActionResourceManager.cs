using System.Collections;

namespace DisneyMobile.CoreUnitySystems
{
	public class InitActionResourceManager : CrossPlatformInitAction
	{
		public DelegateReuiredResourceFailedCallback FailedCallback = null;

		public InitActionResourceManager()
			: base(typeof(ResourceManager), true)
		{
		}

		public override IEnumerator Perform()
		{
			CreateInstance();
			if (base.Instance != null)
			{
				ResourceManager resourceManager = base.Instance as ResourceManager;
				resourceManager.Initialize(CallbackResult);
			}
			return WaitTillComplete();
		}

		public void CallbackResult(bool success, string msg)
		{
			Logger.LogInfo(this, "Resourcemanager init returns " + success + " msg = " + msg);
			if (!success && FailedCallback != null)
			{
				FailedCallback(msg);
			}
			OnComplete();
		}
	}
}
