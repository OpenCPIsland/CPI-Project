using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_Penguin : MonoBehaviour
	{
		private static string ANIM_STATE = "state";

		private Animator m_animator;

		private mg_if_FishingRod m_rod;

		private mg_if_EPenguinState m_state;

		private mg_if_EPenguinState State
		{
			get
			{
				return m_state;
			}
			set
			{
				if (m_state != value)
				{
					m_state = value;
					m_animator.SetInteger(ANIM_STATE, (int)m_state);
				}
			}
		}

		public void Awake()
		{
			m_animator = base.gameObject.GetComponentInChildren<Animator>();
			base.transform.Find("mg_if_PenguinColour").GetComponent<SpriteRenderer>().color = MinigameManager.Instance.GetPenguinColor();
		}

		public void Start()
		{
			State = mg_if_EPenguinState.STATE_UP;
		}

		public void Initialize(mg_if_FishingRod p_rod)
		{
			m_rod = p_rod;
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			if (State != mg_if_EPenguinState.STATE_SHOCK)
			{
				UpdateState();
			}
		}

		private void UpdateState()
		{
			mg_if_EPenguinState state = State;
			state = (State = (m_rod.IsBroken ? mg_if_EPenguinState.STATE_SAD : ((!m_rod.IsHookAboveWater) ? mg_if_EPenguinState.STATE_DOWN : mg_if_EPenguinState.STATE_UP)));
		}

		public void Shock()
		{
			State = mg_if_EPenguinState.STATE_SHOCK;
		}

		public void StopShock()
		{
			UpdateState();
		}
	}
}
