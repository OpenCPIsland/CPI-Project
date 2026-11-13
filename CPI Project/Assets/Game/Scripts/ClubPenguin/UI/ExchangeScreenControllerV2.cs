using ClubPenguin.Analytics;
using ClubPenguin.Net;
using ClubPenguin.Tutorial;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ExchangeScreenControllerV2 : MonoBehaviour, IBaseNetworkErrorHandler
	{
		private enum ExchangeScreenState
		{
			Idle,
			Exchanging,
			Complete
		}

		public ExchangeScreenNumberRoll NumberRoll;

		public ExchangeScreenInventory Inventory;

		public Button ExchangeButton;

		public GameObject ProgressBar;

		public GameObject[] Tubes;

		public ParticleSystem[] CoinParticles;

		public ParticleSystem[] TubeParticles;

		public ParticleSystem ButtonParticles;

		public ParticleSystem CoinBurstParticles;

		public Animator CounterAnimator;

		public float CoinAnimTimeRatio = 0.1f;

		public float MinAnimTime = 10f;

		public float MaxAnimTime = 10f;

		public float TubeAnimDelay = 0.1f;

		public float InventoryAnimDelay = 0.5f;

		public float ReelAnimDelay = 0.5f;

		public float EndExchangeAnimDelay = 0.5f;

		public TutorialDefinitionKey Collect1TutorialDefinition;

		public TutorialDefinitionKey Collect2TutorialDefinition;

		public TutorialDefinitionKey ExchangeTutorialDefinition;

		private ExchangeScreenState currentState;

		private IRewardService rewardService;

		private Text exchangeButtonText;

		private Image progressBarImage;

		private float elapsedAnimTime = 0f;

		private float totalAnimTime = 0f;

		private int coinsToCollect = 0;

		private Animator animator;

		public void Awake()
		{
			animator = GetComponent<Animator>();
			exchangeButtonText = ExchangeButton.transform.Find("Text").GetComponent<Text>();
			progressBarImage = ProgressBar.transform.Find("ProgressFill").Find("Image").GetComponent<Image>();
		}

		public void Start()
		{
			rewardService = Service.Get<INetworkServicesManager>().RewardService;
			toggleExchangeAnimParticles(false);
			ButtonParticles.Stop();
			CoinBurstParticles.Stop(true);
			Inventory.ItemsCreated += onInventoryItemsCreated;
			Service.Get<TutorialManager>().TryStartTutorial(ExchangeTutorialDefinition.Id);
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Exchange));
			Service.Get<ICPSwrveService>().Action("game.exchange", "open");
		}

		public void OnDestroy()
		{
			Inventory.ItemsCreated -= onInventoryItemsCreated;
			Service.Get<EventDispatcher>().DispatchEvent(new AwayFromKeyboardEvent(AwayFromKeyboardStateType.Here));
		}

		public void Update()
		{
			if (currentState == ExchangeScreenState.Exchanging && totalAnimTime > 0f)
			{
				float num = 0f;
				elapsedAnimTime += Time.deltaTime;
				if (elapsedAnimTime >= totalAnimTime)
				{
					num = 1f;
					changeState(ExchangeScreenState.Complete);
				}
				else
				{
					num = elapsedAnimTime / totalAnimTime;
				}
				progressBarImage.fillAmount = num;
			}
		}

		private void onInventoryItemsCreated()
		{
			if (Inventory.TotalValidCollectibles == 0)
			{
				disableExchange();
			}
		}

		private void disableExchange()
		{
			ExchangeButton.interactable = false;
			ExchangeButton.GetComponent<SpriteSelector>().SelectSprite(1);
			ExchangeButton.GetComponentInChildren<Text>().color = new Color(0.8f, 0.8f, 0.8f);
			ExchangeButton.GetComponent<Animator>().StopPlayback();
		}

		public void OnExchangeClick()
		{
			switch (currentState)
			{
			case ExchangeScreenState.Exchanging:
				break;
			case ExchangeScreenState.Idle:
				changeState(ExchangeScreenState.Exchanging);
				Service.Get<ICPSwrveService>().Action("game.exchange", "tap_trade");
				break;
			case ExchangeScreenState.Complete:
				Service.Get<ICPSwrveService>().Action("game.exchange", "tap_claim");
				rewardService.ExchangeAllForCoins();
				ButtonParticles.Stop();
				logExchangeData();
				closeExchange();
				break;
			}
		}

		private void changeState(ExchangeScreenState state)
		{
			switch (state)
			{
			case ExchangeScreenState.Exchanging:
				if (currentState == ExchangeScreenState.Idle)
				{
					rewardService.CalculateExchangeAllForCoins(this);
					Service.Get<EventDispatcher>().AddListener<RewardServiceEvents.MyRewardCalculated>(onMyRewardCalculated);
					ExchangeButton.gameObject.SetActive(false);
					ProgressBar.SetActive(true);
					CounterAnimator.SetTrigger("CounterEnabled");
					animator.SetTrigger("ExchangeLoop");
					progressBarImage.fillAmount = 0f;
					Service.Get<TutorialManager>().SetTutorial(Collect1TutorialDefinition.Id, true);
					Service.Get<TutorialManager>().SetTutorial(Collect2TutorialDefinition.Id, true);
				}
				break;
			case ExchangeScreenState.Complete:
				if (currentState == ExchangeScreenState.Exchanging)
				{
					CoroutineRunner.Start(stopExchangeAnimation(), this, "exchangeStopAnimationDelay");
					currentState = state;
				}
				break;
			}
		}

		private IEnumerator playExchangeAnimation()
		{
			yield return new WaitForSeconds(TubeAnimDelay);
			for (int i = 0; i < TubeParticles.Length; i++)
			{
				TubeParticles[i].Play();
			}
			yield return new WaitForSeconds(InventoryAnimDelay);
			Inventory.StartExchangeAnimation(totalAnimTime, Tubes);
			yield return new WaitForSeconds(ReelAnimDelay);
			NumberRoll.StartSpin();
			for (int i = 0; i < CoinParticles.Length; i++)
			{
				CoinParticles[i].Play();
			}
		}

		private IEnumerator stopExchangeAnimation()
		{
			yield return new WaitForSeconds(EndExchangeAnimDelay);
			if (coinsToCollect > 999)
			{
				CounterAnimator.SetTrigger("Broken");
				NumberRoll.StopSpin(999);
			}
			else
			{
				NumberRoll.StopSpin(coinsToCollect);
			}
			ExchangeButton.gameObject.SetActive(true);
			ProgressBar.SetActive(false);
			exchangeButtonText.text = Service.Get<Localizer>().GetTokenTranslation("Exchange.Claim.Text");
			toggleExchangeAnimParticles(false);
			ButtonParticles.Play();
			CoinBurstParticles.Play(true);
		}

		public void OnCloseClick()
		{
			if (currentState == ExchangeScreenState.Complete)
			{
				Service.Get<ICPSwrveService>().Action("game.exchange", "tap_claim");
				rewardService.ExchangeAllForCoins();
				ButtonParticles.Stop();
				logExchangeData();
			}
			closeExchange();
		}

		private void closeExchange()
		{
			animator.SetTrigger("Close");
		}

		public void OnIntroAnimationComplete()
		{
		}

		public void OnOutroAnimationComplete()
		{
			Object.Destroy(base.gameObject);
		}

		private bool onMyRewardCalculated(RewardServiceEvents.MyRewardCalculated evt)
		{
			coinsToCollect = evt.Coins;
			totalAnimTime = Mathf.Clamp((float)coinsToCollect * CoinAnimTimeRatio, MinAnimTime, MaxAnimTime);
			Service.Get<EventDispatcher>().RemoveListener<RewardServiceEvents.MyRewardCalculated>(onMyRewardCalculated);
			if (coinsToCollect == 0)
			{
				disableExchange();
				CoroutineRunner.Start(stopExchangeAnimation(), this, "exchangeStopAnimationDelay");
			}
			else
			{
				currentState = ExchangeScreenState.Exchanging;
				CoroutineRunner.Start(playExchangeAnimation(), this, "exchangeAnimationDelay");
			}
			return false;
		}

		public void onRequestTimeOut()
		{
			onMyRewardCalculated(new RewardServiceEvents.MyRewardCalculated(0));
		}

		public void onGeneralNetworkError()
		{
			onMyRewardCalculated(new RewardServiceEvents.MyRewardCalculated(0));
		}

		private void toggleExchangeAnimParticles(bool on)
		{
			if (on)
			{
				for (int i = 0; i < CoinParticles.Length; i++)
				{
					CoinParticles[i].Play();
				}
				for (int i = 0; i < TubeParticles.Length; i++)
				{
					TubeParticles[i].Play();
				}
			}
			else
			{
				for (int i = 0; i < CoinParticles.Length; i++)
				{
					CoinParticles[i].Stop();
				}
				for (int i = 0; i < TubeParticles.Length; i++)
				{
					TubeParticles[i].Stop();
				}
			}
		}

		private void logExchangeData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			ExchangeScreenInventoryItem[] componentsInChildren = GetComponentsInChildren<ExchangeScreenInventoryItem>(true);
			foreach (ExchangeScreenInventoryItem exchangeScreenInventoryItem in componentsInChildren)
			{
				if (exchangeScreenInventoryItem.ExchangeItem.CanExchange())
				{
					stringBuilder.Append(string.Format("{0}={1}|", exchangeScreenInventoryItem.ExchangeItem.CollectibleType, exchangeScreenInventoryItem.ExchangeItem.QuantityEarned));
				}
			}
			Service.Get<ICPSwrveService>().CoinsGiven(coinsToCollect, "exchanged", "exchange", stringBuilder.ToString());
		}
	}
}
