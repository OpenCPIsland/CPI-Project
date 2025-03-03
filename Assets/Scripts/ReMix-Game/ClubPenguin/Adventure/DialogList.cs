using DevonLocalization.Core;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu]
	public class DialogList : ScriptableObject
	{
		[Serializable]
		public struct Entry
		{
			[LocalizationToken]
			[FormerlySerializedAs("Content")]
			public string ContentToken;

			public int Weight;

			public string AudioEventName;

			public bool AdvanceSequence;

			public string DialogAnimationTrigger;

			public string DialogAnimationEndTrigger;
		}

		public Entry[] Entries;

		private int[] weights;

		public void OnEnable()
		{
			if (Entries != null && Entries.Length > 0)
			{
				weights = new int[Entries.Length];
				weights[0] = Math.Max(Entries[0].Weight, 1);
				for (int i = 1; i < weights.Length; i++)
				{
					weights[i] = Math.Max(Entries[i].Weight, 1) + weights[i - 1];
				}
			}
		}

		public Entry SelectRandom()
		{
			Entry result = Entries[Entries.Length - 1];
			if (weights != null)
			{
				int max = weights[weights.Length - 1];
				int num = UnityEngine.Random.Range(0, max);
				for (int i = 0; i < weights.Length - 1; i++)
				{
					if (num < weights[i])
					{
						result = Entries[i];
						break;
					}
				}
			}
			return result;
		}
	}
}
