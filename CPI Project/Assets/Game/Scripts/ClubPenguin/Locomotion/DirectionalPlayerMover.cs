using ClubPenguin.Net.Client.Event;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class DirectionalPlayerMover
	{
		private Vector3 destinationFinal;

		private Vector3 faceDirection;

		private Vector3 destinationInitial;

		private Transform movementTransform;

		private LocomoteToPointMover mover;

		public void StartPlayerMovement(Vector3 destination, Vector3 faceDirection)
		{
			destinationFinal = destination;
			this.faceDirection = faceDirection;
			destinationInitial = getInitialDestination();
			mover = createLocomoteMover();
			movementTransform = createMovementTransform();
			startWalkInitial();
		}

		private Transform createMovementTransform()
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.rotation = Quaternion.LookRotation(getForwardVector());
			return gameObject.transform;
		}

		private LocomoteToPointMover createLocomoteMover()
		{
			return SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.AddComponent<LocomoteToPointMover>();
		}

		private void startWalkInitial()
		{
			movementTransform.position = destinationInitial;
			mover.OnComplete += onWalkInitialComplete;
			mover.MoveToTarget(movementTransform, 0.15f, PlayerLocoStyle.Style.Walk, 4f, false);
		}

		private void onWalkInitialComplete()
		{
			mover.OnComplete -= onWalkInitialComplete;
			startWalkFinal();
		}

		private void startWalkFinal()
		{
			movementTransform.position = destinationFinal;
			mover.OnComplete += onWalkFinalComplete;
			mover.MoveToTarget(movementTransform, 0.15f, PlayerLocoStyle.Style.Walk, 4f, false);
		}

		private void onWalkFinalComplete()
		{
			mover.OnComplete -= onWalkFinalComplete;
			Object.Destroy(movementTransform.gameObject);
			Object.Destroy(mover);
		}

		private Vector3 getInitialDestination()
		{
			return destinationFinal + -faceDirection * 0.5f;
		}

		private Vector3 getForwardVector()
		{
			Vector3 result = destinationFinal - destinationInitial;
			result.y = 0f;
			return result;
		}
	}
}
