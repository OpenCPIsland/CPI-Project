using UnityEngine;

namespace ClubPenguin.IslandTargets
{
	public class IslandTargetsSideScoreboard : MonoBehaviour
	{
		public enum SideScoreboardState
		{
			Off,
			ThirtySecsGameStartMark,
			FiveSecsGameStartMark,
			InGame,
			BetweenRounds,
			FinalRound,
			RoundFail,
			WonGame
		}

		public GameObject OffGroup;

		public GameObject InGameGroup;

		public GameObject BetweenRoundsGroup;

		public GameObject FinalRoundGroup;

		public GameObject RoundFailGroup;

		public GameObject WonGameGroup;

		public GameObject ThirtySecsGameStartMarkGroup;

		public GameObject FiveSecsGameStartMarkGroup;

		private Animator animator;

		private string INTRO_ANIM_TRIGGER = "Start";

		private string OUTRO_ANIM_TRIGGER = "End";

		public SideScoreboardState CurrentState
		{
			get;
			private set;
		}

		private void Awake()
		{
			animator = GetComponent<Animator>();
			SetState(SideScoreboardState.Off);
		}

		public void SetState(SideScoreboardState newState)
		{
			switch (newState)
			{
			case SideScoreboardState.Off:
				OffGroup.SetActive(true);
				InGameGroup.SetActive(false);
				BetweenRoundsGroup.SetActive(false);
				FinalRoundGroup.SetActive(false);
				RoundFailGroup.SetActive(false);
				WonGameGroup.SetActive(false);
				ThirtySecsGameStartMarkGroup.SetActive(false);
				FiveSecsGameStartMarkGroup.SetActive(false);
				if (animator != null && CurrentState != 0)
				{
					animator.SetTrigger(OUTRO_ANIM_TRIGGER);
				}
				break;
			case SideScoreboardState.ThirtySecsGameStartMark:
				OffGroup.SetActive(false);
				InGameGroup.SetActive(false);
				BetweenRoundsGroup.SetActive(false);
				FinalRoundGroup.SetActive(false);
				RoundFailGroup.SetActive(false);
				WonGameGroup.SetActive(false);
				ThirtySecsGameStartMarkGroup.SetActive(true);
				FiveSecsGameStartMarkGroup.SetActive(false);
				if (animator != null && CurrentState == SideScoreboardState.Off)
				{
					animator.SetTrigger(INTRO_ANIM_TRIGGER);
				}
				break;
			case SideScoreboardState.FiveSecsGameStartMark:
				OffGroup.SetActive(false);
				InGameGroup.SetActive(false);
				BetweenRoundsGroup.SetActive(false);
				FinalRoundGroup.SetActive(false);
				RoundFailGroup.SetActive(false);
				WonGameGroup.SetActive(false);
				ThirtySecsGameStartMarkGroup.SetActive(false);
				FiveSecsGameStartMarkGroup.SetActive(true);
				if (animator != null && CurrentState == SideScoreboardState.Off)
				{
					animator.SetTrigger(INTRO_ANIM_TRIGGER);
				}
				break;
			case SideScoreboardState.InGame:
				OffGroup.SetActive(false);
				InGameGroup.SetActive(true);
				BetweenRoundsGroup.SetActive(false);
				FinalRoundGroup.SetActive(false);
				RoundFailGroup.SetActive(false);
				WonGameGroup.SetActive(false);
				ThirtySecsGameStartMarkGroup.SetActive(false);
				FiveSecsGameStartMarkGroup.SetActive(false);
				if (animator != null && CurrentState == SideScoreboardState.Off)
				{
					animator.SetTrigger(INTRO_ANIM_TRIGGER);
				}
				break;
			case SideScoreboardState.BetweenRounds:
				OffGroup.SetActive(false);
				InGameGroup.SetActive(false);
				BetweenRoundsGroup.SetActive(true);
				FinalRoundGroup.SetActive(false);
				RoundFailGroup.SetActive(false);
				WonGameGroup.SetActive(false);
				ThirtySecsGameStartMarkGroup.SetActive(false);
				FiveSecsGameStartMarkGroup.SetActive(false);
				break;
			case SideScoreboardState.FinalRound:
				OffGroup.SetActive(false);
				InGameGroup.SetActive(false);
				BetweenRoundsGroup.SetActive(false);
				FinalRoundGroup.SetActive(true);
				RoundFailGroup.SetActive(false);
				WonGameGroup.SetActive(false);
				ThirtySecsGameStartMarkGroup.SetActive(false);
				FiveSecsGameStartMarkGroup.SetActive(false);
				break;
			case SideScoreboardState.RoundFail:
				OffGroup.SetActive(false);
				InGameGroup.SetActive(false);
				BetweenRoundsGroup.SetActive(false);
				FinalRoundGroup.SetActive(false);
				RoundFailGroup.SetActive(true);
				WonGameGroup.SetActive(false);
				ThirtySecsGameStartMarkGroup.SetActive(false);
				FiveSecsGameStartMarkGroup.SetActive(false);
				break;
			case SideScoreboardState.WonGame:
				OffGroup.SetActive(false);
				InGameGroup.SetActive(false);
				BetweenRoundsGroup.SetActive(false);
				FinalRoundGroup.SetActive(false);
				RoundFailGroup.SetActive(false);
				WonGameGroup.SetActive(true);
				ThirtySecsGameStartMarkGroup.SetActive(false);
				FiveSecsGameStartMarkGroup.SetActive(false);
				break;
			}
			CurrentState = newState;
		}
	}
}
