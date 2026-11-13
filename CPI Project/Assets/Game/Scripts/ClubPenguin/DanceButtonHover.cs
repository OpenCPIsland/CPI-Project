using UnityEngine;
using UnityEngine.EventSystems;

namespace ClubPenguin
{
	public class DanceButtonHover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
	{
		private DanceGameIdleState danceGameState;

		public DanceMove DanceMove;

		private void Start()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			Animator component = localPlayerGameObject.GetComponent<Animator>();
			danceGameState = component.GetBehaviour<DanceGameIdleState>();
		}

		public void OnPointerEnter(PointerEventData evt)
		{
			danceGameState.QueueMove(DanceMove);
			evt.Use();
		}
	}
}
