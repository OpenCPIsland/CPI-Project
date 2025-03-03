using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class UpgradeAvailablePromptData : BaseData
	{
		public bool HasSeenUpgradeAvailablePrompt;

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
