using UnityEngine;

namespace ClubPenguin.Collectibles
{
	public class ExplorationCollectible : Collectible
	{
		public GameObject ParticlesAppear;

		public GameObject ParticlesPickup;

		public Transform ViewTransform;

		[Tooltip("Offset from player's feet (zero-point")]
		public Vector3 targetOffset = new Vector3(0f, 0.25f, 0f);

		public float PickupRadius = 0.7f;

		public bool WillAttractToPlayer = true;

		[HideInInspector]
		public bool IsVisible = false;

		private Renderer rendObject;

		private ParticleSystem partSys;

		private MovementAnimationCurve scriptAnimCurve;

		private RotateTransform scriptRotate;

		private Vector3 originalPos;

		private bool isMagnetized = false;

		private GameObject playerObj = null;

		private bool isActive;

		private ExplorationState internalState = ExplorationState.NONE;

		private bool withinInfluenceRange;

		private float attractionSpeed = 1f;

		private float attractionStartTime;

		private float attractionDistanceTotal;

		public override void Awake()
		{
			base.Awake();
			rendObject = base.gameObject.GetComponentInChildren<Renderer>();
			partSys = base.gameObject.GetComponentInChildren<ParticleSystem>();
			scriptAnimCurve = base.gameObject.GetComponentInChildren<MovementAnimationCurve>();
			scriptRotate = base.gameObject.GetComponentInChildren<RotateTransform>();
			if (ViewTransform == null)
			{
				ViewTransform = base.gameObject.GetComponent<Transform>();
			}
			originalPos = ViewTransform.position;
			if (scriptAnimCurve != null)
			{
				float num = base.gameObject.transform.position.x / 1.5f + base.gameObject.transform.position.y / 1.5f + base.gameObject.transform.position.z / 1.5f;
				scriptAnimCurve.startPosition = Mathf.Abs(num - Mathf.Round(num));
			}
			withinInfluenceRange = false;
			setVisible(false);
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
				SetState(ExplorationState.NOT_AVAILABLE);
				break;
			case RespawnState.READY_FOR_PICKUP:
				SetState(ExplorationState.READY_FOR_PICKUP);
				break;
			case RespawnState.WAITING_TO_RESPAWN:
				registerToRespawn(respawnResponse.Time);
				SetState(ExplorationState.WAITING_TO_RESPAWN);
				break;
			}
		}

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(base.gameObject.transform.position + targetOffset, PickupRadius);
		}

		private void Update()
		{
			if (isMagnetized && isActive)
			{
				magnetAttraction();
			}
		}

		public override void OnProximityEnter(ProximityListener other)
		{
			if (internalState == ExplorationState.READY_FOR_PICKUP)
			{
				withinInfluenceRange = true;
				setActive(true);
			}
		}

		public override void OnProximityExit(ProximityListener other)
		{
			if (internalState == ExplorationState.READY_FOR_PICKUP && !isMagnetized)
			{
				withinInfluenceRange = false;
				setActive(false);
			}
		}

		public override void OnProximityStay(ProximityListener other)
		{
			if (isActive && !isMagnetized)
			{
				isMagnetized = true;
				playerObj = other.gameObject;
				sendQuestEvent();
				registerToRespawn();
				if (scriptAnimCurve != null)
				{
					scriptAnimCurve.enabled = false;
				}
				if (scriptRotate != null)
				{
					scriptRotate.enabled = false;
				}
				attractionStartTime = Time.time;
				attractionDistanceTotal = Vector3.Distance(ViewTransform.position, playerObj.transform.position + targetOffset);
				sendCollectedEventLocal();
				sendCollectedEventServer(base.Path, originalPos);
			}
		}

		private void magnetAttraction()
		{
			if (!isActive || !(playerObj != null))
			{
				return;
			}
			float num = Vector3.SqrMagnitude(ViewTransform.position - (playerObj.transform.position + targetOffset));
			if (num <= PickupRadius * PickupRadius)
			{
				playAudioEvent();
				if (ParticlesPickup != null)
				{
					Object.Instantiate(ParticlesPickup, ViewTransform.position, Quaternion.identity);
				}
				SetState(ExplorationState.WAITING_TO_RESPAWN);
			}
			else if (WillAttractToPlayer)
			{
				float num2 = (Time.time - attractionStartTime) * attractionSpeed;
				float t = num2 / attractionDistanceTotal;
				ViewTransform.position = Vector3.Lerp(ViewTransform.position, playerObj.transform.position + targetOffset, t);
			}
		}

		private void SetState(ExplorationState newState)
		{
			switch (newState)
			{
			case ExplorationState.NOT_AVAILABLE:
				reset();
				setVisible(false);
				setActive(false);
				internalState = newState;
				break;
			case ExplorationState.WAITING_TO_RESPAWN:
				reset();
				setVisible(false);
				setActive(false);
				internalState = newState;
				break;
			case ExplorationState.READY_FOR_PICKUP:
				setVisible(true);
				if (withinInfluenceRange)
				{
					setActive(true);
					if (ParticlesAppear != null)
					{
						Object.Instantiate(ParticlesAppear, base.gameObject.transform.position, Quaternion.identity);
					}
				}
				internalState = newState;
				break;
			case ExplorationState.NONE:
				internalState = newState;
				break;
			default:
				internalState = ExplorationState.NONE;
				break;
			}
		}

		private void reset()
		{
			isMagnetized = false;
			ViewTransform.position = originalPos;
		}

		private void setVisible(bool isVisible)
		{
			if (rendObject != null)
			{
				rendObject.enabled = isVisible;
			}
			IsVisible = isVisible;
		}

		private void setActive(bool isEnabled)
		{
			if (scriptAnimCurve != null)
			{
				scriptAnimCurve.enabled = isEnabled;
			}
			if (scriptRotate != null)
			{
				scriptRotate.enabled = isEnabled;
			}
			isActive = isEnabled;
			setParticles(isEnabled);
		}

		private void setParticles(bool isEnabled)
		{
			if (partSys != null)
			{
				if (isEnabled)
				{
					partSys.Play();
					return;
				}
				partSys.Stop();
				partSys.Clear();
			}
		}

		public override void RespawnCollectible()
		{
			SetState(ExplorationState.READY_FOR_PICKUP);
		}
	}
}
