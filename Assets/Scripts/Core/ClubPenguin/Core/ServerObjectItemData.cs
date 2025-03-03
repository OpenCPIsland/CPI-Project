using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin.Core
{
	[Serializable]
	public class ServerObjectItemData : ScopedData, IEntityIdentifierData<CPMMOItemId>
	{
		private CPMMOItem item;

		public CPMMOItem Item
		{
			get
			{
				return item;
			}
			set
			{
				changed(value);
				item = value;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(ServerObjectDataMonoBehaviour);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		public event Action<CPMMOItem> ItemChanged;

		public static string GetEntityName(CPMMOItemId itemId)
		{
			return "so_" + itemId.ToString();
		}

		public bool Match(CPMMOItemId identifier)
		{
			return item.Id.Equals(identifier);
		}

		private void changed(CPMMOItem newItem)
		{
			if (this.ItemChanged != null)
			{
				this.ItemChanged(newItem);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
