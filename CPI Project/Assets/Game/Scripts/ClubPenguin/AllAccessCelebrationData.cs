using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class AllAccessCelebrationData : ScopedData
	{
		public bool ShowAllAccessCelebration;

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
				return typeof(AllAccessCelebrationDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
