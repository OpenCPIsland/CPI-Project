using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class CoinsForChangeCounter : MonoBehaviour
	{
		public Text CoinCountText;

		public TextMesh CoinCountTextMesh;

		public Text[] CoinCountDigits;

		public GameObject BrokenState;

		public CoinsForChangeTracker Tracker;

		[Tooltip("In coins per second")]
		public int CoinCountSpeed = 25;

		private long currentCoinCount;

		private long targetCoinCount;

		private float coinsToAdd;

		private bool isFirstUpdate = true;

		private bool isMaxed = false;

		private void Start()
		{
			if (Tracker != null)
			{
				SetCoinCount(Tracker.CurrentCoinCount, true);
				CoinsForChangeTracker tracker = Tracker;
				tracker.OnCoinCountUpdated = (Action<long>)Delegate.Combine(tracker.OnCoinCountUpdated, new Action<long>(onTotalCoinsUpdated));
			}
		}

		public void SetTracker(CoinsForChangeTracker tracker)
		{
			if (!(Tracker != null))
			{
				if (tracker == null)
				{
					base.enabled = false;
					return;
				}
				Tracker = tracker;
				SetCoinCount(Tracker.CurrentCoinCount, true);
				CoinsForChangeTracker tracker2 = Tracker;
				tracker2.OnCoinCountUpdated = (Action<long>)Delegate.Combine(tracker2.OnCoinCountUpdated, new Action<long>(onTotalCoinsUpdated));
			}
		}

		private void OnDestroy()
		{
			if (Tracker != null)
			{
				CoinsForChangeTracker tracker = Tracker;
				tracker.OnCoinCountUpdated = (Action<long>)Delegate.Remove(tracker.OnCoinCountUpdated, new Action<long>(onTotalCoinsUpdated));
			}
		}

		private void Update()
		{
			if (isMaxed)
			{
				return;
			}
			if (currentCoinCount > targetCoinCount)
			{
				SetCoinCount(targetCoinCount, true);
			}
			else if (currentCoinCount < targetCoinCount)
			{
				coinsToAdd += (float)CoinCountSpeed * Time.deltaTime;
				int num = (int)coinsToAdd;
				currentCoinCount += num;
				coinsToAdd -= num;
				if (currentCoinCount > targetCoinCount)
				{
					currentCoinCount = targetCoinCount;
					coinsToAdd = 0f;
				}
				setCoinCountText(currentCoinCount);
			}
		}

		private void onTotalCoinsUpdated(long totalCoinCount)
		{
			if (!isMaxed)
			{
				SetCoinCount(totalCoinCount, isFirstUpdate);
				isFirstUpdate = false;
			}
		}

		private void SetCoinCount(long count, bool immediate = false)
		{
			if (immediate)
			{
				currentCoinCount = count;
				targetCoinCount = count;
				setCoinCountText(currentCoinCount);
			}
			else
			{
				targetCoinCount = count;
			}
		}

		private void setCoinCountText(long count)
		{
			if (CoinCountText != null)
			{
				CoinCountText.text = count.ToString();
				return;
			}
			if (CoinCountTextMesh != null)
			{
				CoinCountTextMesh.text = count.ToString();
				return;
			}
			long num = count;
			for (int num2 = CoinCountDigits.Length - 1; num2 >= 0; num2--)
			{
				CoinCountDigits[num2].text = ((num == 0) ? "0" : (num % 10).ToString());
				num /= 10;
			}
			if (num != 0)
			{
				if (BrokenState != null)
				{
					BrokenState.SetActive(true);
				}
				for (int num2 = CoinCountDigits.Length - 1; num2 >= 0; num2--)
				{
					CoinCountDigits[num2].text = "9";
				}
				isMaxed = true;
				coinsToAdd = 0f;
			}
		}
	}
}
