using Disney.LaunchPadFramework;
using Fabric;
using UnityEngine;

namespace ClubPenguin.Collectibles
{
	public class SceneryCollectible : Collectible
	{
		public GameObject ParticlesAppear;

		public GameObject ParticlesPickup;

		public GameObject ParticlesPickupAnchor;

		public float AttractForce = 150f;

		public float PickupRadius = 0.7f;

		[Tooltip("Drag the interaction object here")]
		public GameObject InteractionObj;

		[Tooltip("Controls whether object automatically attracts to player after it stops")]
		public bool AutomaticPickup = true;

		[Tooltip("Changes pickup scattering to 2D for Diving")]
		public bool IsDivingCollectible = false;

		[Tooltip("Offset from player's feet (zero-point")]
		public Vector3 TargetOffset = new Vector3(0f, 0.25f, 0f);

		[Header("Force When Activated")]
		public Vector2 RangeX = new Vector2(1f, 2f);

		public Vector2 RangeY = new Vector2(1f, 2f);

		public Vector2 RangeZ = new Vector2(1f, 2f);

		[Header("Special objects, affected when collectible activated")]
		public GameObject[] ObjectsToHide;

		[HideInInspector]
		public bool IsVisible = false;

		private MeshRenderer meshRend;

		private ParticleSystem partSys;

		private RotateTransform rotateScript;

		private MovementAnimationCurve animCurveScript;

		private Rigidbody rigidBody;

		private Vector3 originalPos;

		private Quaternion originalRot;

		private SphereCollider sphereColl;

		private PhysicMaterial slipperyPhysicMaterial;

		private PhysicMaterial stickyPhysicMaterial;

		private SceneryCollectibleTrigger triggerScript;

		private SceneryState internalState = SceneryState.NONE;

		private GameObject playerObj = null;

		private bool hasLanded = false;

		private bool isMagnetic = false;

		private Camera activeCamera;

		public Transform originalParent;

		private Vector3 originalLossyScale;

		private Vector3 originalLocalScale;

		private float attractionSpeed = 1f;

		private float attractionStartTime;

		private float attractionDistanceTotal;

		public override void Awake()
		{
			base.Awake();
			if (InteractionObj != null)
			{
				base.InteractionPath = InteractionObj.GetPath();
			}
			else
			{
				Log.LogError(this, string.Format("{0} requires an interation object to be defined on the prefab.", base.Path));
			}
			partSys = base.gameObject.GetComponentInChildren<ParticleSystem>();
			rotateScript = base.gameObject.GetComponentInChildren<RotateTransform>();
			animCurveScript = base.gameObject.GetComponent<MovementAnimationCurve>();
			rigidBody = base.gameObject.GetComponent<Rigidbody>();
			triggerScript = base.gameObject.GetComponentInChildren<SceneryCollectibleTrigger>();
			meshRend = base.gameObject.GetComponent<MeshRenderer>();
			sphereColl = base.gameObject.GetComponent<SphereCollider>();
			slipperyPhysicMaterial = new PhysicMaterial();
			slipperyPhysicMaterial.dynamicFriction = 0f;
			slipperyPhysicMaterial.staticFriction = 0f;
			slipperyPhysicMaterial.bounciness = 0f;
			slipperyPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
			slipperyPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
			stickyPhysicMaterial = new PhysicMaterial();
			stickyPhysicMaterial.dynamicFriction = float.PositiveInfinity;
			stickyPhysicMaterial.staticFriction = float.PositiveInfinity;
			stickyPhysicMaterial.bounciness = 0f;
			stickyPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
			stickyPhysicMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
			if (animCurveScript != null)
			{
				float num = base.gameObject.transform.position.x / 1.5f + base.gameObject.transform.position.y / 1.5f + base.gameObject.transform.position.z / 1.5f;
				animCurveScript.startPosition = Mathf.Abs(num - Mathf.Round(num));
			}
			if (rigidBody != null)
			{
				rigidBody.angularDrag = float.PositiveInfinity;
			}
			else
			{
				Log.LogError(this, string.Format("Error: collectible '{0}' is missing a rigidbody, will not function correctly", base.gameObject.GetPath()));
			}
			originalPos = base.gameObject.transform.position;
			originalRot = base.gameObject.transform.rotation;
			if (Camera.main != null)
			{
				activeCamera = Camera.main;
			}
			originalParent = base.gameObject.transform.parent;
			originalLossyScale = base.gameObject.transform.lossyScale;
			originalLocalScale = base.gameObject.transform.localScale;
			changeState(SceneryState.INVISIBLE);
		}

		public override void Start()
		{
			base.Start();
			playerObj = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			Object.Destroy(slipperyPhysicMaterial);
			Object.Destroy(stickyPhysicMaterial);
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
				hide();
				break;
			case RespawnState.READY_FOR_PICKUP:
				respawn();
				break;
			case RespawnState.WAITING_TO_RESPAWN:
				registerToRespawn(respawnResponse.Time);
				hide();
				break;
			}
		}

		public override void OnDrawGizmosSelected()
		{
			base.OnDrawGizmosSelected();
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(base.gameObject.transform.position, PickupRadius);
		}

		private void Update()
		{
			if (IsDivingCollectible && internalState == SceneryState.SPAWN)
			{
				checkVelocity();
			}
			if (isMagnetic && internalState == SceneryState.MAGNETIC)
			{
				lockRotation();
				magnetAttraction();
			}
		}

		public void OnActionGraphActivation()
		{
			if (internalState == SceneryState.READY_TO_ACTIVATE)
			{
				addUpwardsForce();
				sendQuestEvent();
				setVisibleSpecialObjects(false);
			}
		}

		private void changeState(SceneryState newState)
		{
			if (newState == internalState)
			{
				return;
			}
			switch (newState)
			{
			case SceneryState.NONE:
				break;
			case SceneryState.READY_TO_ACTIVATE:
				base.gameObject.transform.parent = originalParent;
				base.gameObject.transform.localScale = originalLocalScale;
				internalState = newState;
				scriptControl(false);
				isMagnetic = false;
				rigidBody.drag = 0f;
				rigidBody.useGravity = false;
				rigidBody.isKinematic = true;
				sphereColl.enabled = false;
				sphereColl.material = stickyPhysicMaterial;
				base.gameObject.transform.position = originalPos;
				base.gameObject.transform.rotation = originalRot;
				hasLanded = false;
				setVisible(true);
				setVisibleSpecialObjects(true);
				if (InteractionObj != null)
				{
					InteractionObj.SetActive(true);
				}
				break;
			case SceneryState.SPAWN:
				base.gameObject.transform.parent = originalParent.parent;
				base.gameObject.transform.localScale = originalLossyScale;
				internalState = newState;
				lockRotation();
				scriptControl(false);
				isMagnetic = false;
				if (IsDivingCollectible)
				{
					rigidBody.useGravity = false;
					rigidBody.drag = 5f;
				}
				else
				{
					rigidBody.useGravity = true;
					rigidBody.drag = 0f;
				}
				rigidBody.isKinematic = false;
				sphereColl.enabled = true;
				sphereColl.material = stickyPhysicMaterial;
				hasLanded = false;
				if (InteractionObj != null)
				{
					InteractionObj.SetActive(false);
				}
				break;
			case SceneryState.READY_FOR_PICKUP:
				internalState = newState;
				lockRotation();
				scriptControl(true);
				isMagnetic = false;
				rigidBody.drag = 0f;
				rigidBody.useGravity = false;
				rigidBody.isKinematic = false;
				sphereColl.enabled = true;
				sphereColl.material = stickyPhysicMaterial;
				hasLanded = true;
				if (animCurveScript != null)
				{
					animCurveScript.SetOriginalPosition();
				}
				if (InteractionObj != null)
				{
					InteractionObj.SetActive(false);
				}
				break;
			case SceneryState.MAGNETIC:
				internalState = newState;
				lockRotation();
				scriptControl(false, false, true);
				isMagnetic = true;
				rigidBody.drag = 0f;
				rigidBody.useGravity = false;
				rigidBody.isKinematic = true;
				sphereColl.enabled = false;
				if (IsDivingCollectible)
				{
					hasLanded = false;
				}
				if (InteractionObj != null)
				{
					InteractionObj.SetActive(false);
				}
				break;
			case SceneryState.INVISIBLE:
				base.gameObject.transform.parent = originalParent;
				base.gameObject.transform.localScale = originalLocalScale;
				internalState = newState;
				lockRotation();
				scriptControl(false);
				isMagnetic = false;
				rigidBody.drag = 0f;
				rigidBody.useGravity = false;
				rigidBody.isKinematic = true;
				sphereColl.enabled = false;
				sphereColl.material = stickyPhysicMaterial;
				setVisible(false);
				hasLanded = true;
				if (InteractionObj != null)
				{
					InteractionObj.SetActive(false);
				}
				break;
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			checkVelocity();
		}

		private void checkVelocity()
		{
			if (!hasLanded && !(rigidBody.velocity.magnitude > 0.2f))
			{
				changeState(SceneryState.READY_FOR_PICKUP);
				if (AutomaticPickup)
				{
					OnPickedUp();
				}
			}
		}

		public void OnPickedUp()
		{
			playSound();
			attractionStartTime = Time.time;
			attractionDistanceTotal = Vector3.Distance(base.gameObject.transform.position, playerObj.transform.position + TargetOffset);
			changeState(SceneryState.MAGNETIC);
		}

		private void addUpwardsForce()
		{
			changeState(SceneryState.SPAWN);
			Vector3 normalized;
			if (IsDivingCollectible)
			{
				normalized = (base.gameObject.transform.position - playerObj.transform.position).normalized;
				normalized.x *= Random.Range(RangeX.x, RangeX.y);
				normalized.y *= Random.Range(RangeY.x, RangeY.y);
				normalized.z = 0f;
			}
			else
			{
				normalized = (activeCamera.transform.position - base.gameObject.transform.position).normalized;
				normalized.x *= Random.Range(RangeX.x, RangeX.y);
				normalized.y = Random.Range(RangeY.x, RangeY.y);
				normalized.z *= Random.Range(RangeZ.x, RangeZ.y);
			}
			rigidBody.AddForce(normalized, ForceMode.Impulse);
		}

		private void magnetAttraction()
		{
			if (!(playerObj != null))
			{
				return;
			}
			float num = Vector3.Distance(base.gameObject.transform.position, playerObj.transform.position + TargetOffset);
			if (num <= PickupRadius)
			{
				if (internalState != SceneryState.INVISIBLE)
				{
					sendCollectedEventLocal();
					registerToRespawn();
					pickup();
				}
			}
			else
			{
				float num2 = (Time.time - attractionStartTime) * attractionSpeed;
				float t = num2 / attractionDistanceTotal;
				base.gameObject.transform.position = Vector3.Lerp(base.gameObject.transform.position, playerObj.transform.position + TargetOffset, t);
			}
		}

		private void pickup()
		{
			if (ParticlesPickup != null)
			{
				if (ParticlesPickupAnchor == null)
				{
					Object.Instantiate(ParticlesPickup, base.gameObject.transform.position, Quaternion.identity);
				}
				else
				{
					Object.Instantiate(ParticlesPickup, ParticlesPickupAnchor.transform.position, ParticlesPickupAnchor.transform.rotation);
				}
			}
			hide();
		}

		private void hide()
		{
			changeState(SceneryState.INVISIBLE);
			rigidBody.velocity = Vector3.zero;
			base.gameObject.transform.position = originalPos;
			setVisibleSpecialObjects(false);
		}

		private void respawn()
		{
			if (ParticlesAppear != null)
			{
				Object.Instantiate(ParticlesAppear, base.gameObject.transform.position, Quaternion.identity);
			}
			changeState(SceneryState.READY_TO_ACTIVATE);
		}

		private void scriptControl(bool isEnabled)
		{
			if (animCurveScript != null)
			{
				animCurveScript.enabled = isEnabled;
			}
			if (rotateScript != null)
			{
				rotateScript.enabled = isEnabled;
			}
			if (triggerScript != null)
			{
				triggerScript.enabled = isEnabled;
				triggerScript.IsCollectible(isEnabled);
			}
		}

		private void scriptControl(bool animEnabled, bool rotateEnabled, bool triggerEnabled)
		{
			if (animCurveScript != null)
			{
				animCurveScript.enabled = animEnabled;
			}
			if (rotateScript != null)
			{
				rotateScript.enabled = rotateEnabled;
			}
			if (triggerScript != null)
			{
				triggerScript.enabled = triggerEnabled;
				triggerScript.IsCollectible(triggerEnabled);
			}
		}

		private void lockRotation()
		{
			Quaternion rotation = base.gameObject.transform.rotation;
			rotation.x = 0f;
			rotation.z = 0f;
			base.gameObject.transform.rotation = rotation;
		}

		private void setVisible(bool isVisible)
		{
			meshRend.enabled = isVisible;
			IsVisible = isVisible;
			if (partSys != null)
			{
				if (isVisible)
				{
					partSys.Play();
					return;
				}
				partSys.Stop();
				partSys.Clear();
			}
		}

		private void setVisibleSpecialObjects(bool isVisible)
		{
			int num = ObjectsToHide.Length;
			for (int i = 0; i < num; i++)
			{
				ObjectsToHide[i].SetActive(isVisible);
			}
		}

		private void playSound()
		{
			if (!string.IsNullOrEmpty(AudioEvent))
			{
				EventManager.Instance.PostEvent(AudioEvent, EventAction.PlaySound);
			}
		}

		public override void RespawnCollectible()
		{
			respawn();
		}
	}
}
