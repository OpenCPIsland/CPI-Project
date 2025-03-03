using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.Breadcrumbs
{
	[Serializable]
	[CreateAssetMenu(menuName = "Definition/FeatureLabelBreadcrumbDefinition")]
	public class FeatureLabelBreadcrumbDefinition : StaticGameDataDefinition
	{
		public struct DictionaryKey
		{
			public readonly string TypeId;

			public readonly PersistentBreadcrumbTypeDefinition Type;

			public DictionaryKey(string typeId, PersistentBreadcrumbTypeDefinition type)
			{
				TypeId = typeId;
				Type = type;
			}
		}

		public string TypeId;

		public PersistentBreadcrumbTypeDefinition Type;

		public DateUnityWrapper EndDate;

		[Header("at the base of the hierarchy must be seen for this one to be consider seen.")]
		[Header("If this Feature Label is part of a hierarchy (or trail) of labels, the following label(s)")]
		public FeatureLabelBreadcrumbDefinition[] DependentFeatureLabelBreadcrumbs;
	}
}
