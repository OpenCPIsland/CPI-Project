using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class SearchedUserData : ScopedData
	{
		public IUnidentifiedUser SearchedUser
		{
			get;
			set;
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SearchedUserDataMonoBehaviour);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
