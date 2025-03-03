using MinigameFramework;

namespace BeanCounter
{
	public class mg_bc_JellyGameLogic : mg_bc_GameLogic
	{
		private int m_droppedBags;

		private mg_bc_CandyMachine m_candyMachine;

		public override void Awake()
		{
			m_droppedBags = 0;
			base.ScoreController = new mg_bc_JellyScoreController(MinigameManager.GetActive<mg_BeanCounter>().GameMode);
			m_candyMachine = GetComponentInChildren<mg_bc_CandyMachine>();
			mg_bc_JellyTruck componentInChildren = GetComponentInChildren<mg_bc_JellyTruck>();
			componentInChildren.CandyMachine = m_candyMachine;
			base.Awake();
			(base.DropZone as mg_bc_JellyDropZone).Truck = componentInChildren;
		}

		internal override void OnObjectDropped(mg_bc_FlyingObject _groundedObject)
		{
			base.OnObjectDropped(_groundedObject);
			if (_groundedObject is mg_bc_Bag && !base.Penguin.IsDead)
			{
				if (m_droppedBags % 10 == 0)
				{
					ShowHint("Do not\ndrop bags!", 2f);
				}
				m_droppedBags++;
			}
		}
	}
}
