using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	public class ServerObjectPositionData : ScopedData
	{
		private Vector3 position;

		public Vector3 Position
		{
			get
			{
				return position;
			}
			set
			{
				changed(value);
				position = value;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(ServerObjectPositionDataMonoBehaviour);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		public event Action<Vector3> PositionChanged;

		private void changed(Vector3 newItem)
		{
			if (this.PositionChanged != null)
			{
				this.PositionChanged(newItem);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
