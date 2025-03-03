using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class RemotePlayerData : ScopedData
	{
		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(RemotePlayerDataMonoBehaviour);
			}
		}

		public event Action<RemotePlayerData> PlayerRemoved;

		protected override void notifyWillBeDestroyed()
		{
			if (this.PlayerRemoved != null)
			{
				this.PlayerRemoved(this);
			}
			this.PlayerRemoved = null;
		}
	}
}
