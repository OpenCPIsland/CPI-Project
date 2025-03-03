using ClubPenguin.Locomotion;
using ClubPenguin.Props;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameSpawnLocationValidator : PropSpawnLocationValidator
	{
		public Transform[] PlayerPositions;

		public float MaxVerticalDistance = 0.5f;

		public bool IsValidSwimming;

		public bool IsValidDiving;

		private int hittingCollidersCount;

		private bool isValidLocomotionController;

		private int raycastLayerMask;

		private Vector3[] raycastedPlayerPositions;

		private GameObject localPlayerGO;

		private void Awake()
		{
			string[] layerNames = new string[3]
			{
				"WorldMap",
				"TerrainBarrier",
				"InvisibleBarrier"
			};
			raycastLayerMask = LayerMask.GetMask(layerNames);
			raycastedPlayerPositions = new Vector3[PlayerPositions.Length];
		}

		private void Start()
		{
			localPlayerGO = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			localPlayerGO.GetComponent<LocomotionEventBroadcaster>().OnControllerChangedEvent += onLocomotionControllerChanged;
			onLocomotionControllerChanged(LocomotionHelper.GetCurrentController(localPlayerGO));
		}

		private void Update()
		{
			if (hittingCollidersCount == 0)
			{
				if (checkPlayerGroundPositionRaycasts() && isValidLocomotionController)
				{
					setPositionValid(true);
				}
				else
				{
					setPositionValid(false);
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			hittingCollidersCount++;
			setPositionValid(false);
		}

		private void OnTriggerExit(Collider other)
		{
			hittingCollidersCount--;
		}

		public Vector3 GetPlayerPosition(int playerIndex)
		{
			if (playerIndex < 0 || playerIndex >= raycastedPlayerPositions.Length)
			{
				return Vector3.zero;
			}
			return raycastedPlayerPositions[playerIndex];
		}

		protected override void onDestroy()
		{
			if (localPlayerGO != null)
			{
				localPlayerGO.GetComponent<LocomotionEventBroadcaster>().OnControllerChangedEvent -= onLocomotionControllerChanged;
			}
		}

		private void onLocomotionControllerChanged(LocomotionController newController)
		{
			if (!IsValidSwimming && newController is SwimController && (newController as SwimController).IsInShallowWater)
			{
				isValidLocomotionController = false;
			}
			else if (!IsValidDiving && newController is SwimController && !(newController as SwimController).IsInShallowWater)
			{
				isValidLocomotionController = false;
			}
			else
			{
				isValidLocomotionController = true;
			}
		}

		private bool checkPlayerGroundPositionRaycasts()
		{
			if (PlayerPositions.Length == 0)
			{
				return true;
			}
			for (int i = 0; i < PlayerPositions.Length; i++)
			{
				Vector3 position = PlayerPositions[i].position;
				RaycastHit hitInfo;
				if (Physics.Raycast(position, Vector3.down, out hitInfo, MaxVerticalDistance, raycastLayerMask))
				{
					if (hitInfo.point.y - position.y < MaxVerticalDistance)
					{
						raycastedPlayerPositions[i] = hitInfo.point;
						continue;
					}
					return false;
				}
				return false;
			}
			return true;
		}
	}
}
