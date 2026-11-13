using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/World")]
	public class WorldDefinition : StaticGameDataDefinition
	{
		[Tooltip("The identifier of the world")]
		[StaticGameDataDefinitionId]
		public string WorldName;

		[Tooltip("Required language for user in this world")]
		public Language Language;

		[Tooltip("Optional timezones (long format from the TZ database) used to default place users in this world")]
		public List<string> TimeZones = new List<string>();

		[Tooltip("Optional countrie (ISO 3166-1 alpha-2) codes used to default place users in this world")]
		public List<string> Countries = new List<string>();

		[Tooltip("Optional region codes within countries (ie: province or state) used to default place users in this world")]
		public List<string> Regions = new List<string>();

		[Tooltip("Optional city names used to default place users in this world")]
		public List<string> Cities = new List<string>();

		[Tooltip("The world that all igloos are considered part of, one and only one per language")]
		public bool Igloo;
	}
}
