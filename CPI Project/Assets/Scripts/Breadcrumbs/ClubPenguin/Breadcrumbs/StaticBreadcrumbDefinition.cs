using ClubPenguin.Core.StaticGameData;
using System;
using UnityEngine;

namespace ClubPenguin.Breadcrumbs
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/StaticBreadcrumb")]
	public class StaticBreadcrumbDefinition : StaticGameDataDefinition
	{
		[StaticGameDataDefinitionId]
		public string Id;
	}
}
