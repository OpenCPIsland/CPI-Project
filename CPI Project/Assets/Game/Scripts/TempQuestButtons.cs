using ClubPenguin.Adventure;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

public class TempQuestButtons : MonoBehaviour
{
	public Button SuspendButton;

	public void OnEnable()
	{
		SuspendButton.onClick.AddListener(OnSuspendClicked);
	}

	public void OnDisable()
	{
		SuspendButton.onClick.RemoveListener(OnSuspendClicked);
	}

	public void OnSuspendClicked()
	{
		Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.SuspendQuest(Service.Get<QuestService>().ActiveQuest));
		Service.Get<EventDispatcher>().DispatchEvent(new TrayEvents.SelectTrayScreen("ControlsScreen"));
	}
}
