using BeanCounter;
using MinigameFramework;

public class mg_bc_GameScreen : MinigameScreen
{
	protected override void Awake()
	{
		mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
		active.StartGame();
		base.Awake();
	}

	protected override void OnCloseClicked()
	{
		base.OnCloseClicked();
		MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UISelect");
	}
}
