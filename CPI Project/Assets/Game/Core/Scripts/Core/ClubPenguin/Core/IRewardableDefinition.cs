using ClubPenguin.Net.Domain;

namespace ClubPenguin.Core
{
	public interface IRewardableDefinition
	{
		IRewardable Reward
		{
			get;
		}
	}
}
