using ClubPenguin.Tubes;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Tube : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		private TubeDefinition definition;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			int tubeId = (!(reward.UnlockID is string)) ? ((int)reward.UnlockID) : int.Parse((string)reward.UnlockID);
			loadTubeIcon(getIconPath(tubeId));
		}

		private string getIconPath(int tubeId)
		{
			Dictionary<int, TubeDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TubeDefinition>>();
			string key = RewardPopupConstants.DefaultIconContentKey.Key;
			if (dictionary.TryGetValue(tubeId, out definition))
			{
				key = definition.IconContentKey.Key;
			}
			return key;
		}

		private void loadTubeIcon(string iconPath)
		{
			Content.LoadAsync<Sprite>(iconPath, onloadTubeIconComplete);
		}

		private void onloadTubeIconComplete(string path, Sprite icon)
		{
			callback(icon, null, definition.DisplayNameToken);
		}
	}
}
