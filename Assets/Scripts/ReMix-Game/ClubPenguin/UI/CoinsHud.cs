using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class CoinsHud : MonoBehaviour
	{
		public enum CoinHudState
		{
			closed,
			opening,
			addingCoins,
			waitingToClose
		}

		private const string ANIMATOR_IS_SHOWING = "isShown";

		private const string ANIMATOR_IS_ADDING = "isCoinAdded";

		private const int CANVAS_ORDER_TOP = 15;

		private const int CANVAS_ORDER_DEFAULT = 0;

		private const float ADDITION_DELAY = 0.025f;

		private const float MAX_ANIM_TIME = 3f;

		private const int COIN_EMIT_MAX = 5;

		private const string LOOP_SOUND_EVENT = "SFX/UI/Coin/CoinsLoop";

		public Image CoinImage;

		public Image TextImage;

		public Text CoinText;

		public ParticleSystem CoinParticles;

		public float CloseTime = 1f;

		private int remainingCoins;

		private Animator coinAnimator;

		private int coinCount;

		private CoinHudState state;

		private bool isSuppressed;

		private CoinsData coinsData;

		private CPDataEntityCollection dataEntityCollection;

		private CanvasRenderer rendImageIcon;

		private CanvasRenderer rendImageText;

		private CanvasRenderer rendText;

		private EventDispatcher eventDispatcher;

		private EventChannel eventChannel;

		public CoinHudState State
		{
			get
			{
				return state;
			}
		}

		public event Action<GameObject> HudOpened;

		public event Action<GameObject> HudClosed;

		public void Awake()
		{
			rendImageIcon = CoinImage.GetComponent<CanvasRenderer>();
			rendImageText = TextImage.GetComponent<CanvasRenderer>();
			rendText = CoinText.GetComponent<CanvasRenderer>();
			setVisible(false);
		}

		public void Start()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
			state = CoinHudState.closed;
			coinAnimator = GetComponent<Animator>();
			remainingCoins = 0;
			base.gameObject.SetActive(false);
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			if (!localPlayerHandle.IsNull && dataEntityCollection.HasComponent<PresenceData>(localPlayerHandle))
			{
				setUpCoinCount(localPlayerHandle);
			}
			else
			{
				eventDispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerAdded);
			}
			eventDispatcher.AddListener<HudEvents.SuppressCoinDisplay>(onSuppressCoinDisplay);
			eventDispatcher.AddListener<HudEvents.UnsuppressCoinDisplay>(onUnsuppressCoinDisplay);
		}

		public void OnDestroy()
		{
			stopLoopAudio();
			CoroutineRunner.StopAllForOwner(this);
			if (coinsData != null)
			{
				coinsData.OnCoinsAdded -= onAddCoins;
				coinsData.OnCoinsChanged -= onCoinsChanged;
				coinsData.OnCoinsSet -= onCoinsSet;
			}
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		private bool onLocalPlayerAdded(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			eventDispatcher.RemoveListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerAdded);
			if (!evt.Handle.IsNull)
			{
				setUpCoinCount(evt.Handle);
			}
			return false;
		}

		private void setUpCoinCount(DataEntityHandle localPlayerHandle)
		{
			coinsData = dataEntityCollection.GetComponent<CoinsData>(localPlayerHandle);
			coinCount = coinsData.Coins;
			CoinText.text = coinCount.ToString();
			coinsData.OnCoinsAdded += onAddCoins;
			coinsData.OnCoinsChanged += onCoinsChanged;
			coinsData.OnCoinsSet += onCoinsSet;
		}

		public void OnCoinIntroAnimationComplete()
		{
			CoroutineRunner.Start(updateCoinDisplay(), this, "updateCoinDisplay");
		}

		public void OnCoinOutroAnimationComplete()
		{
			if (this.HudClosed != null)
			{
				this.HudClosed(base.gameObject);
			}
			base.gameObject.SetActive(false);
		}

		private void onAddCoins(int coinsAdded, bool isCollectible)
		{
			if (isSuppressed)
			{
				return;
			}
			switch (state)
			{
			case CoinHudState.closed:
				if (!isCollectible)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new PlayerScoreEvents.ShowPlayerScore(dataEntityCollection.LocalPlayerSessionId, string.Format("+{0}", coinsAdded), PlayerScoreEvents.ParticleType.Coins));
				}
				showCoinHud();
				remainingCoins += coinsAdded;
				state = CoinHudState.opening;
				break;
			case CoinHudState.opening:
			case CoinHudState.addingCoins:
				remainingCoins += coinsAdded;
				break;
			case CoinHudState.waitingToClose:
				CancelInvoke();
				remainingCoins += coinsAdded;
				CoroutineRunner.Start(updateCoinDisplay(), this, "updateXpDisplay");
				break;
			}
		}

		private void onCoinsSet(int coins)
		{
			coinCount = coins;
			CoinText.text = coins.ToString();
		}

		private void onCoinsChanged(int newCoinCount)
		{
			if (newCoinCount <= coinCount)
			{
				coinCount = newCoinCount;
				CoinText.text = coinCount.ToString();
			}
		}

		private IEnumerator updateCoinDisplay()
		{
			eventDispatcher.DispatchEvent(default(HudEvents.CoinAdditionStart));
			state = CoinHudState.addingCoins;
			showCoinAdding();
			startLoopAudio();
			float steps = 120f;
			if ((float)remainingCoins < steps)
			{
				while (remainingCoins > 0)
				{
					coinCount++;
					remainingCoins--;
					CoinText.text = coinCount.ToString();
					CoinParticles.Emit(5);
					yield return new WaitForSeconds(0.025f);
				}
			}
			else
			{
				int coinGroup = Mathf.CeilToInt((float)remainingCoins / steps);
				coinCount += coinGroup;
				remainingCoins -= coinGroup;
				while (remainingCoins > 0)
				{
					if (remainingCoins < coinGroup)
					{
						coinCount += remainingCoins;
						remainingCoins = 0;
					}
					else
					{
						coinCount += coinGroup;
						remainingCoins -= coinGroup;
					}
					CoinText.text = coinCount.ToString();
					CoinParticles.Emit(Mathf.Min(coinGroup, 5));
					yield return new WaitForSeconds(0.025f);
				}
			}
			stopCoinAdding();
			stopLoopAudio();
			state = CoinHudState.waitingToClose;
			eventDispatcher.DispatchEvent(default(HudEvents.CoinAdditionStop));
			Invoke("hideCoinHud", CloseTime);
		}

		private void showCoinAdding()
		{
			coinAnimator.SetBool("isCoinAdded", true);
		}

		private void stopCoinAdding()
		{
			coinAnimator.SetBool("isCoinAdded", false);
		}

		private void showCoinHud()
		{
			setVisible(true);
			base.transform.localScale = Vector3.one;
			CoinImage.transform.localRotation = Quaternion.identity;
			base.gameObject.SetActive(true);
			coinAnimator.SetBool("isShown", true);
			if (state == CoinHudState.closed && this.HudOpened != null)
			{
				this.HudOpened(base.gameObject);
			}
		}

		private void hideCoinHud()
		{
			state = CoinHudState.closed;
			coinAnimator.SetBool("isShown", false);
		}

		private bool onSuppressCoinDisplay(HudEvents.SuppressCoinDisplay evt)
		{
			isSuppressed = true;
			return false;
		}

		private bool onUnsuppressCoinDisplay(HudEvents.UnsuppressCoinDisplay evt)
		{
			isSuppressed = false;
			return false;
		}

		private void setVisible(bool isVisible)
		{
			if (rendImageIcon != null)
			{
				rendImageIcon.SetAlpha(isVisible ? 1 : 0);
			}
			if (rendImageText != null)
			{
				rendImageText.SetAlpha(isVisible ? 1 : 0);
			}
			if (rendText != null)
			{
				rendText.SetAlpha(isVisible ? 1 : 0);
			}
		}

		private void startLoopAudio()
		{
			if (EventManager.Instance != null && this != null)
			{
				EventManager.Instance.PostEvent("SFX/UI/Coin/CoinsLoop", EventAction.PlaySound, this);
			}
		}

		private void stopLoopAudio()
		{
			if (EventManager.Instance != null && this != null)
			{
				EventManager.Instance.PostEvent("SFX/UI/Coin/CoinsLoop", EventAction.StopSound, this);
			}
		}
	}
}
