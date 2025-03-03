using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class PositionData : ScopedData
	{
		[SerializeField]
		private Vector3 position;

		public Vector3 Position
		{
			get
			{
				return position;
			}
			set
			{
				if (value != position)
				{
					positionChanged(value);
					position = value;
				}
			}
		}

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
				return typeof(PositionDataMonoBehaviour);
			}
		}

		public event Action<PositionData, Vector3> PlayerMoved;

		protected override void notifyWillBeDestroyed()
		{
			this.PlayerMoved = null;
		}

		private void positionChanged(Vector3 newPosition)
		{
			this.PlayerMoved.InvokeSafe(this, newPosition);
		}
	}
}
