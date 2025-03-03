using ClubPenguin.Core;
using ClubPenguin.ObjectManipulation;
using UnityEngine;

namespace ClubPenguin.SceneManipulation
{
	public interface ISceneModifier
	{
		void ObjectAdded(DecorationLayoutData data, GameObject go);

		void ObjectRemoved(DecorationLayoutData data, GameObject go);

		void ProcessObject(GameObject go);

		void OnLayoutProcessed();

		bool CanObjectBeSelected(ManipulatableObject obj);

		void AfterObjectSelected(ManipulatableObject obj, bool isNewObject);

		void AfterObjectDeselected(ObjectManipulator obj);

		void Destroy();
	}
}
