using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	[Serializable]
	public class ClaimedRewardIdsData : ScopedData
	{
		private List<int> rewardIds;

		public List<int> RewardIds
		{
			get
			{
				if (rewardIds == null)
				{
					rewardIds = new List<int>();
				}
				return rewardIds;
			}
			internal set
			{
				if (value != null)
				{
					rewardIds = new List<int>(value);
				}
				else
				{
					rewardIds = new List<int>();
				}
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(ClaimedRewardIdsDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
