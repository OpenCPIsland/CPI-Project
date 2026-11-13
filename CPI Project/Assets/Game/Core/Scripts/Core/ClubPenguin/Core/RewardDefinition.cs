using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	[JsonConverter(typeof(RewardDefinitionJsonConverter))]
	public abstract class RewardDefinition : ScriptableObject, IEnumerable<IRewardableDefinition>, IEnumerable, ICustomExportScriptableObject
	{
		public class RewardDefinitionJsonConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return typeof(RewardDefinition).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				serializer.Serialize(writer, ((RewardDefinition)value).ToReward());
			}
		}

		public abstract IEnumerator<IRewardableDefinition> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public List<T> GetDefinitions<T>() where T : IRewardableDefinition
		{
			Type typeFromHandle = typeof(T);
			List<T> list = new List<T>();
			using (IEnumerator<IRewardableDefinition> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IRewardableDefinition current = enumerator.Current;
					if (current.GetType() == typeFromHandle)
					{
						list.Add((T)current);
					}
				}
			}
			return list;
		}

		public Reward ToReward()
		{
			Reward reward = new Reward();
			using (IEnumerator<IRewardableDefinition> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IRewardableDefinition current = enumerator.Current;
					if (current == null)
					{
						Log.LogErrorFormatted(this, "Contains null reward definition {0}", current);
					}
					try
					{
						reward.Add(current.Reward);
					}
					catch (Exception ex)
					{
						Log.LogErrorFormatted(this, "Skipping bad reward component {0}, in {1}, throwing exception {2}", current, base.name, ex.Message);
						Log.LogException(this, ex);
					}
				}
			}
			return reward;
		}
	}
}
