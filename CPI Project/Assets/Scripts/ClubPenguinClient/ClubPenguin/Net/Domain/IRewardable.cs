using LitJson;

namespace ClubPenguin.Net.Domain
{
	public interface IRewardable
	{
		string RewardType
		{
			get;
		}

		object Reward
		{
			get;
		}

		void FromJson(JsonData jsonData);

		void Add(IRewardable reward);

		bool IsEmpty();
	}
}
