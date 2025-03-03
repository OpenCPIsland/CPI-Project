using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class ScriptedController : LocomotionController
	{
		private Animator anim;

		private int stateID;

		private bool animPending;

		private int layerIndex;

		public void Awake()
		{
			anim = GetComponent<Animator>();
		}

		public void PlayAnim(string stateName, int _layerIndex)
		{
			animPending = true;
			stateID = Animator.StringToHash(stateName);
			layerIndex = _layerIndex;
		}

		public void Update()
		{
			if (!animPending || !(anim != null))
			{
				return;
			}
			if (anim.HasState(layerIndex, stateID))
			{
				anim.Play(stateID, layerIndex, 0f);
				if ((anim.IsInTransition(layerIndex) ? anim.GetNextAnimatorStateInfo(layerIndex) : anim.GetCurrentAnimatorStateInfo(layerIndex)).fullPathHash == stateID)
				{
					animPending = false;
				}
			}
			else
			{
				Debug.LogWarning("ScriptedController couldn't find specified animation ID: " + stateID);
				animPending = false;
			}
		}

		public override bool AllowTriggerInteractions()
		{
			return false;
		}

		public override bool AllowTriggerOnStay()
		{
			return false;
		}
	}
}
