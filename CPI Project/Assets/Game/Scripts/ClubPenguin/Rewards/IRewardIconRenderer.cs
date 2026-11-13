namespace ClubPenguin.Rewards
{
	public interface IRewardIconRenderer
	{
		void RenderReward(DReward reward, RewardIconRenderComplete callback);
	}
}
