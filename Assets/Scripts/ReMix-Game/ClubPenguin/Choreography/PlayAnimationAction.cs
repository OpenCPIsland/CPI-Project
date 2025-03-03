using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Choreography
{
	public class PlayAnimationAction : ScriptableAction
	{
		public string StateName;

		public int LayerIndex;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			int stateHash = Animator.StringToHash(StateName);
			Animator anim = player.GetComponent<Animator>();
			anim.Play(stateHash, LayerIndex);
			while (anim.GetCurrentAnimatorStateInfo(LayerIndex).shortNameHash != stateHash)
			{
				yield return null;
			}
			while (anim.GetCurrentAnimatorStateInfo(LayerIndex).shortNameHash == stateHash)
			{
				yield return null;
			}
		}
	}
}
