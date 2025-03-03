using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class EntityKeepAliveData : ScopedData
	{
		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(EntityKeepAliveDataMonoBehaviour);
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
