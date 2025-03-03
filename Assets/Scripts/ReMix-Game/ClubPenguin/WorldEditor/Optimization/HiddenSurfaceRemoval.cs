using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	[RequireComponent(typeof(VisibilityIterator))]
	public abstract class HiddenSurfaceRemoval : MonoBehaviour
	{
		public abstract void Begin(GameObjectData[] datas);

		public abstract void Execute(Visibility visibility);

		public abstract void End();
	}
}
