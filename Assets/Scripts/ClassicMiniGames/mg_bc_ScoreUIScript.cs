using BeanCounter;

public class mg_bc_ScoreUIScript : mg_bc_UIValueLabel<int>
{
	public override void Start()
	{
		base.Start();
		mg_bc_ScoreController.Instance.CurrentScore.SetDisplayer(this);
	}
}
