using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class DailySpinEntityData : ScopedData
	{
		[SerializeField]
		public long TimeOfLastSpinInMilliseconds;

		[SerializeField]
		public int CurrentChestId;

		[SerializeField]
		public int NumPunchesOnCurrentChest;

		[SerializeField]
		public int NumChestsReceivedOfCurrentChestId;

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(DailySpinEntityDataMonoBehaviour);
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
