using ClubPenguin.Analytics;
using ClubPenguin.Audio;
using ClubPenguin.Breadcrumbs;
using ClubPenguin.Chat;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ChatEmoteItem : MonoBehaviour
	{
		public Text EmoteIconText;

		public TutorialBreadcrumb TutorialBreadcrumb;

		public Image RecentEmoteImage;

		private EmoteDefinition emoteDefinition;

		private string emoteString;

		public string EmoteId
		{
			get
			{
				return (emoteDefinition != null) ? emoteDefinition.Id : string.Empty;
			}
		}

		public void SetEmote(EmoteDefinition emoteDefinition)
		{
			if (emoteDefinition != null)
			{
				if (emoteDefinition.Id == "RecentEmoteKey")
				{
					EmoteIconText.text = null;
					GetComponent<Button>().interactable = false;
					RecentEmoteImage.enabled = true;
					return;
				}
				this.emoteDefinition = emoteDefinition;
				if (!string.IsNullOrEmpty(emoteDefinition.Sound))
				{
					disableClickSound();
				}
				emoteString = EmoteManager.GetEmoteString(emoteDefinition);
				EmoteIconText.text = emoteString;
				AccessibilitySettings component = GetComponent<AccessibilitySettings>();
				component.CustomToken = emoteDefinition.Token;
				TutorialBreadcrumb.SetBreadcrumbId(string.Format("Emote_{0}", emoteDefinition.name));
				GetComponent<Button>().interactable = true;
				RecentEmoteImage.enabled = false;
			}
			else
			{
				EmoteIconText.text = null;
				GetComponent<Button>().interactable = false;
				RecentEmoteImage.enabled = false;
			}
		}

		private void disableClickSound()
		{
			ButtonToFabricEvents component = GetComponent<ButtonToFabricEvents>();
			if (component != null)
			{
				component.enabled = false;
			}
		}

		public void OnClick()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new ChatEvents.EmoteSelected(emoteString));
			StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
			if (componentInParent != null)
			{
				Service.Get<ICPSwrveService>().Action("game.emoji", emoteDefinition.name);
			}
			Service.Get<TutorialBreadcrumbController>().RemoveBreadcrumb(TutorialBreadcrumb.BreadcrumbId);
		}
	}
}
