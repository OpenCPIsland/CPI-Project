using MinigameFramework;

namespace Pizzatron
{
	public class mg_pt_Conveyor
	{
		private mg_pt_EConveyorState m_nextState;

		private mg_pt_ConveyorObject m_conveyorObject;

		private mg_pt_ConveyorSpeedData m_speedData;

		private mg_pt_ConveyorTierData m_tierData;

		private int m_currentTier;

		private int m_sequentialFails;

		public mg_pt_EConveyorState State
		{
			get;
			private set;
		}

		public float ComplexitySpeedMultiplier
		{
			get;
			set;
		}

		public float ConveyorSpeed
		{
			get
			{
				return 0.58f * ComplexitySpeedMultiplier * m_tierData.SpeedModifier;
			}
		}

		public void Initialize(mg_pt_GameScreen p_screen, mg_pt_ConveyorSpeedData p_speedData)
		{
			m_speedData = p_speedData;
			State = mg_pt_EConveyorState.STOPPED;
			m_nextState = mg_pt_EConveyorState.RUNNING;
			m_conveyorObject = p_screen.ConveyorObject;
			m_currentTier = 0;
			m_sequentialFails = 0;
			UpdateTier();
			NextState();
			PlaySFX();
		}

		public void MinigameUpdate(float p_deltaTime, float p_pizzaSpeedModifier)
		{
			m_conveyorObject.Reposition(p_deltaTime, ConveyorSpeed * p_pizzaSpeedModifier);
		}

		private void NextState()
		{
			if (m_nextState != mg_pt_EConveyorState.INVALID && m_nextState != State)
			{
				State = m_nextState;
				m_nextState = mg_pt_EConveyorState.RUNNING;
			}
		}

		public void PizzaCompleted()
		{
			m_sequentialFails = 0;
			m_currentTier++;
			UpdateTier();
		}

		public void PizzaFailed()
		{
			m_sequentialFails++;
			m_currentTier -= m_sequentialFails;
			UpdateTier();
		}

		private void UpdateTier()
		{
			m_currentTier = m_speedData.NormalizeTier(m_currentTier);
			m_tierData = m_speedData.GetTier(m_currentTier);
		}

		public void PlaySFX()
		{
			MinigameManager.GetActive().PlaySFX("mg_pt_sfx_conveyor_belt_loop");
		}
	}
}
