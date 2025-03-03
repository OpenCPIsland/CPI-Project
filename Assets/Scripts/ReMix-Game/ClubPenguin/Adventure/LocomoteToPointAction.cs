using ClubPenguin.Locomotion;
using ClubPenguin.Net.Client.Event;
using Disney.Kelowna.Common;
using HutongGames.PlayMaker;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Locomotion")]
	[HutongGames.PlayMaker.Tooltip("Tells the Target to move to the Destination position.")]
	public class LocomoteToPointAction : FsmStateAction
	{
		public enum LocomotionStyle
		{
			Walk,
			Run
		}

		private const float CONTROLLER_DISABLE_TIME_IN_SECONDS = 2f;

		public FsmGameObject Target;

		public FsmGameObject Destination;

		public LocomotionStyle Style = LocomotionStyle.Walk;

		public FsmFloat DistanceThreshold = 0.15f;

		public float TimeoutTime = 4f;

		public bool WaitForFinish = true;

		private LocomoteToPointMover locomoteMover;

		public override void OnEnter()
		{
			if (Target != null && Destination != null)
			{
				locomoteMover = Target.Value.gameObject.AddComponent<LocomoteToPointMover>();
				PlayerLocoStyle.Style locomotionStyle = PlayerLocoStyle.Style.Walk;
				if (Style == LocomotionStyle.Run)
				{
					locomotionStyle = PlayerLocoStyle.Style.Run;
				}
				if (WaitForFinish)
				{
					locomoteMover.OnComplete += onLocomoteComplete;
				}
				locomoteMover.MoveToTarget(Destination.Value.transform, DistanceThreshold.Value, locomotionStyle, TimeoutTime, !WaitForFinish);
				disableOtherLocomotionControllers();
				if (!WaitForFinish)
				{
					Finish();
				}
			}
			else
			{
				Finish();
			}
		}

		private void disableOtherLocomotionControllers()
		{
			LocomotionTracker component = Target.Value.gameObject.GetComponent<LocomotionTracker>();
			if (component != null)
			{
				component.DisallowController<SlideController>(2f);
			}
		}

		private void onLocomoteComplete()
		{
			locomoteMover.OnComplete -= onLocomoteComplete;
			CoroutineRunner.Start(End(), this, "");
		}

		private IEnumerator End()
		{
			Object.Destroy(locomoteMover);
			yield return new WaitForEndOfFrame();
			Finish();
		}
	}
}
