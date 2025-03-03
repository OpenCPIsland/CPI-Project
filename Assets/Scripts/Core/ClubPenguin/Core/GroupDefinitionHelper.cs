using ClubPenguin.Core.StaticGameData;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin.Core
{
	public static class GroupDefinitionHelper
	{
		private static Dictionary<string, GroupDefinition> _groups = null;

		private static Dictionary<string, GroupDefinition> groups
		{
			get
			{
				if (_groups == null)
				{
					_groups = Service.Get<IGameData>().Get<Dictionary<string, GroupDefinition>>();
				}
				return _groups;
			}
		}

		public static List<GroupDefinition> GetGroups<T>(GroupDefinitionKey parent = null) where T : StaticGameDataDefinition
		{
			string typeString = StaticGameDataKey.GetTypeString(typeof(T));
			List<GroupDefinition> list = new List<GroupDefinition>();
			foreach (GroupDefinition value in groups.Values)
			{
				bool flag = false;
				if ((value.Parent != null && !string.IsNullOrEmpty(value.Parent.Key)) ? (parent != null && value.Parent.Key == parent.Key) : (parent == null || string.IsNullOrEmpty(parent.Key)))
				{
					bool flag2 = false;
					for (int i = 0; i < value.Items.Length; i++)
					{
						if (value.Items[i].UnderliningTypeString == typeString)
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						list.Add(value);
					}
				}
			}
			return list;
		}

		public static List<StaticGameDataDefinitionKey> GetKeys<T>(GroupDefinition group) where T : StaticGameDataDefinition
		{
			string typeString = StaticGameDataKey.GetTypeString(typeof(T));
			List<StaticGameDataDefinitionKey> list = new List<StaticGameDataDefinitionKey>();
			StaticGameDataDefinitionKey[] items = group.Items;
			foreach (StaticGameDataDefinitionKey staticGameDataDefinitionKey in items)
			{
				if (staticGameDataDefinitionKey.UnderliningTypeString == typeString)
				{
					list.Add(staticGameDataDefinitionKey);
				}
			}
			return list;
		}
	}
}
