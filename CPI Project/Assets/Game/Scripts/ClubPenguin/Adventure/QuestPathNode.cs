using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestPathNode : MonoBehaviour
	{
		public GameObject ParticleEffectPrefabOverride;

		private float colliderRadius = 1f;

		private bool isNodeActive = false;

		private GameObject attachedParticles;

		private ParticleSystem[] particleSystems;

		private QuestPathNode nextNode;

		private float distanceToNextNode;

		private bool usingColliderOverride = false;

		private Collider pathCollider;

		private int nodeIndex = -1;

		private bool showInEditor = false;

		public float ColliderRadius
		{
			get
			{
				return colliderRadius;
			}
			set
			{
				if (pathCollider != null && !usingColliderOverride)
				{
					((SphereCollider)pathCollider).radius = value;
				}
				colliderRadius = value;
			}
		}

		public bool IsNodeActive
		{
			get
			{
				return isNodeActive;
			}
			set
			{
				isNodeActive = value;
				if (pathCollider != null)
				{
					pathCollider.enabled = value;
				}
			}
		}

		public QuestPathNode NextNode
		{
			get
			{
				return nextNode;
			}
			set
			{
				nextNode = value;
				Vector3 forward = nextNode.transform.position - base.transform.position;
				distanceToNextNode = forward.magnitude;
				base.transform.rotation = Quaternion.LookRotation(forward);
			}
		}

		public float DistanceToNextNode
		{
			get
			{
				return distanceToNextNode;
			}
		}

		public int NodeIndex
		{
			get
			{
				return nodeIndex;
			}
			set
			{
				nodeIndex = value;
			}
		}

		public bool ShowInEditor
		{
			get
			{
				return showInEditor;
			}
			set
			{
				showInEditor = value;
			}
		}

		public void Awake()
		{
			pathCollider = GetComponent<Collider>();
			if (pathCollider == null)
			{
				pathCollider = base.gameObject.AddComponent<SphereCollider>();
				((SphereCollider)pathCollider).radius = ColliderRadius;
				pathCollider.gameObject.layer = LayerMask.NameToLayer("LocalPlayerInteractibles");
			}
			else
			{
				usingColliderOverride = true;
			}
			pathCollider.isTrigger = true;
			pathCollider.enabled = false;
		}

		public void Update()
		{
			if (!(attachedParticles != null))
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < particleSystems.Length; i++)
			{
				if (particleSystems[i].IsAlive())
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Object.Destroy(attachedParticles);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.CompareTag("Player"))
			{
				base.transform.parent.GetComponent<QuestPath>().OnNodeTriggered(nodeIndex);
			}
		}

		public void EnableParticles(GameObject particleSystemPrefab)
		{
			if ((!(ParticleEffectPrefabOverride != null) && !(particleSystemPrefab != null)) || !(attachedParticles == null) || !(particleSystemPrefab != null) || !(nextNode != null))
			{
				return;
			}
			if (ParticleEffectPrefabOverride != null)
			{
				attachedParticles = Object.Instantiate(ParticleEffectPrefabOverride);
			}
			else
			{
				attachedParticles = Object.Instantiate(particleSystemPrefab);
			}
			attachedParticles.transform.SetParent(base.transform);
			attachedParticles.transform.localPosition = Vector3.zero;
			attachedParticles.transform.localRotation = Quaternion.identity;
			particleSystems = attachedParticles.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem particleSystem = particleSystems[i];
				if (particleSystem.transform.localRotation.eulerAngles.x == 0f)
				{
					particleSystem.SetStartLifeTimeConstant(distanceToNextNode / particleSystem.main.startSpeed.constant * 1.1f);
					continue;
				}
				particleSystem.transform.localScale = new Vector3(particleSystem.transform.localScale.x, distanceToNextNode * 0.37f, particleSystem.transform.localScale.z);
				particleSystem.transform.localPosition = new Vector3(particleSystem.transform.localPosition.x, particleSystem.transform.localPosition.y, distanceToNextNode * 0.185f);
			}
		}

		public void DisableParticles()
		{
			if (attachedParticles != null)
			{
				for (int i = 0; i < particleSystems.Length; i++)
				{
					particleSystems[i].Stop();
				}
			}
		}

		public void OnDrawGizmos()
		{
		}
	}
}
