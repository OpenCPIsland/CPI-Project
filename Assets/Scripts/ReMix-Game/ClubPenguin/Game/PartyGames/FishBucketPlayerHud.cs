using Disney.Kelowna.Common;
using Fabric;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class FishBucketPlayerHud : MonoBehaviour
	{
		private const int DEFAULT_SELECTOR_INDEX = 0;

		private const int LOCAL_PLAYER_SELECTOR_INDEX = 1;

		public Text CountText;

		public SpriteSelector BucketSpriteSelector;

		public TintSelector BgTintSelector;

		public GameObject ActiveOutline;

		public Text PlayerNameText;

		public float ScoreUpdateDelay = 1.5f;

		[Space(10f)]
		public string PointIncreaseSFXTrigger;

		public string PointDecreaseSFXTrigger;

		private int currentCount;

		private int targetCount;

		private bool isLocalPlayer;

		private void Awake()
		{
			CountText.text = currentCount.ToString();
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		private void OnDisable()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		public void Init(FishBucket.FishBucketPlayerData playerData)
		{
			BucketSpriteSelector.SelectSprite(playerData.PlayerNum);
			PlayerNameText.text = playerData.DisplayName;
			isLocalPlayer = playerData.IsLocalPlayer;
			if (isLocalPlayer)
			{
				ActiveOutline.GetComponent<TintSelector>().SelectColor(1);
			}
		}

		public void SetHighlighted(bool highlighted)
		{
			ActiveOutline.SetActive(highlighted);
			if (isLocalPlayer)
			{
				if (highlighted)
				{
					CountText.GetComponent<TintSelector>().SelectColor(1);
					BgTintSelector.SelectColor(1);
				}
				else
				{
					CountText.GetComponent<TintSelector>().SelectColor(0);
					BgTintSelector.SelectColor(0);
				}
			}
		}

		public void SetInactive()
		{
			ActiveOutline.SetActive(false);
			CountText.gameObject.SetActive(false);
			BucketSpriteSelector.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
			BgTintSelector.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
		}

		public void ChangeScore(int scoreDelta)
		{
			targetCount += scoreDelta;
			if (targetCount < 0)
			{
				targetCount = 0;
			}
			CoroutineRunner.Start(updateScore(), this, "UpdateFishBucketScore");
		}

		private IEnumerator updateScore()
		{
			yield return new WaitForSeconds(ScoreUpdateDelay);
			iTween.ScaleTo(CountText.gameObject, new Vector3(1.35f, 1.35f, 1f), 0.1f);
			yield return new WaitForSeconds(0.2f);
			while (currentCount != targetCount)
			{
				yield return new WaitForSeconds(0.2f);
				bool countingDown = currentCount > targetCount;
				currentCount = (countingDown ? (--currentCount) : (++currentCount));
				CountText.text = currentCount.ToString();
				string countSFXTrigger = countingDown ? PointDecreaseSFXTrigger : PointIncreaseSFXTrigger;
				EventManager.Instance.PostEvent(countSFXTrigger, EventAction.PlaySound);
			}
			yield return new WaitForSeconds(0.2f);
			iTween.ScaleTo(CountText.gameObject, new Vector3(1f, 1f, 1f), 0.1f);
		}
	}
}
