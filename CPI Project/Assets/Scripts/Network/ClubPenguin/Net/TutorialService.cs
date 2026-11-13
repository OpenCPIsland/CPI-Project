using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;

namespace ClubPenguin.Net
{
	public class TutorialService : BaseNetworkService, ITutorialService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void SetTutorial(Tutorial tutorial)
		{
			APICall<SetTutorialOperation> aPICall = clubPenguinClient.TutorialApi.SetTutorial(tutorial);
			aPICall.OnResponse += onSetTutorialReceived;
			aPICall.Execute();
		}

		private void onSetTutorialReceived(SetTutorialOperation operation, HttpResponse httpResponse)
		{
			byte[] array = new byte[operation.TutorialResponse.tutorialBytes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)operation.TutorialResponse.tutorialBytes[i];
			}
			Service.Get<EventDispatcher>().DispatchEvent(new TutorialServiceEvents.TutorialReceived(array));
		}
	}
}
