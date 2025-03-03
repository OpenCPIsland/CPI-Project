using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/Group")]
	public class GroupDefinition : StaticGameDataDefinition, IComparable<GroupDefinition>
	{
		[StaticGameDataDefinitionId]
		public string Id;

		[LocalizationToken]
		[Tooltip("The optional user facing name of this group")]
		public string DisplayName;

		public int SortOrder;

		[Tooltip("The collection of definitions in this group")]
		public StaticGameDataDefinitionKey[] Items = new StaticGameDataDefinitionKey[0];

		[Tooltip("Optional, used to create a nested set of groups")]
		public GroupDefinitionKey Parent;

		public int CompareTo(GroupDefinition other)
		{
			return SortOrder.CompareTo(other.SortOrder);
		}
	}
}
