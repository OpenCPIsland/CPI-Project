using UnityEngine;
using UnityEngine.Events;

namespace ClubPenguin
{
	public class MultipleAnimationListener : MonoBehaviour
	{
		public UnityEvent[] Listeners;

		public void OnAnimationEventDispatched(int index)
		{
			Listeners[index].Invoke();
		}
	}
}
