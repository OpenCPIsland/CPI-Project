using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubPenguin
{
	[Serializable]
	public class MiniGamePlayCountData : ScopedData
	{
		private Dictionary<string, int> minigamePlayCounts = new Dictionary<string, int>();

		public Dictionary<string, int> MinigamePlayCounts
		{
			get
			{
				return minigamePlayCounts;
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(MiniGamePlayCountDataMonoBehaviour);
			}
		}

		public event Action<string, int> OnMinigamePlayCountChanged;

		public void SetMinigamePlayCount(string game, int count)
		{
			if (!minigamePlayCounts.ContainsKey(game))
			{
				minigamePlayCounts[game] = 0;
			}
			if (minigamePlayCounts[game] != count)
			{
				if (this.OnMinigamePlayCountChanged != null)
				{
					this.OnMinigamePlayCountChanged(game, count);
				}
				minigamePlayCounts[game] = count;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string key in minigamePlayCounts.Keys)
			{
				stringBuilder.Append(key);
				stringBuilder.Append(": ");
				stringBuilder.Append(minigamePlayCounts[key]);
				stringBuilder.Append(", ");
			}
			if (stringBuilder.Length == 0)
			{
				stringBuilder.Append("No minigame data found!");
			}
			return stringBuilder.ToString();
		}

		protected override void notifyWillBeDestroyed()
		{
			this.OnMinigamePlayCountChanged = null;
		}
	}
}
