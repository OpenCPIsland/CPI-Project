using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public abstract class VisibilityIterator : MonoBehaviour
	{
		public abstract Visibility Current
		{
			get;
		}

		public abstract bool MoveNext();

		public abstract void Reset();
	}
}
