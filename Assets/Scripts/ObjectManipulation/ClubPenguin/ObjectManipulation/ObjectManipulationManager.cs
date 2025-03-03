using ClubPenguin.Core;
using ClubPenguin.ObjectManipulation.Input;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation
{
	[RequireComponent(typeof(ObjectManipulationInputController))]
	public class ObjectManipulationManager : MonoBehaviour
	{
		private ObjectManipulationInputController objectManipulationInputController;

		public void Awake()
		{
			objectManipulationInputController = GetComponent<ObjectManipulationInputController>();
			objectManipulationInputController.ObjectDeselected += onObjectManipulationInputControllerObjectDeselected;
			ObjectManipulationInputController obj = objectManipulationInputController;
			obj.BeforeDragComplete = (Action<ObjectManipulator, Action<bool>>)Delegate.Combine(obj.BeforeDragComplete, new Action<ObjectManipulator, Action<bool>>(onConfirmSquashedObjectBeforeDragComplete));
		}

		public void OnDestroy()
		{
			objectManipulationInputController.ObjectDeselected -= onObjectManipulationInputControllerObjectDeselected;
			ObjectManipulationInputController obj = objectManipulationInputController;
			obj.BeforeDragComplete = (Action<ObjectManipulator, Action<bool>>)Delegate.Remove(obj.BeforeDragComplete, new Action<ObjectManipulator, Action<bool>>(onConfirmSquashedObjectBeforeDragComplete));
		}

		private void OnSelectedObjectManipulatorTriggerEnter(Collider otherCollider)
		{
			ObjectManipulator currentObjectManipulator = objectManipulationInputController.CurrentObjectManipulator;
			CollidableObject componentInParent = otherCollider.gameObject.GetComponentInParent<CollidableObject>();
			if (componentInParent != null && currentObjectManipulator != null)
			{
				switch (objectManipulationInputController.GetCollisionRule(componentInParent))
				{
				case CollisionRuleResult.Stack:
				case CollisionRuleResult.StackXNormal:
					break;
				case CollisionRuleResult.Intersect:
					break;
				case CollisionRuleResult.NotAllowed:
					currentObjectManipulator.CollisionIsValid = false;
					break;
				case CollisionRuleResult.Squash:
					componentInParent.IsSquashed = true;
					break;
				}
			}
		}

		private void OnSelectedObjectManipulatorTriggerExit(Collider otherCollider)
		{
			ObjectManipulator currentObjectManipulator = objectManipulationInputController.CurrentObjectManipulator;
			if (!(currentObjectManipulator != null))
			{
				return;
			}
			currentObjectManipulator.CollisionIsValid = objectManipulationInputController.IsSelectedObjectAllowedInCurrentPosition();
			CollidableObject componentInParent = otherCollider.gameObject.GetComponentInParent<CollidableObject>();
			if (componentInParent != null)
			{
				CollisionRuleResult collisionRule = objectManipulationInputController.GetCollisionRule(componentInParent);
				if (collisionRule == CollisionRuleResult.Squash)
				{
					componentInParent.IsSquashed = false;
				}
			}
		}

		private void onConfirmSquashedObjectBeforeDragComplete(ObjectManipulator selected, Action<bool> callback)
		{
			if (selected == null)
			{
				return;
			}
			HashSet<ManipulatableObject> squashed = new HashSet<ManipulatableObject>();
			foreach (Collider currentCollider in selected.CurrentColliders)
			{
				ManipulatableObject componentInParent = currentCollider.GetComponentInParent<ManipulatableObject>();
				if (componentInParent != null && componentInParent.IsSquashed)
				{
					squashed.Add(componentInParent);
					ManipulatableObject[] componentsInChildren = componentInParent.GetComponentsInChildren<ManipulatableObject>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						squashed.Add(componentsInChildren[i]);
					}
				}
			}
			Action<int, Action<bool>> confirmObjectRemoval = Service.Get<ObjectManipulationService>().ConfirmObjectRemoval;
			if (squashed.Count > 0 && confirmObjectRemoval != null)
			{
				confirmObjectRemoval(squashed.Count, delegate(bool delete)
				{
					if (delete)
					{
						foreach (ManipulatableObject item in squashed)
						{
							if (item != null)
							{
								item.RemoveObject(false);
							}
						}
					}
					objectManipulationInputController.SkipOneFrame = true;
					if (callback != null)
					{
						callback(delete);
					}
				});
				return;
			}
			foreach (ManipulatableObject item2 in squashed)
			{
				if (item2 != null)
				{
					item2.RemoveObject(false);
				}
			}
			if (callback != null)
			{
				callback(true);
			}
		}

		private void onObjectManipulationInputControllerObjectDeselected(ObjectManipulator m)
		{
			m.TriggerEnter -= OnSelectedObjectManipulatorTriggerEnter;
			m.TriggerExit -= OnSelectedObjectManipulatorTriggerExit;
		}

		public void WatchObject(ObjectManipulator m)
		{
			if (m != null)
			{
				m.TriggerEnter += OnSelectedObjectManipulatorTriggerEnter;
				m.TriggerExit += OnSelectedObjectManipulatorTriggerExit;
			}
		}
	}
}
