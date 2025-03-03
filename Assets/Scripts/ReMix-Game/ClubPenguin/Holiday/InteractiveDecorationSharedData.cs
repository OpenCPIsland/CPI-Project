using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Holiday
{
	public class InteractiveDecorationSharedData : MonoBehaviour
	{
		public DecorationMPBData[] MasterBlocks;

		public DecorationGroupColorData[] ColorData;

		public DateUnityWrapper HideStartDate;

		public DateUnityWrapper HideEndDate;

		private Dictionary<string, MaterialPropertyBlock> sharedBlocks;

		private void Awake()
		{
			sharedBlocks = new Dictionary<string, MaterialPropertyBlock>();
			DecorationMPBData[] masterBlocks = MasterBlocks;
			foreach (DecorationMPBData decorationMPBData in masterBlocks)
			{
				if (decorationMPBData.SampleRenderer != null)
				{
					MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
					decorationMPBData.SampleRenderer.GetPropertyBlock(materialPropertyBlock);
					if (materialPropertyBlock != null)
					{
						if (!string.IsNullOrEmpty(decorationMPBData.GroupName))
						{
							if (!sharedBlocks.ContainsKey(decorationMPBData.GroupName))
							{
								sharedBlocks.Add(decorationMPBData.GroupName, materialPropertyBlock);
							}
							else
							{
								Log.LogError(this, string.Format("O_o\t Error: MasterBlocks on {0} has a duplicate block name entry", base.gameObject.GetPath()));
							}
						}
						else
						{
							Log.LogError(this, string.Format("O_o\t Error: MasterBlocks on {0} has a null block name entry", base.gameObject.GetPath()));
						}
					}
					else
					{
						Log.LogError(this, string.Format("O_o\t Error: {0} can't get a MaterialPropertyBlock for {1}", base.gameObject.GetPath(), decorationMPBData.SampleRenderer.gameObject.GetPath()));
					}
				}
				else
				{
					Log.LogError(this, string.Format("O_o\t Error: MasterBlocks on {0} has a null sample renderer entry", base.gameObject.GetPath()));
				}
			}
		}

		public MaterialPropertyBlock GetSharedBlock(string blockName)
		{
			MaterialPropertyBlock result = new MaterialPropertyBlock();
			if (sharedBlocks.ContainsKey(blockName))
			{
				result = sharedBlocks[blockName];
			}
			else
			{
				Log.LogError(this, string.Format("O_o\t Error: Can't find block named {0}", blockName));
			}
			return result;
		}

		public Dictionary<DecorationColor, Vector2> GetColorOffsetData(string groupName)
		{
			Dictionary<DecorationColor, Vector2> dictionary = new Dictionary<DecorationColor, Vector2>();
			DecorationGroupColorData[] colorData = ColorData;
			foreach (DecorationGroupColorData decorationGroupColorData in colorData)
			{
				if (decorationGroupColorData.GroupName == groupName)
				{
					DecorationColorData[] groupColors = decorationGroupColorData.GroupColors;
					foreach (DecorationColorData decorationColorData in groupColors)
					{
						dictionary.Add(decorationColorData.Color, decorationColorData.Offset);
					}
				}
			}
			return dictionary;
		}
	}
}
