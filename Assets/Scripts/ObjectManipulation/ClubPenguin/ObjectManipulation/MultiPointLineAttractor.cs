using ClubPenguin.ObjectManipulation.Input;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation
{
	public class MultiPointLineAttractor : MonoBehaviour
	{
		private class Attractor : MonoBehaviour
		{
		}

		public int Segments;

		public Vector3 AttractorSize;

		public Vector3 AttractorCenter;

		[SerializeField]
		private ObjectManipulator trackedObject;

		[SerializeField]
		private int trackedObjectSize;

		private bool snapped;

		private ObjectManipulationInputController inputController;

		private Collider[] oddAttractionPoints;

		private Collider[] evenAttractionPoints;

		private HashSet<Attractor> attractors;

		public event Action<MultiPointLineAttractor, Collider, ObjectManipulator, Quaternion> OnAttracted;

		public event Action<MultiPointLineAttractor, ObjectManipulator> OnUnSnapped;

		private void Awake()
		{
			attractors = new HashSet<Attractor>();
			Vector3 vector = new Vector3(1f / (float)Segments, 0f, 0f);
			oddAttractionPoints = new Collider[Segments];
			Vector3 position = vector * (0.5f - (float)Segments / 2f);
			for (int i = 0; i < Segments; i++)
			{
				oddAttractionPoints[i] = initAttractionPoint(position);
				position += vector;
			}
			evenAttractionPoints = new Collider[Segments - 1];
			position = vector * (1f - (float)Segments / 2f);
			for (int i = 0; i < Segments - 1; i++)
			{
				evenAttractionPoints[i] = initAttractionPoint(position);
				position += vector;
			}
		}

		private void OnDestroy()
		{
			this.OnAttracted = null;
			this.OnUnSnapped = null;
			foreach (Attractor attractor in attractors)
			{
				UnityEngine.Object.Destroy(attractor.gameObject);
			}
		}

		public void SetTrackedObject(ObjectManipulator obj, int size, ObjectManipulationInputController inputController)
		{
			trackedObject = obj;
			trackedObjectSize = size;
			if (base.isActiveAndEnabled)
			{
				enabledAttractionPoints();
			}
			this.inputController = inputController;
			inputController.BeforeDragPosition += checkForSnapPosition;
		}

		public void ClearTrackedObject()
		{
			if (attractors != null)
			{
				trackedObject = null;
				trackedObjectSize = 0;
				disableColliders(oddAttractionPoints);
				disableColliders(evenAttractionPoints);
			}
			if (inputController != null)
			{
				inputController.BeforeDragPosition -= checkForSnapPosition;
			}
		}

		private Collider initAttractionPoint(Vector3 position)
		{
			GameObject gameObject = new GameObject("AttractionPoint " + attractors.Count, typeof(BoxCollider), typeof(Attractor));
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = position;
			gameObject.layer = base.gameObject.layer;
			gameObject.transform.SetParent(base.transform.parent);
			gameObject.transform.localScale = Vector3.one;
			BoxCollider component = gameObject.GetComponent<BoxCollider>();
			component.size = AttractorSize;
			component.center = AttractorCenter;
			component.isTrigger = true;
			component.enabled = false;
			Attractor component2 = gameObject.GetComponent<Attractor>();
			attractors.Add(component2);
			return component;
		}

		private void checkForSnapPosition(Vector2 screenPosition, ObjectManipulationInputController.Cancelable cancelable)
		{
			Ray ray = Camera.main.ScreenPointToRay(screenPosition);
			RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, LayerMask.GetMask(LayerMask.LayerToName(base.gameObject.layer)), QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				Attractor component = array[i].transform.GetComponent<Attractor>();
				if (!(component != null) || !attractors.Contains(component))
				{
					continue;
				}
				cancelable.Cancel();
				if (!snapped)
				{
					if (this.OnAttracted != null)
					{
						this.OnAttracted(this, component.GetComponent<Collider>(), trackedObject, base.transform.rotation);
					}
					snapped = true;
				}
				return;
			}
			if (snapped)
			{
				snapped = false;
				this.OnUnSnapped.InvokeSafe(this, trackedObject);
			}
		}

		private void OnEnable()
		{
			if (trackedObject != null)
			{
				enabledAttractionPoints();
			}
			Service.Get<ObjectManipulationService>().StructurePlotManager.RegisterStructurePlot(this);
		}

		private void OnDisable()
		{
			disableColliders(oddAttractionPoints);
			disableColliders(evenAttractionPoints);
			Service.Get<ObjectManipulationService>().StructurePlotManager.RemoveStructurePlot(this);
		}

		private void disableColliders(Collider[] colliders)
		{
			int num = colliders.Length;
			for (int i = 0; i < num; i++)
			{
				colliders[i].enabled = false;
			}
		}

		private void enabledAttractionPoints()
		{
			int num;
			Collider[] array;
			Collider[] colliders;
			if (trackedObjectSize % 2 == 0)
			{
				num = trackedObjectSize / 2 - 1;
				array = evenAttractionPoints;
				colliders = oddAttractionPoints;
			}
			else
			{
				num = (trackedObjectSize - 1) / 2;
				array = oddAttractionPoints;
				colliders = evenAttractionPoints;
			}
			disableColliders(colliders);
			int num2 = array.Length;
			int num3 = num;
			int num4 = num2 - num;
			for (int i = 0; i < num2; i++)
			{
				array[i].enabled = (i >= num3 && i < num4);
			}
		}

		private void OnValidate()
		{
		}
	}
}
