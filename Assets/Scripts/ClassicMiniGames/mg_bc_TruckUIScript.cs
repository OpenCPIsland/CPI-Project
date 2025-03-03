using BeanCounter;
using MinigameFramework;

public class mg_bc_TruckUIScript : mg_bc_UIValueLabel<int>
{
	public override void Start()
	{
		base.Start();
		MinigameManager.GetActive<mg_BeanCounter>().GameLogic.Truck.TruckNumber.SetDisplayer(this);
	}
}
