using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class PlayerCardData : BaseData
	{
		public bool Enabled = true;

		public bool IsPlayerCardShowing;

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
