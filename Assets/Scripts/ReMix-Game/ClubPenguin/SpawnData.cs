using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class SpawnData : BaseData
	{
		public SpawnedAction SpawnedAction;

		public Reward PendingReward;

		public Vector3 Position
		{
			get;
			set;
		}

		public Quaternion Rotation
		{
			get;
			set;
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SpawnDataMonoBehaviour);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
