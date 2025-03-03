using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class CoinsDisplay : MonoBehaviour
	{
		public ParticleSystem CoinParticles;

		[SerializeField]
		private Text coinCountText;

		[SerializeField]
		private float TOTAL_ANIM_TIME = 1f;

		[SerializeField]
		private float TIME_PER_TICK = 0.1f;

		[SerializeField]
		private int PARTICLES_PER_TICK = 2;

		private int coinCount;

		private CoinsData coinsData;

		private void Start()
		{
			int coinText = 0;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (!cPDataEntityCollection.LocalPlayerHandle.IsNull && cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out coinsData))
			{
				coinText = coinsData.Coins;
				coinsData.OnCoinsChanged += onCoinDataChanged;
			}
			setCoinText(coinText);
		}

		private void OnDestroy()
		{
			if (coinsData != null)
			{
				coinsData.OnCoinsChanged -= onCoinDataChanged;
			}
			CoroutineRunner.StopAllForOwner(this);
		}

		private void onCoinDataChanged(int newCount)
		{
			int coinDelta = newCount - coinCount;
			CoroutineRunner.StopAllForOwner(this);
			CoroutineRunner.Start(animateCoinDelta(coinDelta), this, "");
		}

		private void setCoinText(int newCount)
		{
			coinCountText.text = newCount.ToString();
			coinCount = newCount;
		}

		private IEnumerator animateCoinDelta(int coinDelta)
		{
			int coinAnimTickCount = (int)Math.Round(TOTAL_ANIM_TIME / TIME_PER_TICK);
			int coinDeltaPerTick = coinDelta / coinAnimTickCount;
			for (int i = 0; i < coinAnimTickCount; i++)
			{
				yield return new WaitForSeconds(TIME_PER_TICK);
				if (coinDelta > 0 && CoinParticles != null)
				{
					CoinParticles.Emit(PARTICLES_PER_TICK);
				}
				setCoinText(coinCount + coinDeltaPerTick);
			}
			setCoinText(coinsData.Coins);
		}
	}
}
