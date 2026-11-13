using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	[DisallowMultipleComponent]
	public class LocalPlayerMovementObserver : MonoBehaviour
	{
		private Vector3 lastPosition = Vector3.zero;

		public void Update()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null && localPlayerGameObject.transform.position != lastPosition)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new LocalPlayerPositionEvents.PlayerPositionChangedEvent(lastPosition, localPlayerGameObject));
				lastPosition = localPlayerGameObject.transform.position;
			}
		}
	}
}
