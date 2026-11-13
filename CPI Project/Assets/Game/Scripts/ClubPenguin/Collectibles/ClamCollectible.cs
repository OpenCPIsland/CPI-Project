using ClubPenguin.Diving;
using UnityEngine;

namespace ClubPenguin.Collectibles
{
	public class ClamCollectible : Collectible
	{
		public GameObject ParticlesAppear;

		public GameObject ParticlesPickup;

		public float PickupRadius = 0.2f;

		private Renderer[] rendObjects;

		private ClamShell scriptClamShell;

		private ClamShellPearl scriptClamShellPearl;

		private Vector3 originalPearlPos;

		private GameObject parentObject;

		private bool isActive;

		private bool withinInfluenceRange;

		public override void Awake()
		{
			base.Awake();
			scriptClamShell = base.gameObject.GetComponentInParent<ClamShell>();
			scriptClamShellPearl = base.gameObject.GetComponent<ClamShellPearl>();
			originalPearlPos = base.gameObject.transform.position;
			parentObject = scriptClamShell.gameObject;
			rendObjects = parentObject.GetComponentsInChildren<Renderer>();
			withinInfluenceRange = false;
			setActive(false);
		}

		public override void StartCollectible(RespawnResponse respawnResponse)
		{
			isInitialized = true;
			checkIfCollected(respawnResponse);
		}

		private void checkIfCollected(RespawnResponse respawnResponse)
		{
			switch (respawnResponse.State)
			{
			case RespawnState.NOT_AVAILABLE:
				SetState(ClamState.NOT_AVAILABLE);
				break;
			case RespawnState.READY_FOR_PICKUP:
				SetState(ClamState.READY_FOR_PICKUP);
				break;
			case RespawnState.WAITING_TO_RESPAWN:
				registerToRespawn(respawnResponse.Time);
				SetState(ClamState.WAITING_TO_RESPAWN);
				break;
			}
		}

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(base.gameObject.transform.position, PickupRadius);
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.gameObject.name.Equals("InfluenceRange"))
			{
				withinInfluenceRange = true;
				setActive(true);
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			if (collider.gameObject.name.Equals("InfluenceRange"))
			{
				withinInfluenceRange = false;
				setActive(false);
			}
		}

		public void OnPickedUp()
		{
			if (isActive)
			{
				playAudioEvent();
				sendQuestEvent();
				registerToRespawn();
				sendCollectedEventLocal();
				sendCollectedEventServer(base.Path, originalPearlPos);
			}
		}

		private void SetState(ClamState newState)
		{
			switch (newState)
			{
			case ClamState.NONE:
				break;
			case ClamState.NOT_AVAILABLE:
				reset();
				setVisible(false);
				setActive(false);
				break;
			case ClamState.WAITING_TO_RESPAWN:
				reset();
				setVisible(false);
				setActive(false);
				break;
			case ClamState.READY_FOR_PICKUP:
				reset();
				scriptClamShell.ResetClamShell();
				setActive(true);
				if (withinInfluenceRange)
				{
					if (ParticlesAppear != null)
					{
						Object.Instantiate(ParticlesAppear, base.gameObject.transform.position, Quaternion.identity);
					}
					setVisible(true);
				}
				break;
			}
		}

		private void reset()
		{
			base.gameObject.transform.position = originalPearlPos;
		}

		private void setRenders(bool isVisible)
		{
			int num = rendObjects.Length;
			for (int i = 0; i < num; i++)
			{
				rendObjects[i].enabled = isVisible;
			}
		}

		private void setVisible(bool isVisible)
		{
			if (!isVisible)
			{
				scriptClamShell.HasBeenCollected();
			}
		}

		private void setActive(bool isEnabled)
		{
			scriptClamShellPearl.IsCollectible(isEnabled);
			isActive = isEnabled;
		}

		public override void RespawnCollectible()
		{
			SetState(ClamState.READY_FOR_PICKUP);
		}
	}
}
