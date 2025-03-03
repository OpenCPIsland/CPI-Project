using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class LocalPlayerInZoneData : BaseData
	{
		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(LocalPlayerInZoneDataDataMonoBehaviour);
			}
		}

		public event Action<LocalPlayerInZoneData> LocalPlayerLeftZone;

		protected override void notifyWillBeDestroyed()
		{
			if (this.LocalPlayerLeftZone != null)
			{
				this.LocalPlayerLeftZone(this);
			}
			this.LocalPlayerLeftZone = null;
		}
	}
}
