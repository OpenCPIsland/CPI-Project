using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	internal class DecorationInventoryData : ScopedData
	{
		private Dictionary<int, int> decorations;

		public Dictionary<int, int> Decorations
		{
			get
			{
				return decorations;
			}
			internal set
			{
				decorations = new Dictionary<int, int>(value);
				if (this.OnDecorationsChanged != null)
				{
					this.OnDecorationsChanged(decorations);
				}
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(DecorationInventoryDataMonoBehaviour);
			}
		}

		public event Action<Dictionary<int, int>> OnDecorationsChanged;

		public void AddDecoration(int defintionId, int count)
		{
			if (!decorations.ContainsKey(defintionId))
			{
				decorations[defintionId] = count;
			}
			else
			{
				decorations[defintionId] += count;
			}
			if (this.OnDecorationsChanged != null)
			{
				this.OnDecorationsChanged(decorations);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
