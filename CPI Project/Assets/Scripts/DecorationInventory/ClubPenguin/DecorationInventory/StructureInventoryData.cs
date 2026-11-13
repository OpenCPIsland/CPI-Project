using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;

namespace ClubPenguin.DecorationInventory
{
	[Serializable]
	internal class StructureInventoryData : ScopedData
	{
		private Dictionary<int, int> structures;

		public Dictionary<int, int> Structures
		{
			get
			{
				return structures;
			}
			internal set
			{
				structures = new Dictionary<int, int>(value);
				if (this.OnStructuresChanged != null)
				{
					this.OnStructuresChanged(structures);
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
				return typeof(StructureInventoryDataMonoBehaviour);
			}
		}

		public event Action<Dictionary<int, int>> OnStructuresChanged;

		public void AddStructure(int defintionId, int count)
		{
			if (!structures.ContainsKey(defintionId))
			{
				structures[defintionId] = count;
			}
			else
			{
				structures[defintionId] += count;
			}
			if (this.OnStructuresChanged != null)
			{
				this.OnStructuresChanged(structures);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
