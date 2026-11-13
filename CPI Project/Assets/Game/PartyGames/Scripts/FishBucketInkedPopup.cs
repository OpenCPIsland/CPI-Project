using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class FishBucketInkedPopup : MonoBehaviour
	{
		public Text ScoreText;

		public void SetScore(int scoreDelta)
		{
			ScoreText.text = scoreDelta.ToString();
		}
	}
}
