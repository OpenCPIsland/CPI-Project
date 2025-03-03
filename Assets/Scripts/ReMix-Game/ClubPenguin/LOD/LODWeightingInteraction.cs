using ClubPenguin.Locomotion;
using Disney.Kelowna.Common.DataModel;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODWeightingInteraction : LODWeightingRule
	{
		public LODWeightingInteractionData Data;

		public override void Setup()
		{
			base.Setup();
			request.Data.OnGameObjectGeneratedEvent += onGameObjectGenerated;
		}

		public override void OnDisable()
		{
			request.Data.OnGameObjectGeneratedEvent -= onGameObjectGenerated;
		}

		protected override float UpdateWeighting()
		{
			return 0f;
		}

		private void onGameObjectGenerated(GameObject remotePlayer, DataEntityHandle remotePlayerHandle, LODRequestData requestData)
		{
			LocomotionEventBroadcaster component = remotePlayer.GetComponent<LocomotionEventBroadcaster>();
			if (component != null)
			{
				component.OnDoActionEvent += onLocomotionBroadcasterDoAction;
			}
		}

		private void onLocomotionBroadcasterDoAction(LocomotionController.LocomotionAction action, object userData = null)
		{
			if (action == LocomotionController.LocomotionAction.Interact)
			{
				LODWeightingCurve lODWeightingCurve = base.gameObject.AddComponent<LODWeightingCurve>();
				lODWeightingCurve.CurveData = Data.CurveData;
				lODWeightingCurve.Setup();
			}
		}
	}
}
