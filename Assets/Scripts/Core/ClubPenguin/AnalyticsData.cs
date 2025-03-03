using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	[Serializable]
	public class AnalyticsData : ScopedData
	{
		private HashSet<string> singularCalls = new HashSet<string>();

		public HashSet<string> SingularCalls
		{
			get
			{
				return singularCalls;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(AnalyticsDataMonoBehaviour);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
