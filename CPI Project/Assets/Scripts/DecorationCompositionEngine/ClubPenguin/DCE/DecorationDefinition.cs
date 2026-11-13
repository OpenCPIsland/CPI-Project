using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.DCE
{
	public class DecorationDefinition : ScriptableObject
	{
		[Serializable]
		public class DecorationModelDefinition
		{
			[Serializable]
			public struct Part
			{
				public int SlotIndex;

				public bool Required;
			}

			public string Name;

			public Part[] Parts;
		}

		public const string DEFINITION_NAME_PATTERN = "{0}_{1}_{2}_{3}LOD";

		public static readonly AssetContentKey DEFINITION_KEYPATTERN = new AssetContentKey("definitions/decorations/*");

		public readonly Dictionary<string, int> DecorationIndexLookup = new Dictionary<string, int>();

		public readonly Dictionary<string, int> BoneIndexLookup = new Dictionary<string, int>();

		public DecorationModelDefinition[] DecorationModelDefinitions = new DecorationModelDefinition[0];

		public string[] BoneNames = new string[0];

		public Matrix4x4[] BindPose;

		public Avatar UnityAvatar;

		public RendererProperties RenderProperties;

		public DecorationModelDefinition GetDecorationModelDefinition(DDecorationPart part)
		{
			return GetDecorationModelDefinition(part.Name);
		}

		public DecorationModelDefinition GetDecorationModelDefinition(string name)
		{
			int value;
			if (DecorationIndexLookup.TryGetValue(name.ToLower(), out value))
			{
				return DecorationModelDefinitions[value];
			}
			Log.LogError(this, "Could not find equipment definition for " + name);
			return null;
		}

		public void OnEnable()
		{
			UpdateLookups();
		}

		public void UpdateLookups()
		{
			DecorationIndexLookup.Clear();
			for (int i = 0; i < DecorationModelDefinitions.Length; i++)
			{
				DecorationIndexLookup[DecorationModelDefinitions[i].Name.ToLower()] = i;
			}
			BoneIndexLookup.Clear();
			for (int i = 0; i < BoneNames.Length; i++)
			{
				BoneIndexLookup[BoneNames[i]] = i;
			}
		}
	}
}
