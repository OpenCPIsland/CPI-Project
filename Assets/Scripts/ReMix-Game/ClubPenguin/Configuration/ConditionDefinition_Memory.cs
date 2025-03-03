using System;
using UnityEngine;

namespace ClubPenguin.Configuration
{
	[CreateAssetMenu(menuName = "Conditional/Condition/Memory")]
	public class ConditionDefinition_Memory : ConditionDefinition
	{
		public enum MemoryTypeEnum
		{
			SYSTEM,
			GRAPHIC
		}

		public MemoryTypeEnum MemoryType;

		public int LessThanEqualToMemory;

		public override bool IsSatisfied()
		{
			int memoryToCheck = getMemoryToCheck();
			return memoryToCheck <= LessThanEqualToMemory;
		}

		private int getMemoryToCheck()
		{
			int num = 0;
			switch (MemoryType)
			{
			case MemoryTypeEnum.SYSTEM:
				return SystemInfo.systemMemorySize;
			case MemoryTypeEnum.GRAPHIC:
				return SystemInfo.graphicsMemorySize;
			default:
				throw new NotImplementedException("Unrecognised Memory Type");
			}
		}
	}
}
