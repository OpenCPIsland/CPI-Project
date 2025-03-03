using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class OpenGUITrayButton : MonoBehaviour
	{
		private const bool IS_PERSISTENT = true;

		public Image MainNavButtonIcon;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<TrayEvents.TrayOpened>(onTrayOpened);
			Service.Get<EventDispatcher>().AddListener<TrayEvents.TrayClosed>(onTrayClosed);
			base.gameObject.SetActive(false);
		}

		public void OnClick()
		{
			base.gameObject.SetActive(false);
			Service.Get<EventDispatcher>().DispatchEvent(new TrayEvents.OpenTray(true));
		}

		private bool onTrayOpened(TrayEvents.TrayOpened evt)
		{
			base.gameObject.SetActive(false);
			MainNavButtonIcon.enabled = true;
			base.transform.GetComponent<Button>().interactable = base.transform.parent.GetComponent<Button>().interactable;
			return false;
		}

		private bool onTrayClosed(TrayEvents.TrayClosed evt)
		{
			base.gameObject.SetActive(true);
			MainNavButtonIcon.enabled = false;
			base.transform.GetComponent<Button>().interactable = base.transform.parent.GetComponent<Button>().interactable;
			return false;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<TrayEvents.TrayOpened>(onTrayOpened);
			Service.Get<EventDispatcher>().RemoveListener<TrayEvents.TrayClosed>(onTrayClosed);
		}
	}
}
