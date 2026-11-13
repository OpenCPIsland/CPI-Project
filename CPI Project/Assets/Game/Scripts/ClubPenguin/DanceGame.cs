using ClubPenguin.Locomotion;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Animator))]
	public class DanceGame : MonoBehaviour
	{
		private const string danceMoveSymbols = " ABCD";

		public Image ChallengeIndicator;

		public Image MoveIndicator;

		public Text MoveText;

		private Animator anim;

		private void Awake()
		{
			anim = GetComponent<Animator>();
		}

		public bool ToggleChallenge()
		{
			ChallengeIndicator.enabled = !ChallengeIndicator.enabled;
			return ChallengeIndicator.enabled;
		}

		public void SetCurrentMove(DanceMove move)
		{
			MoveText.text = " ABCD".Substring((int)move, 1);
		}

		private void Update()
		{
			if (LocomotionHelper.IsCurrentControllerOfType<RunController>(base.gameObject))
			{
				MoveIndicator.enabled = anim.GetBool("Dancing");
			}
		}
	}
}
