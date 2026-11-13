using ClubPenguin.Interactables;
using ClubPenguin.PartyGames;
using ClubPenguin.Props;
using ClubPenguin.UI;
using System;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	[RequireComponent(typeof(InputButtonMapper))]
	public class PartyGameLobbyStartButton : MonoBehaviour
	{
		private InvitationalItemExperience experience;

		private PartyGameDefinition partyGameDefinition;

		private TrayInputButtonDisabler buttonDisabler;

		private bool isEnablingStartButton;

		private void Start()
		{
			getLocalPlayerInvitationalItemExperience();
			getPartyGameDefinitionForLocalPlayerPropExperience();
			getButtonDisabler();
			disableStartButton();
		}

		private void OnDestroy()
		{
			if (experience != null)
			{
				InvitationalItemExperience invitationalItemExperience = experience;
				invitationalItemExperience.AvailableItemQuantityChangedAction = (Action<int>)Delegate.Remove(invitationalItemExperience.AvailableItemQuantityChangedAction, new Action<int>(onExperienceAvailabelItemQuantityChanged));
			}
			enableStartButton();
		}

		private void onExperienceAvailabelItemQuantityChanged(int availableQuantity)
		{
			if (!(partyGameDefinition == null))
			{
				int num = experience.TotalItemQuantity - availableQuantity + 1;
				if (num >= partyGameDefinition.MinPlayerCount && !isEnablingStartButton)
				{
					enableStartButton();
				}
				else if (num < partyGameDefinition.MinPlayerCount && isEnablingStartButton)
				{
					disableStartButton();
				}
			}
		}

		private void getLocalPlayerInvitationalItemExperience()
		{
			experience = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponentInChildren<InvitationalItemExperience>();
			if (experience != null)
			{
				InvitationalItemExperience invitationalItemExperience = experience;
				invitationalItemExperience.AvailableItemQuantityChangedAction = (Action<int>)Delegate.Combine(invitationalItemExperience.AvailableItemQuantityChangedAction, new Action<int>(onExperienceAvailabelItemQuantityChanged));
			}
			else
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void getPartyGameDefinitionForLocalPlayerPropExperience()
		{
			PropExperience componentInChildren = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponentInChildren<PropExperience>();
			if (componentInChildren == null)
			{
				UnityEngine.Object.Destroy(this);
			}
			partyGameDefinition = PartyGameUtils.GetPartyGameForTriggerProp(componentInChildren.PropDef.Id);
			if (partyGameDefinition == null)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void enableStartButton()
		{
			buttonDisabler.EnableElement();
			isEnablingStartButton = true;
		}

		private void disableStartButton()
		{
			buttonDisabler.DisableElement(false);
			isEnablingStartButton = false;
		}

		private void getButtonDisabler()
		{
			buttonDisabler = GetComponentInParent<TrayInputButtonDisabler>();
			if (buttonDisabler == null)
			{
				UnityEngine.Object.Destroy(this);
			}
		}
	}
}
