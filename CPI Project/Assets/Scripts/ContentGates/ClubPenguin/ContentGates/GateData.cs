using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;

namespace ClubPenguin.ContentGates
{
	[Serializable]
	internal class GateData : ScopedData
	{
		public Dictionary<Type, bool> GateStatus
		{
			get;
			set;
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(GateDataMonoBehaviour);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Application.ToString();
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
