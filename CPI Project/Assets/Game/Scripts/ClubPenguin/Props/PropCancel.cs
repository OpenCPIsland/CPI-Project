using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(Prop))]
	[DisallowMultipleComponent]
	public class PropCancel : MonoBehaviour
	{
		private const int partySuppliesButtonIndex = 0;

		private const int cancelButtonIndex = 3;

		[SerializeField]
		private bool ShowConfirmationPopup = false;

		[SerializeField]
		private bool WatchActionEventCancel = true;

		[SerializeField]
		private string i18nConfirmationTitleText;

		[SerializeField]
		private string i18nConfirmationBodyText;

		[Tooltip("This will set the image in the prompt without loading it.")]
		[SerializeField]
		[Header("Optional Image Override")]
		private Sprite ImageOverride = null;

		public bool RestoreTouchPadControlsOnCancel = false;

		private Prop prop;

		private PropUser propUser;

		private EventChannel eventChannel;

		public void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			prop = GetComponent<Prop>();
			if (WatchActionEventCancel)
			{
				eventChannel.AddListener<InputEvents.ActionEvent>(onActionEvent);
			}
		}

		public void Start()
		{
			if (!SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.IsDestroyed())
			{
				propUser = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PropUser>();
				propUser.EPropRemoved += onPropRemoved;
			}
		}

		public void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			if (propUser != null)
			{
				propUser.EPropRemoved -= onPropRemoved;
			}
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			if (prop.IsOwnerLocalPlayer && base.enabled && evt.Action == InputEvents.Actions.Cancel)
			{
				if (ShowConfirmationPopup)
				{
					DPrompt data = new DPrompt(i18nConfirmationTitleText, i18nConfirmationBodyText, DPrompt.ButtonFlags.CANCEL | DPrompt.ButtonFlags.OK, ImageOverride);
					Service.Get<PromptManager>().ShowPrompt(data, onConfirmationPromptButtonsClicked);
				}
				else
				{
					Service.Get<ActionConfirmationService>().ConfirmAction(typeof(PropCancel), null, delegate
					{
						UnequipProp();
					});
				}
				if (RestoreTouchPadControlsOnCancel)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
				}
			}
			return false;
		}

		private void onPropRemoved(Prop prop)
		{
			if (this.prop.name.Equals(prop.name))
			{
				CancelFSM();
			}
		}

		private void CancelFSM()
		{
			if (prop.IsOwnerLocalPlayer)
			{
				PlayMakerFSM component = base.gameObject.GetComponent<PlayMakerFSM>();
				if (component != null)
				{
					component.SendEvent("CancelFSM");
				}
			}
		}

		private void onConfirmationPromptButtonsClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.OK)
			{
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(PropCancel), null, delegate
				{
					UnequipProp();
				});
			}
		}

		public void UnequipProp(bool immediate = false)
		{
			if (prop.IsOwnerLocalPlayer)
			{
				Service.Get<PropService>().LocalPlayerStoreProp();
				if (immediate)
				{
					prop.PropUserRef.RemoveProp();
					prop.gameObject.SetActive(false);
					prop.PropUserRef.ResetPropControls(true);
				}
			}
			else
			{
				prop.PropUserRef.RemoveProp();
				prop.gameObject.SetActive(false);
			}
			if (prop.PropDef.PropType == PropDefinition.PropTypes.Consumable)
			{
				Service.Get<ICPSwrveService>().Timing(Mathf.RoundToInt(Time.timeSinceLevelLoad), "consumable_store", prop.PropDef.name);
			}
		}
	}
}
