using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.SceneManipulation
{
	public class EditModeModifier : ISceneModifier
	{
		private bool structuresEnabled;

		private SceneManipulationService sceneManipulationService;

		private Dictionary<int, StructureDefinition> structureDefinitions;

		private MultiPointLineAttractorLocator _attractorLocator;

		private List<MultiPointLineAttractor> attractors = new List<MultiPointLineAttractor>();

		private MultiPointLineAttractorLocator attractorLocator
		{
			get
			{
				if (_attractorLocator == null)
				{
					_attractorLocator = Object.FindObjectOfType<MultiPointLineAttractorLocator>();
				}
				return _attractorLocator;
			}
		}

		public EditModeModifier(SceneManipulationService sceneManipulationService)
		{
			this.sceneManipulationService = sceneManipulationService;
			structureDefinitions = Service.Get<IGameData>().Get<Dictionary<int, StructureDefinition>>();
		}

		public void EnabledStructures()
		{
			if (!structuresEnabled)
			{
				structuresEnabled = true;
				sceneManipulationService.ProcessLayout();
			}
		}

		public void DisableStructures()
		{
			if (structuresEnabled)
			{
				structuresEnabled = false;
				sceneManipulationService.ProcessLayout();
			}
		}

		private void onAttracted(MultiPointLineAttractor attractor, Collider collider, ObjectManipulator other, Quaternion rotation)
		{
			other.BaseLocationIsValid = true;
			other.SetPosition(collider.transform.position);
			other.SetRotation(rotation);
			attractor.OnUnSnapped += onUnSnap;
		}

		private void onUnSnap(MultiPointLineAttractor attractor, ObjectManipulator obj)
		{
			attractor.OnUnSnapped -= onUnSnap;
			obj.BaseLocationIsValid = false;
		}

		public void ObjectAdded(DecorationLayoutData data, GameObject go)
		{
			if (data.Type == DecorationLayoutData.DefinitionType.Structure)
			{
				ManipulatableStructure manipulatableStructure = go.AddComponent<ManipulatableStructure>();
				int sizeUnits = 0;
				StructureDefinition value;
				if (structureDefinitions.TryGetValue(data.DefinitionId, out value))
				{
					sizeUnits = value.SizeUnits;
				}
				manipulatableStructure.SizeUnits = sizeUnits;
			}
		}

		public void ObjectRemoved(DecorationLayoutData data, GameObject go)
		{
			ManipulatableStructure component = go.GetComponent<ManipulatableStructure>();
			if ((bool)component)
			{
				Object.Destroy(component);
			}
		}

		public void ProcessObject(GameObject go)
		{
			ManipulatableObjectEffects component = go.GetComponent<ManipulatableObjectEffects>();
			if (component != null && go.GetComponent<ManipulatableStructure>() == null)
			{
				component.SetSelectable(!structuresEnabled);
			}
			PartneredObject component2 = go.GetComponent<PartneredObject>();
			if (component2 != null)
			{
				sceneManipulationService.numberTracker.RegisterNumber(component2.Number);
			}
		}

		public void OnLayoutProcessed()
		{
			removeAttractorEvents();
			attractors.Clear();
			for (int i = 0; i < attractorLocator.AttractorContainers.Length; i++)
			{
				GameObject gameObject = attractorLocator.AttractorContainers[i];
				if (gameObject != null)
				{
					gameObject.SetActive(structuresEnabled);
					if (structuresEnabled)
					{
						attractors.AddRange(gameObject.GetComponentsInChildren<MultiPointLineAttractor>());
					}
				}
			}
		}

		public bool CanObjectBeSelected(ManipulatableObject obj)
		{
			bool flag = obj.GetComponent<ManipulatableStructure>() != null;
			if (structuresEnabled != flag)
			{
				return false;
			}
			return true;
		}

		public void AfterObjectSelected(ManipulatableObject obj, bool isNewObject)
		{
			if (obj.GetComponent<ManipulatableStructure>() != null)
			{
				ObjectManipulator component = obj.GetComponent<ObjectManipulator>();
				ManipulatableStructure component2 = obj.GetComponent<ManipulatableStructure>();
				component.BaseLocationIsValid = !isNewObject;
				for (int i = 0; i < attractors.Count; i++)
				{
					attractors[i].SetTrackedObject(component, component2.SizeUnits, sceneManipulationService.ObjectManipulationInputController);
					attractors[i].OnAttracted += onAttracted;
				}
			}
		}

		public void AfterObjectDeselected(ObjectManipulator obj)
		{
			if (obj.GetComponent<ManipulatableStructure>() != null)
			{
				removeAttractorEvents();
			}
		}

		public void Destroy()
		{
			removeAttractorEvents();
		}

		private void removeAttractorEvents()
		{
			for (int i = 0; i < attractors.Count; i++)
			{
				if (attractors[i] != null)
				{
					attractors[i].OnAttracted -= onAttracted;
					attractors[i].ClearTrackedObject();
				}
			}
		}
	}
}
