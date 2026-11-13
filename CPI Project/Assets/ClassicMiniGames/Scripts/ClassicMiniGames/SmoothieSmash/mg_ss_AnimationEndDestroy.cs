using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_AnimationEndDestroy : MonoBehaviour
	{
		private void OnAnimationFinished()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
