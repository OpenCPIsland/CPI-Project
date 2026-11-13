#define UNITY_ASSERTIONS
using ClubPenguin.ObjectManipulation.Input;
using Disney.Kelowna.Common;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.ObjectManipulation
{
	public class SplittableObject : MonoBehaviour
	{
		[Tooltip("Objects to be split apart once item has been deselected")]
		public GameObject[] SplitList;

		[Tooltip("The object that will be kept when deserialized")]
		public GameObject LayoutObject;

		private ObjectManipulationInputController objectManipulationInputController;

		public event Action<SplittableObject> ChildrenSplit;

		public void SetObjectManipulationController(ObjectManipulationInputController controller)
		{
			if (objectManipulationInputController == null)
			{
				objectManipulationInputController = controller;
				objectManipulationInputController.ObjectDeselected += OnObjectManipulatorObjectDeselected;
			}
		}

		public void OnValidate()
		{
			Assert.IsNotNull(SplitList, "The split list cannot be null!");
			Assert.IsTrue(SplitList.Length > 0, "The split list cannot be empty");
		}

		public void OnDestroy()
		{
			this.ChildrenSplit = null;
			if (objectManipulationInputController != null)
			{
				objectManipulationInputController.ObjectDeselected -= OnObjectManipulatorObjectDeselected;
			}
		}

		public void SplitChildren()
		{
			for (int i = 0; i < SplitList.Length; i++)
			{
				SplitList[i].transform.SetParent(null);
			}
			this.ChildrenSplit.InvokeSafe(this);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void OnObjectManipulatorObjectDeselected(ObjectManipulator objectManipulator)
		{
			if (objectManipulator.IsAllowed)
			{
				SplitChildren();
			}
		}

		public GameObject ExtractSingleItem()
		{
			LayoutObject.transform.SetParent(null);
			UnityEngine.Object.Destroy(base.gameObject);
			return LayoutObject;
		}
	}
}
