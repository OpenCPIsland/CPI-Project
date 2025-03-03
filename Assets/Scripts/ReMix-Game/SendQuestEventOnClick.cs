using ClubPenguin.Adventure;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

public class SendQuestEventOnClick : MonoBehaviour
{
	public string QuestEvent;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(onButtonClick);
	}

	public void OnDestroy()
	{
		GetComponent<Button>().onClick.RemoveListener(onButtonClick);
	}

	private void onButtonClick()
	{
		QuestService questService = Service.Get<QuestService>();
		questService.SendEvent(QuestEvent);
	}
}
