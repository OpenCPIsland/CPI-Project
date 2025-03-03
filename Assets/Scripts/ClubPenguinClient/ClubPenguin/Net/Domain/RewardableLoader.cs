using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ClubPenguin.Net.Domain
{
	public class RewardableLoader
	{
		private static Dictionary<string, Type> rewardableTypeMap;

		private static Dictionary<Type, string> rewardableIdentifierMap;

		public static Dictionary<string, Type> RewardableTypeMap
		{
			get
			{
				if (rewardableTypeMap == null)
				{
					initMaps();
				}
				return rewardableTypeMap;
			}
		}

		public static Dictionary<Type, string> RewardableIdentifierMap
		{
			get
			{
				if (rewardableIdentifierMap == null)
				{
					initMaps();
				}
				return rewardableIdentifierMap;
			}
		}

		public static IRewardable GenerateRewardable(string identifier)
		{
			return GenerateRewardable(RewardableTypeMap[identifier]);
		}

		public static IRewardable GenerateRewardable(Type type)
		{
			return (IRewardable)Activator.CreateInstance(type);
		}

		public static IRewardable GenerateRewardable(string identifier, object initValue)
		{
			return (IRewardable)Activator.CreateInstance(RewardableTypeMap[identifier], initValue);
		}

		public static IRewardable GenerateRewardable(Type type, object initValue)
		{
			return (IRewardable)Activator.CreateInstance(type, initValue);
		}

		public static void Init(Type[] rewardables)
		{
			rewardableTypeMap = new Dictionary<string, Type>();
			rewardableIdentifierMap = new Dictionary<Type, string>();
			int num = rewardables.Length;
			for (int i = 0; i < num; i++)
			{
				IRewardable rewardable = GenerateRewardable(rewardables[i]);
				rewardableTypeMap.Add(rewardable.RewardType, rewardables[i]);
				rewardableIdentifierMap.Add(rewardables[i], rewardable.RewardType);
			}
		}

		private static void initMaps()
		{
			if (Application.isPlaying)
			{
				Log.LogError(typeof(RewardableLoader), "RewardableLoader rewableable types were not initialized properly.");
			}
			rewardableTypeMap = new Dictionary<string, Type>();
			rewardableIdentifierMap = new Dictionary<Type, string>();
			Type typeFromHandle = typeof(IRewardable);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			int num = assemblies.Length;
			for (int i = 0; i < num; i++)
			{
				Assembly assembly = assemblies[i];
				string name = assembly.GetName().Name;
				if (name.StartsWith("System") || name.StartsWith("Mono.") || name.StartsWith("UnityEngine") || name.StartsWith("UnityEditor") || name.StartsWith("Boo.") || name.StartsWith("Fabric."))
				{
					continue;
				}
				Type[] exportedTypes = assembly.GetExportedTypes();
				int num2 = exportedTypes.Length;
				for (int j = 0; j < num2; j++)
				{
					Type type = exportedTypes[j];
					if (typeFromHandle.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
					{
						IRewardable rewardable = GenerateRewardable(type);
						rewardableTypeMap.Add(rewardable.RewardType, type);
						rewardableIdentifierMap.Add(type, rewardable.RewardType);
					}
				}
			}
		}
	}
}
