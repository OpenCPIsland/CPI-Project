using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	[JsonConverter(typeof(RewardJsonConverter))]
	public class Reward : IEnumerable<IRewardable>, IEnumerable
	{
		private class RewardJsonConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(Reward);
			}

			public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}

			public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, JsonSerializer serializer)
			{
				serializer.Serialize(writer, RewardJsonWritter.FromReward((Reward)value));
			}
		}

		private Dictionary<string, IRewardable> rewardables;

		static Reward()
		{
			JsonMapper.RegisterExporter<Reward>(exportRewardAsRewardJsonWritter);
		}

		public Reward()
		{
			rewardables = new Dictionary<string, IRewardable>();
		}

		public IEnumerator<IRewardable> GetEnumerator()
		{
			return rewardables.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(IRewardable item)
		{
			if (rewardables.ContainsKey(item.RewardType))
			{
				rewardables[item.RewardType].Add(item);
			}
			else
			{
				rewardables.Add(item.RewardType, item);
			}
		}

		public bool TryGetValue<T>(out T rewardable) where T : IRewardable
		{
			string key = RewardableLoader.RewardableIdentifierMap[typeof(T)];
			if (rewardables.ContainsKey(key))
			{
				rewardable = (T)rewardables[key];
				return true;
			}
			rewardable = default(T);
			return false;
		}

		public void ClearReward(Type type)
		{
			string key = RewardableLoader.RewardableIdentifierMap[type];
			rewardables.Remove(key);
		}

		public void AddReward(Reward reward)
		{
			foreach (IRewardable item in reward)
			{
				Add(item);
			}
		}

		public virtual bool isEmpty()
		{
			using (IEnumerator<IRewardable> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IRewardable current = enumerator.Current;
					if (!current.IsEmpty())
					{
						return false;
					}
				}
			}
			return true;
		}

		private static void exportRewardAsRewardJsonWritter(Reward value, LitJson.JsonWriter writer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			LitJson.JsonWriter writer2 = new LitJson.JsonWriter(stringBuilder);
			JsonMapper.ToJson(RewardJsonWritter.FromReward(value), writer2);
			writer.WriteRaw(stringBuilder.ToString());
		}
	}
}
