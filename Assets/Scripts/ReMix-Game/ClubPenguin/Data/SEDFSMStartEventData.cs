using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin.Data
{
	[Serializable]
	public class SEDFSMStartEventData : ScopedData
	{
		public string EventName;

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
				return typeof(SEDFSMStartEventDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
