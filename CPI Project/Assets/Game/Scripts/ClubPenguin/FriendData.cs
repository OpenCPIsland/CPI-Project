using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class FriendData : ScopedData
	{
		public IFriend Friend
		{
			get;
			set;
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(FriendDataMonoBehaviour);
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
