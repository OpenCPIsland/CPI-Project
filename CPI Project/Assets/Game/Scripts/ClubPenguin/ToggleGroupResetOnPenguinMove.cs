using ClubPenguin.Gui;
using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin
{
	public class ToggleGroupResetOnPenguinMove : MonoBehaviour
	{
		private SpriteToggleGroupButton[] group;

		private LocomotionEventBroadcaster locomotionEventBroadcaster;

		public void Start()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				locomotionEventBroadcaster = localPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
				locomotionEventBroadcaster.OnStickDirectionEvent += OnStickDirection;
			}
			group = base.transform.GetComponentsInChildren<SpriteToggleGroupButton>();
		}

		private void OnStickDirection(Vector2 steer)
		{
			if (steer != Vector2.zero && group != null)
			{
				group[0].SetSprite(true);
				for (int i = 1; i < group.Length; i++)
				{
					group[i].SetSprite(false);
				}
			}
		}

		public void OnDestroy()
		{
			if (locomotionEventBroadcaster != null)
			{
				locomotionEventBroadcaster.OnStickDirectionEvent -= OnStickDirection;
			}
		}
	}
}
