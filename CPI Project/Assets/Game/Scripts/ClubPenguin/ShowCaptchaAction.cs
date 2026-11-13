using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin
{
	[ActionCategory("Misc")]
	public class ShowCaptchaAction : FsmStateAction
	{
		private EventDispatcher eventDispatcher;

		private ShowCaptchaCommand showCaptchaCommand;

		public override void OnEnter()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<CaptchaServiceEvents.CaptchaSolutionAccepted>(onCaptchaSolutionAccepted);
			showCaptchaCommand = new ShowCaptchaCommand();
			showCaptchaCommand.Execute(CaptchaType.CREATE_ACCOUNT, 445, 445);
		}

		public override void OnExit()
		{
			eventDispatcher.RemoveListener<CaptchaServiceEvents.CaptchaSolutionAccepted>(onCaptchaSolutionAccepted);
		}

		private bool onCaptchaSolutionAccepted(CaptchaServiceEvents.CaptchaSolutionAccepted evt)
		{
			Finish();
			return false;
		}
	}
}
