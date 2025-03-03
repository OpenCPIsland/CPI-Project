using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin
{
	public class SwordColliderEnabler : MonoBehaviour
	{
		private bool isColliderEnabled = false;

		private Collider swordCollider;

		private GameObject owner;

		private EventDispatcher dispatcher;

		public void Awake()
		{
			swordCollider = GetComponent<Collider>();
			swordCollider.enabled = false;
		}

		public void Start()
		{
			owner = base.transform.parent.GetComponent<Prop>().PropUserRef.gameObject;
		}

		public void Update()
		{
			bool flag = isPlayingPropAnimation();
			if (isColliderEnabled && !flag)
			{
				disableCollider();
			}
			else if (!isColliderEnabled && flag)
			{
				enableCollider();
			}
		}

		private void enableCollider()
		{
			swordCollider.enabled = true;
			isColliderEnabled = true;
		}

		private void disableCollider()
		{
			swordCollider.enabled = false;
			isColliderEnabled = false;
		}

		private bool isPlayingPropAnimation()
		{
			AnimatorStateInfo currentAnimatorStateInfo = owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(AnimationHashes.Layers.Torso);
			if (currentAnimatorStateInfo.fullPathHash == AnimationHashes.States.Interactions.TorsoAction1 || currentAnimatorStateInfo.fullPathHash == AnimationHashes.States.Interactions.TorsoAction2 || currentAnimatorStateInfo.fullPathHash == AnimationHashes.States.Interactions.TorsoAction3)
			{
				return true;
			}
			return false;
		}
	}
}
