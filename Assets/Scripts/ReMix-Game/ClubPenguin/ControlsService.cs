using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class ControlsService : AbstractDataModelService
	{
		private EventChannel eventChannel;

		private ControlsScreenData controlsScreenData;

		private void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<ControlsScreenEvents.SetLeftOption>(onSetLeftOption);
			eventChannel.AddListener<ControlsScreenEvents.SetDefaultLeftOption>(onSetDefaultLeftOption);
			eventChannel.AddListener<ControlsScreenEvents.ReturnToDefaultLeftOption>(onReturnToDefaultLeftOption);
			eventChannel.AddListener<ControlsScreenEvents.SetRightOption>(onSetRightOption);
			eventChannel.AddListener<ControlsScreenEvents.ReturnToDefaultRightOption>(onReturnToDefaultRightOption);
			eventChannel.AddListener<ControlsScreenEvents.SetButton>(onSetButton);
			eventChannel.AddListener<SessionEvents.SessionEndedEvent>(onSessionEnded);
			DataEntityHandle handle = dataEntityCollection.AddEntity("ControlsScreenData");
			controlsScreenData = dataEntityCollection.AddComponent<ControlsScreenData>(handle);
		}

		private bool onSetLeftOption(ControlsScreenEvents.SetLeftOption evt)
		{
			controlsScreenData.SetLeftOption(evt.LeftOptionContentKey);
			return true;
		}

		private bool onSetDefaultLeftOption(ControlsScreenEvents.SetDefaultLeftOption evt)
		{
			controlsScreenData.DefaultLeftOptionPrefab = evt.DefaultLeftOptionPrefab;
			return true;
		}

		private bool onReturnToDefaultLeftOption(ControlsScreenEvents.ReturnToDefaultLeftOption evt)
		{
			controlsScreenData.ReturnToDefaultLeftOption();
			return true;
		}

		private bool onSetRightOption(ControlsScreenEvents.SetRightOption evt)
		{
			controlsScreenData.SetRightOption(evt.RightButtonGroupContentKey);
			return true;
		}

		private bool onReturnToDefaultRightOption(ControlsScreenEvents.ReturnToDefaultRightOption evt)
		{
			controlsScreenData.ReturnToDefaultRightOption();
			return true;
		}

		private bool onSetButton(ControlsScreenEvents.SetButton evt)
		{
			controlsScreenData.SetButton(evt.ButtonContentKey, evt.ButtonIndex);
			return true;
		}

		private bool onSessionEnded(SessionEvents.SessionEndedEvent evt)
		{
			controlsScreenData.ReturnToDefaultLeftOption();
			controlsScreenData.ReturnToDefaultRightOption();
			return false;
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}
	}
}
