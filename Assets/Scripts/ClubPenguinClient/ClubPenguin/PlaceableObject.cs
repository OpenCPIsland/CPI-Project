using UnityEngine;

namespace ClubPenguin
{
	public class PlaceableObject : MonoBehaviour
	{
		public enum SelectedState
		{
			NotSelected,
			Selected
		}

		private static int nextInstanceNumber = 0;

		public ParticleSystem ParticlesPrefab;

		public Collider MyCollider;

		public GameObject RenderableGameObject;

		public string FurnitureTemplateID;

		public ParticleSystem ParticlesPrefabDropFurniture;

		public PlaceableObject Next = null;

		public int PieceNumber = 1;

		private Shader oldShader = null;

		public GameObject partner;

		public bool isVisibleOnDeselect = true;

		public bool isVisibleInWorldMode = true;

		private Shader shader = null;

		private ParticleSystem particlesSelectedFurniture;

		public Shader SelectedObjectShader
		{
			get
			{
				if (shader == null)
				{
					shader = Shader.Find("DiffuseSpecFresnel_1Light_Vert");
				}
				return shader;
			}
			set
			{
				shader = value;
			}
		}

		public SelectedState State
		{
			get;
			private set;
		}

		public int InstanceNumber
		{
			get;
			private set;
		}

		private void Awake()
		{
			InstanceNumber = ++nextInstanceNumber;
		}

		private void EnableSelectionParticles()
		{
			if (ParticlesPrefab != null)
			{
				if (particlesSelectedFurniture == null)
				{
					particlesSelectedFurniture = Object.Instantiate(ParticlesPrefab);
				}
				if (particlesSelectedFurniture != null)
				{
					particlesSelectedFurniture.transform.position = base.transform.rotation * particlesSelectedFurniture.transform.position + base.transform.position;
					particlesSelectedFurniture.transform.rotation *= base.transform.rotation;
					particlesSelectedFurniture.transform.parent = RenderableGameObject.transform;
				}
			}
		}

		public void select()
		{
			if (State == SelectedState.NotSelected)
			{
				State = SelectedState.Selected;
				base.gameObject.SetActive(true);
				Renderer componentInChildren = GetComponentInChildren<Renderer>();
				if (componentInChildren != null && SelectedObjectShader != null)
				{
					oldShader = componentInChildren.material.shader;
					componentInChildren.material.shader = SelectedObjectShader;
				}
				EnableSelectionParticles();
				SelectPartner();
			}
		}

		private void SelectPartner()
		{
			if (partner != null)
			{
				PlaceableObject component = partner.GetComponent<PlaceableObject>();
				if (component != null)
				{
					component.select();
				}
			}
		}

		public void deselect()
		{
			if (State != 0)
			{
				State = SelectedState.NotSelected;
				Renderer componentInChildren = GetComponentInChildren<Renderer>();
				if (componentInChildren != null && oldShader != null)
				{
					componentInChildren.material.shader = oldShader;
					oldShader = null;
					base.gameObject.SetActive(isVisibleOnDeselect);
				}
				DestroyParticles();
				DeselectPartner();
			}
		}

		private void DestroyParticles()
		{
			if (particlesSelectedFurniture != null)
			{
				particlesSelectedFurniture.transform.parent = null;
				Object.Destroy(particlesSelectedFurniture.gameObject);
				Object.Destroy(particlesSelectedFurniture);
				particlesSelectedFurniture = null;
			}
		}

		private void DeselectPartner()
		{
			if (partner != null)
			{
				PlaceableObject component = partner.GetComponent<PlaceableObject>();
				if (component != null)
				{
					component.deselect();
				}
			}
		}

		public void PlayDropFurnitureParticles()
		{
			if (ParticlesPrefabDropFurniture != null)
			{
				ParticleSystem particleSystem = Object.Instantiate(ParticlesPrefabDropFurniture, RenderableGameObject.transform.position, Quaternion.identity);
				if (particleSystem != null)
				{
					particleSystem.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 0.2f, base.transform.position.z);
					particleSystem.transform.parent = RenderableGameObject.transform;
					Object.Destroy(particleSystem.gameObject, particleSystem.main.duration);
				}
			}
		}

		public void ghost()
		{
			MyCollider.enabled = false;
		}

		public void unghost()
		{
			MyCollider.enabled = true;
		}

		public override string ToString()
		{
			return string.Concat("PlaceableObject ", InstanceNumber, ": ", State, " Template ID ", FurnitureTemplateID);
		}
	}
}
