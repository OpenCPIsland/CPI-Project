using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class SessionIdData : ScopedData, IEntityIdentifierData<long>
	{
		[SerializeField]
		private long sessionId;

		public long SessionId
		{
			get
			{
				return sessionId;
			}
			set
			{
				if (value > 0)
				{
					sessionId = value;
					return;
				}
				throw new Exception("Session Id is not greater than zero");
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SessionIdDataMonoBehaviour);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		public bool Match(long identifier)
		{
			return identifier == sessionId;
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
