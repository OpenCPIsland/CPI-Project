using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation
{
	public class CollidableObject : MonoBehaviour
	{
		public CollisionRuleSetDefinitionKey CollisionRuleSet;

		private Bounds myBounds;

		private Vector3 localBoundsSize;

		private Vector3 knownPosition;

		private Vector3 knownScale;

		private Quaternion knownRotation;

		[SerializeField]
		private bool isSquashed;

		[SerializeField]
		private Collider[] myColliders;

		private CollidableObjectCollisionOverrides collisionOverrides;

		public bool IsSquashed
		{
			get
			{
				return isSquashed;
			}
			set
			{
				if (value != isSquashed)
				{
					isSquashed = value;
					ManipulatableObject componentInParent = GetComponentInParent<ManipulatableObject>();
					if (componentInParent != null)
					{
						componentInParent.CheckIfSquashedChanged();
					}
				}
			}
		}

		public Collider[] MyColliders
		{
			get
			{
				return myColliders;
			}
		}

		private void Awake()
		{
			ReloadColliders();
			collisionOverrides = GetComponent<CollidableObjectCollisionOverrides>();
			if (collisionOverrides != null)
			{
				collisionOverrides.DefaultColliders = myColliders;
			}
		}

		private void OnEnable()
		{
			if (collisionOverrides != null)
			{
				collisionOverrides.SetEditting();
			}
		}

		private void OnDisable()
		{
			if (collisionOverrides != null)
			{
				collisionOverrides.SetDefault();
			}
		}

		public void ReloadColliders()
		{
			Transform transform = base.transform.Find("Colliders");
			if (transform != null)
			{
				myColliders = transform.GetComponentsInChildren<Collider>();
			}
			else
			{
				myColliders = new Collider[0];
			}
		}

		public Vector3 GetLocalBoundsSize()
		{
			calculateBounds();
			return localBoundsSize;
		}

		public Bounds GetBounds()
		{
			calculateBounds();
			return myBounds;
		}

		private void calculateBounds()
		{
			if (myBounds == default(Bounds) || localBoundsSize == default(Vector3) || knownPosition != base.transform.position || base.transform.localScale != knownScale || knownRotation != base.transform.rotation)
			{
				localBoundsSize = Vector3.zero;
				myBounds = new Bounds(base.transform.position, Vector3.zero);
				Collider[] array = myColliders;
				foreach (Collider collider in array)
				{
					myBounds.Encapsulate(collider.bounds);
					Vector3 position = colliderLocalSize(collider);
					Vector3 position2 = collider.transform.TransformPoint(position);
					Vector3 rhs = base.transform.InverseTransformPoint(position2);
					localBoundsSize = Vector3.Max(localBoundsSize, rhs);
				}
				knownPosition = base.transform.position;
				knownScale = base.transform.localScale;
				knownRotation = base.transform.rotation;
			}
		}

		private Vector3 colliderLocalSize(Collider collider)
		{
			if (collider is BoxCollider)
			{
				return ((BoxCollider)collider).size;
			}
			if (collider is SphereCollider)
			{
				return 2f * ((SphereCollider)collider).radius * Vector3.one;
			}
			if (collider is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
				Vector3 result = 2f * capsuleCollider.radius * Vector3.one;
				result[capsuleCollider.direction] = Mathf.Max(capsuleCollider.height, result[capsuleCollider.direction]);
				return result;
			}
			if (collider is MeshCollider)
			{
				return ((MeshCollider)collider).sharedMesh.bounds.size;
			}
			return Vector3.zero;
		}

		public void EnableTriggers(bool isTrigger = true)
		{
			for (int i = 0; i < myColliders.Length; i++)
			{
				MeshCollider meshCollider = myColliders[i] as MeshCollider;
				if (meshCollider != null && !meshCollider.convex)
				{
					meshCollider.convex = true;
					Log.LogErrorFormatted(this, "Found concave mesh collider on {0} but forcing it to convex. Please fix asset", base.gameObject.name);
				}
				myColliders[i].isTrigger = isTrigger;
			}
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Bounds bounds = GetBounds();
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}
}
