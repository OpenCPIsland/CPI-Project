using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

public class TrayEventsAdapter : MonoBehaviour
{
	private EventChannel eventChannel;

	private StateMachineContext trayFSMContext;

	private void Start()
	{
		findWorldTrayFSMContext();
		addListeners();
	}

	public void OnDestroy()
	{
		removeListeners();
	}

	private void findWorldTrayFSMContext()
	{
		GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
		if (gameObject != null)
		{
			trayFSMContext = gameObject.GetComponent<StateMachineContext>();
		}
		else
		{
			Log.LogError(this, "Unable to find world tray state machine context on game object of tag : " + UIConstants.Tags.UI_Tray_Root);
		}
	}

	private void addListeners()
	{
		eventChannel = new EventChannel(Service.Get<EventDispatcher>());
		eventChannel.AddListener<TrayEvents.SelectTrayScreen>(onSelectTrayScreen);
		eventChannel.AddListener<TrayEvents.OpenTray>(onOpenTray);
		eventChannel.AddListener<TrayEvents.CloseTray>(onCloseTray);
	}

	private bool onSelectTrayScreen(TrayEvents.SelectTrayScreen evt)
	{
		string text = LegacyScreenToFSMEvent(evt.ScreenName);
		if (!string.IsNullOrEmpty(text))
		{
			trayFSMContext.SendEvent(new ExternalEvent("Root", "maxnpc"));
			trayFSMContext.SendEvent(new ExternalEvent("ScreenContainerContent", text));
		}
		return false;
	}

	private bool onOpenTray(TrayEvents.OpenTray evt)
	{
		exitCinematic();
		return false;
	}

	private void exitCinematic()
	{
		trayFSMContext.SendEvent(new ExternalEvent("Root", "exit_cinematic"));
	}

	private bool onCloseTray(TrayEvents.CloseTray evt)
	{
		trayFSMContext.SendEvent(new ExternalEvent("Root", "minnpc"));
		return false;
	}

	private void removeListeners()
	{
		eventChannel.RemoveAllListeners();
	}

	private string LegacyScreenToFSMEvent(string legacyScreenName)
	{
		string result = "";
		switch (legacyScreenName)
		{
		case "ControlsScreen":
			exitCinematic();
			break;
		case "QuestScreen":
			result = "quest";
			break;
		case "ConsumablesInventoryScreen":
			result = "consumable";
			break;
		case "FriendsScreen":
		case "MyPenguinScreen":
			result = "penguin";
			break;
		}
		return result;
	}
}
