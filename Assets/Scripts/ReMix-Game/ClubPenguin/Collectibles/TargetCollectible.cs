using UnityEngine;

namespace ClubPenguin.Collectibles
{
	public class TargetCollectible : MonoBehaviour
	{
		public float AttractForce = 150f;

		public float PickupRadius = 0.5f;

		public GameObject ParticlesAppear;

		public GameObject ParticlesPickup;

		public bool AutomaticPickup = false;

		private MeshRenderer meshRend;

		private ParticleSystem partSys;

		private RotateTransform rotateScript;

		private MovementAnimationCurve animCurveScript;

		private Rigidbody rigidBody;

		private SphereCollider sphereColl;

		private PhysicMaterial slipperyPhysicMaterial;

		private PhysicMaterial stickyPhysicMaterial;

		private TargetCollectibleTrigger triggerScript;

		private GameObject playerObj = null;

		private float increasingForce = 0f;

		private bool hasLanded = false;

		private bool isPickedUp = false;

		private Camera activeCamera;

		private void Awake()
		{
			partSys = base.gameObject.GetComponentInChildren<ParticleSystem>();
			rotateScript = base.gameObject.GetComponentInChildren<RotateTransform>();
			animCurveScript = base.gameObject.GetComponent<MovementAnimationCurve>();
			rigidBody = base.gameObject.GetComponent<Rigidbody>();
			triggerScript = base.gameObject.GetComponentInChildren<TargetCollectibleTrigger>();
			triggerScript.Init();
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
			float num = base.gameObject.transform.position.x / 1.5f + base.gameObject.transform.position.y / 1.5f + base.gameObject.transform.position.z / 1.5f;
			animCurveScript.startPosition = Mathf.Abs(num - Mathf.Round(num));
			rigidBody.angularDrag = float.PositiveInfinity;
			ChangeState(SceneryState.READY_TO_ACTIVATE);
		}

		private void Start()
		{
			playerObj = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (Camera.main != null)
			{
				activeCamera = Camera.main;
			}
		}

		public void OnDestroy()
		{
			Object.Destroy(slipperyPhysicMaterial);
			Object.Destroy(stickyPhysicMaterial);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(base.gameObject.transform.position, PickupRadius);
		}

		private void Update()
		{
			if (isPickedUp)
			{
				LockRotation();
				MagnetAttraction();
			}
		}

		public void OnAwarded()
		{
			AddUpwardsForce();
		}

		private void ChangeState(SceneryState newState)
		{
			switch (newState)
			{
			case SceneryState.NONE:
				break;
			case SceneryState.READY_TO_ACTIVATE:
				ScriptControl(false);
				isPickedUp = false;
				rigidBody.useGravity = false;
				rigidBody.isKinematic = true;
				sphereColl.enabled = false;
				sphereColl.material = stickyPhysicMaterial;
				hasLanded = false;
				SetVisible(false);
				break;
			case SceneryState.SPAWN:
				LockRotation();
				ScriptControl(false);
				isPickedUp = false;
				rigidBody.useGravity = true;
				rigidBody.isKinematic = false;
				sphereColl.enabled = true;
				sphereColl.material = stickyPhysicMaterial;
				SetVisible(true);
				hasLanded = false;
				break;
			case SceneryState.READY_FOR_PICKUP:
				LockRotation();
				ScriptControl(true);
				isPickedUp = false;
				rigidBody.useGravity = false;
				rigidBody.isKinematic = false;
				sphereColl.enabled = true;
				sphereColl.material = stickyPhysicMaterial;
				hasLanded = true;
				if (animCurveScript != null)
				{
					animCurveScript.SetOriginalPosition();
				}
				break;
			case SceneryState.MAGNETIC:
				LockRotation();
				ScriptControl(true);
				isPickedUp = true;
				rigidBody.useGravity = false;
				rigidBody.isKinematic = false;
				sphereColl.enabled = false;
				sphereColl.material = slipperyPhysicMaterial;
				break;
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if (!hasLanded && !(rigidBody.velocity.magnitude > 0.2f))
			{
				ChangeState(SceneryState.READY_FOR_PICKUP);
				if (AutomaticPickup)
				{
					OnPickedUp();
				}
			}
		}

		public void OnPickedUp()
		{
			increasingForce = AttractForce;
			ChangeState(SceneryState.MAGNETIC);
		}

		private void AddUpwardsForce()
		{
			ChangeState(SceneryState.SPAWN);
			Vector3 normalized = (activeCamera.transform.position - base.gameObject.transform.position).normalized;
			normalized.x *= Random.Range(-3f, 3f);
			normalized.y = Random.Range(6f, 10f);
			normalized.z *= Random.Range(-3f, 3f);
			rigidBody.AddForce(normalized, ForceMode.Impulse);
		}

		private void MagnetAttraction()
		{
			if (!(playerObj != null))
			{
				return;
			}
			float num = Vector3.Distance(base.gameObject.transform.position, playerObj.transform.position);
			if (num <= PickupRadius)
			{
				DestroyCollectible();
				return;
			}
			Vector3 force = (playerObj.transform.position - base.gameObject.transform.position).normalized * increasingForce;
			rigidBody.AddForce(force, ForceMode.Force);
			if (increasingForce > -1000f && increasingForce < 1000f)
			{
				increasingForce *= 1.1f;
			}
		}

		private void DestroyCollectible()
		{
			if (ParticlesPickup != null)
			{
				Object.Instantiate(ParticlesPickup, base.gameObject.transform.position, Quaternion.identity);
			}
			Object.Destroy(base.gameObject);
		}

		private void ScriptControl(bool isEnabled)
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

		private void LockRotation()
		{
			Quaternion rotation = base.gameObject.transform.rotation;
			rotation.x = 0f;
			rotation.z = 0f;
			base.gameObject.transform.rotation = rotation;
		}

		private void SetVisible(bool isVisible)
		{
			meshRend.enabled = isVisible;
			if (isVisible)
			{
				partSys.Play();
				return;
			}
			partSys.Stop();
			partSys.Clear();
		}
	}
}
