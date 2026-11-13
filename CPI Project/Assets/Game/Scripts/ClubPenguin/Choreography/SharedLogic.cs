using UnityEngine;

namespace ClubPenguin.Choreography
{
	public abstract class SharedLogic : ScriptableObject
	{
		public abstract bool Execute(Actor.InteractionState interactionState);
	}
}
