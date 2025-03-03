using ClubPenguin.Interactables;
using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class FilterTriggerAction : Action
	{
		public string OwnerTag;

		public bool TriggerEveryFrame = true;

		public bool ExcludeIfSitting = false;

		public bool ExcludeIfSwimming = false;

		public bool ExcludeIfInAir = false;

		public bool CanInteract(long interactingPlayerId, GameObject actionGraphOwner)
		{
			InvitationalItemExperience component = actionGraphOwner.GetComponent<InvitationalItemExperience>();
			if (component != null && !component.CanInteract(interactingPlayerId))
			{
				return false;
			}
			if (ExcludeIfSitting && LocomotionHelper.IsCurrentControllerOfType<SitController>(actionGraphOwner))
			{
				return false;
			}
			if (ExcludeIfSwimming && LocomotionHelper.IsCurrentControllerOfType<SwimController>(actionGraphOwner))
			{
				return false;
			}
			if (ExcludeIfInAir)
			{
				Animator component2 = actionGraphOwner.GetComponent<Animator>();
				if (component2 != null && LocomotionUtils.IsInAir(LocomotionUtils.GetAnimatorStateInfo(component2)))
				{
					return false;
				}
			}
			return true;
		}

		protected override void CopyTo(Action _destAction)
		{
			FilterTriggerAction filterTriggerAction = _destAction as FilterTriggerAction;
			filterTriggerAction.OwnerTag = OwnerTag;
			filterTriggerAction.TriggerEveryFrame = TriggerEveryFrame;
			filterTriggerAction.ExcludeIfSitting = ExcludeIfSitting;
			filterTriggerAction.ExcludeIfSwimming = ExcludeIfSwimming;
			filterTriggerAction.ExcludeIfInAir = ExcludeIfInAir;
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			Completed();
		}
	}
}
