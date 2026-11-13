using System;
using UnityEngine;
using UnityEngine.Rendering;

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
			Debug.Log($"[ConditionDefinition_Memory] Type={MemoryType}, Reported={memoryToCheck} MB, Threshold={LessThanEqualToMemory} MB, Satisfied={memoryToCheck <= LessThanEqualToMemory}");
			return memoryToCheck <= LessThanEqualToMemory;
		}

		private int getMemoryToCheck()
		{
			switch (MemoryType)
			{
			case MemoryTypeEnum.SYSTEM:
				return SystemInfo.systemMemorySize;
			case MemoryTypeEnum.GRAPHIC:
				return GetReliableGraphicsMemorySize();
			default:
				throw new NotImplementedException("Unrecognised Memory Type");
			}
		}

		private int GetReliableGraphicsMemorySize()
		{
			int reported = SystemInfo.graphicsMemorySize;

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (reported <= 512 && SystemInfo.systemMemorySize > 4096)
			{
				bool isVulkan = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan;
				bool isOpenGL = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore;
				if (isVulkan || isOpenGL)
				{
					Debug.LogWarning($"[ConditionDefinition_Memory] Linux VRAM under-report detected: " +
						$"graphicsMemorySize={reported} MB (likely incorrect). " +
						$"GraphicsAPI={SystemInfo.graphicsDeviceType}, GPU={SystemInfo.graphicsDeviceName}. " +
						$"Overriding to max int to ensure correct quality tier selection.");
					return int.MaxValue;
				}
			}
#endif
			return reported;
		}
	}
}
