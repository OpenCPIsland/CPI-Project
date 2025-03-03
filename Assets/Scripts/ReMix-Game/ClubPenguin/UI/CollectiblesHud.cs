using ClubPenguin.Collectibles;
using ClubPenguin.Core;
using ClubPenguin.Tutorial;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class CollectiblesHud : MonoBehaviour
	{
		public enum CollectibleHudState
		{
			none,
			closed,
			opening,
			adding,
			waitingToClose,
			reopen
		}

		private const string ANIMATOR_IS_SHOWING = "isShown";

		private const string ANIMATOR_IS_ADDING = "isCoinAdded";

		private const float MAX_ADDITION_DELAY = 0.5f;

		private const string COLLECTIBLES_TUTORIAL_COUNT_KEY = "CollectiblesTutorialCount";

		private const string COLLECTIBLES_TUTORIALS_SHOWN_KEY = "CollectiblesTutorialsShown";

		private const string COLLECTIBLES_SHOW_TUTORIAL_KEY = "CollectiblesShowTutorial";

		private const int COLLECTIBLE_TUTORIAL_SHOW_COUNT1 = 50;

		private const int COLLECTIBLE_TUTORIAL_SHOW_COUNT2 = 100;

		private const int COLLECTIBLE_TUTORIAL_SHOW_COUNT3 = 300;

		public Image CollectibleImage;

		public Image TextImage;

		public Text CountText;

		public ParticleSystem CollectibleParticles;

		public float CloseTime = 4f;

		private Animator animator;

		private int collectibleCount;

		private int remainingCollectibles;

		private CollectibleHudState state;

		private CollectibleDefinitionService definitionService;

		private CollectibleDefinition currentCollectibleDefinition;

		private Dictionary<string, Sprite> loadedSprites;

		private string previousType;

		private DataEntityHandle localPlayerHandle;

		private int numCollectiblesSinceTutorial;

		private int numTutorialsShown;

		private bool invokeActive = false;

		private bool coroutineActive = false;

		private CanvasRenderer rendImageIcon;

		private CanvasRenderer rendImageText;

		private CanvasRenderer rendText;

		public TutorialDefinitionKey CollectiblesTutorial;

		public bool ShouldShowCollectibleTutorial
		{
			get;
			set;
		}

		public CollectibleHudState State
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
			rendImageIcon = CollectibleImage.GetComponent<CanvasRenderer>();
			rendImageText = TextImage.GetComponent<CanvasRenderer>();
			rendText = CountText.GetComponent<CanvasRenderer>();
			setVisible(false);
		}

		public void Start()
		{
			state = CollectibleHudState.closed;
			animator = GetComponent<Animator>();
			definitionService = Service.Get<CollectibleDefinitionService>();
			loadedSprites = new Dictionary<string, Sprite>();
			previousType = "";
			base.gameObject.SetActive(false);
			Service.Get<EventDispatcher>().AddListener<CollectibleEvents.CollectibleAdd>(onAddCollectible);
			localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			if (PlayerPrefs.HasKey("CollectiblesTutorialCount"))
			{
				numCollectiblesSinceTutorial = PlayerPrefs.GetInt("CollectiblesTutorialCount");
			}
			if (PlayerPrefs.HasKey("CollectiblesTutorialsShown"))
			{
				numTutorialsShown = PlayerPrefs.GetInt("CollectiblesTutorialsShown");
			}
			if (PlayerPrefs.HasKey("CollectiblesShowTutorial"))
			{
				ShouldShowCollectibleTutorial = (PlayerPrefs.GetInt("CollectiblesShowTutorial") == 1);
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			Service.Get<EventDispatcher>().RemoveListener<CollectibleEvents.CollectibleAdd>(onAddCollectible);
			PlayerPrefs.SetInt("CollectiblesTutorialCount", numCollectiblesSinceTutorial);
			PlayerPrefs.SetInt("CollectiblesTutorialsShown", numTutorialsShown);
			PlayerPrefs.SetInt("CollectiblesShowTutorial", ShouldShowCollectibleTutorial ? 1 : 0);
		}

		public void OnCoinIntroAnimationComplete()
		{
			if (!coroutineActive)
			{
				CoroutineRunner.Start(updateCollectibleDisplay(), this, "updateCollectibleDisplay");
			}
		}

		public void OnCoinOutroAnimationComplete()
		{
			if (this.HudClosed != null)
			{
				this.HudClosed(base.gameObject);
			}
			base.gameObject.SetActive(false);
		}

		private bool onAddCollectible(CollectibleEvents.CollectibleAdd evt)
		{
			currentCollectibleDefinition = definitionService.Get(evt.Type);
			int amount = evt.Amount;
			if (evt.Type != previousType || state == CollectibleHudState.closed)
			{
				setCollectibleImage();
				remainingCollectibles = 0;
				CollectiblesData component;
				if (Service.Get<CPDataEntityCollection>().TryGetComponent(localPlayerHandle, out component))
				{
					collectibleCount = component.GetCollectibleTotal(evt.Type);
				}
			}
			switch (state)
			{
			case CollectibleHudState.closed:
				remainingCollectibles += amount;
				showCollectibleHud();
				state = CollectibleHudState.opening;
				break;
			case CollectibleHudState.opening:
			case CollectibleHudState.adding:
				remainingCollectibles += amount;
				if (remainingCollectibles > 1)
				{
					showCollectibleHud();
					state = CollectibleHudState.reopen;
				}
				break;
			case CollectibleHudState.reopen:
				remainingCollectibles += amount;
				if (!coroutineActive)
				{
					CoroutineRunner.Start(updateCollectibleDisplay(), this, "updateCollectibleDisplay");
					state = CollectibleHudState.adding;
				}
				break;
			case CollectibleHudState.waitingToClose:
				wrapperCancelInvoke();
				remainingCollectibles += amount;
				if (!coroutineActive)
				{
					CoroutineRunner.Start(updateCollectibleDisplay(), this, "updateCollectibleDisplay");
				}
				break;
			}
			previousType = evt.Type;
			if (!ShouldShowCollectibleTutorial)
			{
				numCollectiblesSinceTutorial += amount;
				checkTutorial();
			}
			return false;
		}

		private IEnumerator updateCollectibleDisplay()
		{
			coroutineActive = true;
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.CoinAdditionStart));
			state = CollectibleHudState.adding;
			showCollectibleAdding();
			if (remainingCollectibles == 1)
			{
				CollectibleParticles.Emit(2);
			}
			while (remainingCollectibles > 0)
			{
				collectibleCount++;
				remainingCollectibles--;
				CountText.text = collectibleCount.ToString();
				CollectibleParticles.Emit(1);
				if (remainingCollectibles > 0)
				{
					yield return new WaitForSeconds(Mathf.Min(1f / (float)remainingCollectibles, 0.5f));
				}
			}
			stopCollectibleAdding();
			state = CollectibleHudState.waitingToClose;
			wrapperInvoke("hideCollectibleHud", CloseTime);
			coroutineActive = false;
		}

		private void wrapperInvoke(string methodName, float invokeTime)
		{
			if (invokeActive)
			{
				CancelInvoke();
			}
			invokeActive = true;
			Invoke(methodName, invokeTime);
		}

		private void wrapperCancelInvoke()
		{
			invokeActive = false;
			CancelInvoke();
		}

		private void showCollectibleAdding()
		{
			animatorSetBool("isCoinAdded", true);
		}

		private void stopCollectibleAdding()
		{
			animatorSetBool("isCoinAdded", false);
		}

		private void showCollectibleHud()
		{
			setVisible(true);
			base.transform.localScale = Vector3.one;
			CollectibleImage.transform.localRotation = Quaternion.identity;
			base.gameObject.SetActive(true);
			animatorSetBool("isShown", true);
			if (state == CollectibleHudState.closed && this.HudOpened != null)
			{
				this.HudOpened(base.gameObject);
			}
			CountText.text = collectibleCount.ToString();
			state = CollectibleHudState.opening;
		}

		private void hideCollectibleHud()
		{
			invokeActive = false;
			animatorSetBool("isShown", false);
			state = CollectibleHudState.closed;
		}

		private void animatorSetBool(string varName, bool value)
		{
			if (animator == null)
			{
				animator = GetComponent<Animator>();
			}
			if (animator != null && animator.isInitialized)
			{
				animator.SetBool(varName, value);
			}
		}

		private void setCollectibleImage()
		{
			string key = currentCollectibleDefinition.SpriteAsset.Key;
			if (loadedSprites.ContainsKey(key))
			{
				CollectibleImage.sprite = loadedSprites[key];
			}
			else
			{
				Content.LoadAsync(onSpriteLoaded, currentCollectibleDefinition.SpriteAsset);
			}
		}

		private void onSpriteLoaded(string path, Sprite sprite)
		{
			CollectibleImage.sprite = sprite;
			loadedSprites[path] = sprite;
		}

		private void checkTutorial()
		{
			if (Service.Get<TutorialManager>().IsTutorialAvailable(CollectiblesTutorial.Id))
			{
				int num = 300;
				if (numTutorialsShown == 0)
				{
					num = 50;
				}
				else if (numTutorialsShown == 1)
				{
					num = 100;
				}
				if (numCollectiblesSinceTutorial > num)
				{
					numCollectiblesSinceTutorial = 0;
					numTutorialsShown++;
					ShouldShowCollectibleTutorial = true;
				}
			}
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
	}
}
