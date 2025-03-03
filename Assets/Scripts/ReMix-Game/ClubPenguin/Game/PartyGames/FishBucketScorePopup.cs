using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class FishBucketScorePopup : MonoBehaviour
	{
		public Text ScoreText;

		public Color PositiveColor;

		public Color NegativeColor;

		public void SetScore(int scoreDelta)
		{
			string text = "";
			text = ((scoreDelta <= 0) ? scoreDelta.ToString() : string.Format("+{0}", scoreDelta));
			ScoreText.text = text;
			ScoreText.GetComponent<Outline>().effectColor = ((scoreDelta > 0) ? PositiveColor : NegativeColor);
		}
	}
}
