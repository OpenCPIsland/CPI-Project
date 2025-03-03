using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class ScavengerHuntRoleIndicator : MonoBehaviour
	{
		private const int HIDER_INDEX = 0;

		private const int FINDER_INDEX = 1;

		public Image InnerCircleImage;

		public SpriteSelector ImageSpriteSelector;

		public Color[] OuterCircleColors;

		private long userId;

		private ScavengerHuntData scavengerHunData;

		public void Init(long userId, int introTimeInSeconds, ScavengerHuntData scavengerHunData)
		{
			this.userId = userId;
			this.scavengerHunData = scavengerHunData;
			CoroutineRunner.Start(randomizeRoles(introTimeInSeconds), this, "randomizeRoles");
		}

		private IEnumerator randomizeRoles(float waitTime)
		{
			int currentSecondCount = 0;
			bool isRoleViewHiding = userId == scavengerHunData.LocalPlayerSessionId;
			WaitForSeconds waitASecondYield = new WaitForSeconds(0.5f);
			for (; (float)currentSecondCount < waitTime; currentSecondCount++)
			{
				int rndindex = (!isRoleViewHiding) ? 1 : 0;
				isRoleViewHiding = !isRoleViewHiding;
				InnerCircleImage.color = OuterCircleColors[rndindex];
				ImageSpriteSelector.SelectSprite(rndindex);
				yield return waitASecondYield;
			}
			int index = (!isHider()) ? 1 : 0;
			InnerCircleImage.color = OuterCircleColors[index];
			ImageSpriteSelector.SelectSprite(index);
		}

		private bool isHider()
		{
			if (userId == scavengerHunData.LocalPlayerSessionId)
			{
				return scavengerHunData.LocalPlayerRole == ScavengerHunt.ScavengerHuntRoles.Hider;
			}
			return scavengerHunData.OtherPlayerRole == ScavengerHunt.ScavengerHuntRoles.Hider;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
