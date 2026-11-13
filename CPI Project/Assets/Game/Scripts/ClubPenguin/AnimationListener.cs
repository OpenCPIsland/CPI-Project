using UnityEngine;
using UnityEngine.Events;

namespace ClubPenguin
{
	public class AnimationListener : MonoBehaviour
	{
		public UnityEvent Listener;

		public void OnAnimationEventDispatched()
		{
			Listener.Invoke();
		}
	}
}
