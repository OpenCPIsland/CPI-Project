using ClubPenguin.Game.PartyGames;
using ClubPenguin.Interactables;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PartyGameLobbyHudInitializer : MonoBehaviour
	{
		public PrefabContentKey UIPrefabContentKey;

		public string ParentName = "HudLeftPanel";

		public bool IsHost = false;

		private GameObject hudObject;

		private void Start()
		{
			if (isLocalPlayer())
			{
				Content.LoadAsync(onHudPrefabLoaded, UIPrefabContentKey);
			}
		}

		private void onHudPrefabLoaded(string path, GameObject hudPrefab)
		{
			GameObject gameObject = GameObject.Find(ParentName);
			if (gameObject != null)
			{
				hudObject = Object.Instantiate(hudPrefab, gameObject.transform, false);
				PartyGameLobbyHud component = hudObject.GetComponent<PartyGameLobbyHud>();
				PartyGameLobbyHud.PartyGameLobbyHudState state = IsHost ? PartyGameLobbyHud.PartyGameLobbyHudState.HostLobby : PartyGameLobbyHud.PartyGameLobbyHudState.Lobby;
				component.SetState(state);
				InvitationalItemExperience component2 = GetComponent<InvitationalItemExperience>();
				if (GetComponent<InvitationalItemExperience>() != null)
				{
					component.SetInvitationalItem(component2);
				}
			}
		}

		private void OnDestroy()
		{
			if (hudObject != null)
			{
				Object.Destroy(hudObject);
			}
		}

		private bool isLocalPlayer()
		{
			PropExperience component = GetComponent<PropExperience>();
			if (component != null)
			{
				return component.IsOwnerLocalPlayer;
			}
			Prop component2 = GetComponent<Prop>();
			if (component2 != null)
			{
				return component2.IsOwnerLocalPlayer;
			}
			return false;
		}
	}
}
