using ClubPenguin.Mix;
using Disney.Mix.SDK;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class RememberMeSoftLoginStateHandler : AbstractAccountStateHandler
	{
		public string SoftLoginFailEvent;

		private bool active;

		private void OnEnable()
		{
			Service.Get<MixLoginCreateService>().OnSoftLoginFailed += onSoftLoginFailed;
		}

		private void OnDisable()
		{
			Service.Get<MixLoginCreateService>().OnSoftLoginFailed -= onSoftLoginFailed;
		}

		private void onSoftLoginFailed(IRestoreLastSessionResult result)
		{
			if (active)
			{
				rootStateMachine.SendEvent(SoftLoginFailEvent);
			}
		}

		public void OnStateChanged(string state)
		{
			if (state == HandledState && rootStateMachine != null)
			{
				active = true;
			}
			else
			{
				active = false;
			}
		}
	}
}
