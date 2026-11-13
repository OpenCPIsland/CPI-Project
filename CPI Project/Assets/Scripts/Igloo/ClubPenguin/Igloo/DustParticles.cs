#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using ClubPenguin.ObjectManipulation;
using ClubPenguin.ObjectManipulation.Input;
using ClubPenguin.SceneManipulation;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.Igloo
{
	public class DustParticles : MonoBehaviour
	{
		[Header("Four particle systems for +/-Z and +/- X directions.")]
		public ParticleSystem[] Systems = new ParticleSystem[4];

		[Header("Simple multiplier to increase/decrease playback speed")]
		public float PlaybackSpeed = 1.5f;

		[Tooltip("Particles used when selected object is deleted")]
		public GameObject DestroyDecorationParticlePrefab;

		private SceneManipulationService sceneManipulationService;

		private ObjectManipulationInputController _objectManipulationInputController;

		private InteractionState previousState = InteractionState.DisabledInput;

		private ObjectManipulationInputController objectManipulationInputController
		{
			get
			{
				if (_objectManipulationInputController == null && SceneRefs.IsSet<ObjectManipulationInputController>())
				{
					_objectManipulationInputController = SceneRefs.Get<ObjectManipulationInputController>();
				}
				return _objectManipulationInputController;
			}
		}

		public void Awake()
		{
			Assert.IsNotNull(Systems, "Dust particle systems are not set up properly.");
			Assert.AreEqual(4, Systems.Length, "Dust particle systems are not set up properly.");
			for (int i = 0; i < Systems.Length; i++)
			{
				Assert.IsTrue(Systems[0], "Dust particle systems are not set up properly.");
				ParticleSystem.MainModule main = Systems[i].main;
				main.simulationSpeed = PlaybackSpeed;
			}
		}

		public void Start()
		{
			if (objectManipulationInputController != null)
			{
				objectManipulationInputController.InteractionStateChanged += OnObjectManipulationInputControllerInteractionStateChanged;
			}
			if (SceneRefs.IsSet<SceneManipulationService>())
			{
				sceneManipulationService = SceneRefs.Get<SceneManipulationService>();
				sceneManipulationService.ObjectRemoved += onSceneManipulationServiceObjectRemoved;
			}
			Service.Get<EventDispatcher>().AddListener<ObjectManipulationEvents.ConfirmPlacementSelectedItemEvent>(OnConfirmPlacementSelectedItemEvent, EventDispatcher.Priority.FIRST);
		}

		public void OnDestroy()
		{
			if (objectManipulationInputController != null)
			{
				objectManipulationInputController.InteractionStateChanged -= OnObjectManipulationInputControllerInteractionStateChanged;
			}
			if (SceneRefs.IsSet<SceneManipulationService>())
			{
				sceneManipulationService = SceneRefs.Get<SceneManipulationService>();
				sceneManipulationService.ObjectRemoved -= onSceneManipulationServiceObjectRemoved;
			}
			Service.Get<EventDispatcher>().RemoveListener<ObjectManipulationEvents.ConfirmPlacementSelectedItemEvent>(OnConfirmPlacementSelectedItemEvent);
		}

		private void OnObjectManipulationInputControllerInteractionStateChanged(InteractionState state)
		{
			if (state == InteractionState.ActiveSelectedItem && previousState == InteractionState.DragItem)
			{
				ShowParticlesOnSelectedObject();
			}
			previousState = state;
		}

		private void ShowParticlesOnSelectedObject()
		{
			ObjectManipulator currentObjectManipulator = objectManipulationInputController.CurrentObjectManipulator;
			if (currentObjectManipulator != null)
			{
				CollidableObject component = currentObjectManipulator.GetComponent<CollidableObject>();
				if (component != null)
				{
					Bounds bounds = component.GetBounds();
					SetExtents(bounds.extents);
					base.transform.position = bounds.center - new Vector3(0f, bounds.extents.y, 0f);
					base.transform.rotation = Quaternion.identity;
					TriggerParticles();
				}
			}
		}

		private void onSceneManipulationServiceObjectRemoved(ManipulatableObject obj)
		{
			if (obj != null)
			{
				Object.Instantiate(DestroyDecorationParticlePrefab, obj.transform.position, Quaternion.identity);
			}
		}

		private bool OnConfirmPlacementSelectedItemEvent(ObjectManipulationEvents.ConfirmPlacementSelectedItemEvent evt)
		{
			ShowParticlesOnSelectedObject();
			return false;
		}

		private void TriggerParticles()
		{
			for (int i = 0; i < 4; i++)
			{
				Systems[i].Play(false);
			}
		}

		private void SetTransformFromCollider(Collider collider)
		{
			Bounds bounds = collider.bounds;
			Vector3 extents = bounds.extents;
			base.transform.position = bounds.center - new Vector3(0f, extents.y, 0f);
			base.transform.rotation = Quaternion.identity;
			SetExtents(extents);
		}

		private void SetTransformFromCollider(BoxCollider collider)
		{
			Vector3 extents = collider.size * 0.5f;
			base.transform.position = collider.transform.TransformPoint(collider.center) - collider.transform.TransformDirection(new Vector3(0f, extents.y, 0f));
			base.transform.rotation = collider.transform.rotation;
			SetExtents(extents);
		}

		private void SetExtents(Vector3 extents)
		{
			Systems[0].transform.localPosition = new Vector3(0f, 0f, 0f - extents.z);
			Systems[1].transform.localPosition = new Vector3(0f, 0f, extents.z);
			Systems[2].transform.localPosition = new Vector3(extents.x, 0f, 0f);
			Systems[3].transform.localPosition = new Vector3(0f - extents.x, 0f, 0f);
			for (int i = 0; i < 4; i++)
			{
				ParticleSystem.ShapeModule shape = Systems[i].shape;
				float num2 = shape.radius = ((i < 2) ? extents.x : extents.z);
			}
		}
	}
}
