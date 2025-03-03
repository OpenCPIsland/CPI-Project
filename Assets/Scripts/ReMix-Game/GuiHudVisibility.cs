using ClubPenguin;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

public class GuiHudVisibility : MonoBehaviour
{
	private GameObject GuiHudCanvas;

	private EventDispatcher eventDispatcher;

	private void Awake()
	{
		eventDispatcher = Service.Get<EventDispatcher>();
	}

	private void Start()
	{
		GuiHudCanvas = GetComponentInChildren<Canvas>().gameObject;
		eventDispatcher.AddListener<ChatEvents.ShowFullScreen>(onShowFullscreenChat);
		eventDispatcher.AddListener<ChatEvents.HideFullScreen>(onHideFullscreenChat);
	}

	public void OnDestroy()
	{
		eventDispatcher.RemoveListener<ChatEvents.ShowFullScreen>(onShowFullscreenChat);
		eventDispatcher.RemoveListener<ChatEvents.HideFullScreen>(onHideFullscreenChat);
	}

	private bool onHideFullscreenChat(ChatEvents.HideFullScreen evt)
	{
		GuiHudCanvas.SetActive(true);
		return false;
	}

	private bool onShowFullscreenChat(ChatEvents.ShowFullScreen evt)
	{
		GuiHudCanvas.SetActive(false);
		return false;
	}
}
