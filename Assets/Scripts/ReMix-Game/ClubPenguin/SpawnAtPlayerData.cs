using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class SpawnAtPlayerData : BaseData
	{
		public string PlayerSWID
		{
			get;
			set;
		}

		public Vector3 PlayerPosition
		{
			get;
			set;
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SpawnAtPlayerDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
