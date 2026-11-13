using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public abstract class GoalPlanner : MonoBehaviour
	{
		[HideInInspector]
		public bool Dirty;

		public virtual bool IsFinished
		{
			get
			{
				return true;
			}
		}

		public abstract void Plan(ref Setup setup);
	}
}
