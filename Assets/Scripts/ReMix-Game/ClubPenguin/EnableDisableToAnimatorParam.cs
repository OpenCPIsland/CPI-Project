using UnityEngine;

namespace ClubPenguin
{
	public class EnableDisableToAnimatorParam : MonoBehaviour
	{
		public Animator Target;

		public string BoolParam;

		public bool Invert;

		private int paramHash;

		private void Awake()
		{
			paramHash = Animator.StringToHash(BoolParam);
		}

		private void OnEnable()
		{
			if (Target != null)
			{
				Target.SetBool(paramHash, !Invert);
			}
		}

		private void OnDisable()
		{
			if (Target != null)
			{
				Target.SetBool(paramHash, Invert);
			}
		}
	}
}
