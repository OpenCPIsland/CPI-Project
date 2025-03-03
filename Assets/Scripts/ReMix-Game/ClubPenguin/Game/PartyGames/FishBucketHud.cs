using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class FishBucketHud : MonoBehaviour
	{
		public enum FishBucketHudState
		{
			Instructions,
			PickingTurn,
			InGame
		}

		private const string PICKING_TOKEN = "Activity.FishCatch.GoFirst";

		private const string FIRST_TURN_TOKEN = "Activity.FishCatch.PlayerPicked";

		private const string LOCAL_PLAYER_TURN_TOKEN = "Activity.FishCatch.TurnInstruction";

		private const string OTHER_PLAYER_TURN_TOKEN = "Activity.FishCatch.OthersTurn";

		public Text HeaderText;

		public GameObject PlayerBucketPanel;

		public Image FishProgressBar;

		public GameObject PickingTurnTextPanel;

		public GameObject GameOverText;

		public GameObject GamePanel;

		public GameObject InstructionText;

		public float InstructionTime = 2f;

		public float ProgressBarAnimTime = 0.3f;

		public ColorUtils.ColorAtPercent[] ProgressBarColors;

		public Vector3 ScorePopupOffset;

		public float ScorePopupDelay = 1f;

		public float ScorePopupDestroyTime = 2f;

		public float InkedPopupDestroyTime = 2f;

		[Space(10f)]
		public string GameInstructionSFXTrigger;

		public string GameStartSFXTrigger;

		public string ShotStartSFXTrigger;

		public string InkWipeSFXTrigger;

		private int totalCards;

		private int cardsLeft;

		private Dictionary<long, FishBucketPlayerHud> playerHuds;

		private FishBucketPlayerHud currentPlayerHud;

		private long currentPlayerId;

		private UITimer turnIndicator;

		private Animator turnIndicatorAnimator;

		private FishBucketDefinition fishBucketDefinition;

		private Localizer localizer;

		private EventDispatcher dispatcher;

		private readonly PrefabContentKey PLAYER_BUCKET_KEY = new PrefabContentKey("Prefabs/FishBucket/PlayerBucketItem");

		private readonly PrefabContentKey INKED_OVERLAY_PREFAB_KEY = new PrefabContentKey("PartyGameFX/FishBucketInkWash");

		private readonly PrefabContentKey INKED_POPUP_PREFAB_KEY = new PrefabContentKey("Prefabs/FishBucket/FishBucketInked");

		private readonly PrefabContentKey TURN_INDICATOR_PREFAB_KEY = new PrefabContentKey("Prefabs/FishBucket/FishBucketTimerIndicator");

		private readonly int ANIMATOR_HASH_TURN_INDICATOR_OPEN = Animator.StringToHash("Open");

		private readonly int ANIMATOR_HASH_TURN_INDICATOR_CLOSE = Animator.StringToHash("Close");

		private GameObject inkedOverlayPrefab;

		private GameObject inkedPopupPrefab;

		private GameObject turnIndicatorPrefab;

		public FishBucketHudState HudState
		{
			get;
			private set;
		}

		private void Awake()
		{
			localizer = Service.Get<Localizer>();
			dispatcher = Service.Get<EventDispatcher>();
		}

		private void Start()
		{
			FishProgressBar.fillAmount = 1f;
			Content.LoadAsync(onInkedOverlayLoaded, INKED_OVERLAY_PREFAB_KEY);
			Content.LoadAsync(onInkedPopupLoaded, INKED_POPUP_PREFAB_KEY);
			Content.LoadAsync(onTurnIndicatorLoaded, TURN_INDICATOR_PREFAB_KEY);
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			iTween.StopByName("ProgressBarAnim");
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.RemovePlayerIndicator(currentPlayerId, false));
		}

		private void OnDisable()
		{
			iTween.StopByName("ProgressBarAnim");
		}

		private void onInkedOverlayLoaded(string path, GameObject prefab)
		{
			inkedOverlayPrefab = prefab;
		}

		private void onInkedPopupLoaded(string path, GameObject prefab)
		{
			inkedPopupPrefab = prefab;
		}

		private void onTurnIndicatorLoaded(string path, GameObject prefab)
		{
			turnIndicatorPrefab = prefab;
		}

		private void createTurnIndicator()
		{
			GameObject gameObject = Object.Instantiate(turnIndicatorPrefab);
			turnIndicator = gameObject.GetComponent<UITimer>();
			turnIndicatorAnimator = gameObject.GetComponent<Animator>();
			turnIndicator.gameObject.SetActive(false);
		}

		public void Init(Dictionary<long, FishBucket.FishBucketPlayerData> playerData, FishBucketDefinition definition)
		{
			fishBucketDefinition = definition;
			CoroutineRunner.Start(loadPlayerHuds(playerData), this, "LoadFishBucketPlayerHuds");
		}

		private IEnumerator loadPlayerHuds(Dictionary<long, FishBucket.FishBucketPlayerData> playerData)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(PLAYER_BUCKET_KEY);
			yield return request;
			playerHuds = new Dictionary<long, FishBucketPlayerHud>();
			foreach (FishBucket.FishBucketPlayerData value in playerData.Values)
			{
				FishBucketPlayerHud component = Object.Instantiate(request.Asset, PlayerBucketPanel.transform, false).GetComponent<FishBucketPlayerHud>();
				component.Init(value);
				component.SetHighlighted(false);
				playerHuds.Add(value.PlayerId, component);
			}
		}

		public void SetCurrentPlayersTurn(FishBucket.FishBucketPlayerData playerData, bool isLocalPlayer)
		{
			if (currentPlayerHud != null)
			{
				currentPlayerHud.SetHighlighted(false);
			}
			if (currentPlayerId != 0)
			{
				dispatcher.DispatchEvent(new PlayerIndicatorEvents.RemovePlayerIndicator(currentPlayerId, false, false));
			}
			currentPlayerId = playerData.PlayerId;
			currentPlayerHud = playerHuds[playerData.PlayerId];
			currentPlayerHud.SetHighlighted(true);
			string text = "";
			text = ((!isLocalPlayer) ? string.Format(localizer.GetTokenTranslation("Activity.FishCatch.OthersTurn"), playerData.DisplayName) : localizer.GetTokenTranslation("Activity.FishCatch.TurnInstruction"));
			HeaderText.text = text;
			if (turnIndicator == null)
			{
				createTurnIndicator();
			}
			dispatcher.DispatchEvent(new PlayerIndicatorEvents.ShowPlayerIndicator(turnIndicator.gameObject, currentPlayerId));
			turnIndicatorAnimator.SetTrigger(ANIMATOR_HASH_TURN_INDICATOR_OPEN);
			turnIndicator.StartCountdown(fishBucketDefinition.TurnTimeInSeconds);
		}

		public void SetState(FishBucketHudState newState)
		{
			switch (newState)
			{
			case FishBucketHudState.Instructions:
				HeaderText.gameObject.SetActive(false);
				InstructionText.SetActive(true);
				GamePanel.SetActive(false);
				PickingTurnTextPanel.SetActive(false);
				CoroutineRunner.Start(switchToPickingTurn(InstructionTime), this, "");
				EventManager.Instance.PostEvent(GameInstructionSFXTrigger, EventAction.PlaySound);
				break;
			case FishBucketHudState.PickingTurn:
				PickingTurnTextPanel.SetActive(true);
				InstructionText.SetActive(false);
				break;
			case FishBucketHudState.InGame:
				HeaderText.gameObject.SetActive(true);
				GamePanel.SetActive(true);
				PickingTurnTextPanel.SetActive(false);
				EventManager.Instance.PostEvent(GameStartSFXTrigger, EventAction.PlaySound);
				break;
			}
			HudState = newState;
		}

		private IEnumerator switchToPickingTurn(float delay)
		{
			yield return new WaitForSeconds(InstructionTime);
			SetState(FishBucketHudState.PickingTurn);
		}

		public void SetTotalCards(int cards)
		{
			totalCards = cards;
			cardsLeft = totalCards;
		}

		public void ShowTurn(FishBucket.FishBucketPlayerData playerData, int scoreDelta)
		{
			EventManager.Instance.PostEvent(ShotStartSFXTrigger, EventAction.PlaySound);
			cardsLeft--;
			CoroutineRunner.Start(showProgressBarAnim(), this, "ProgressBarAnim");
			playerHuds[playerData.PlayerId].ChangeScore(scoreDelta);
			turnIndicatorAnimator.SetTrigger(ANIMATOR_HASH_TURN_INDICATOR_CLOSE);
			if (scoreDelta > 0 || !playerData.IsLocalPlayer)
			{
				CoroutineRunner.Start(showScorePopup(playerData.PlayerId, scoreDelta), this, "ShowFishBucketScorePopup");
			}
		}

		private IEnumerator showProgressBarAnim()
		{
			float newFillAmount = (float)cardsLeft / (float)totalCards;
			iTween.StopByName("ProgressBarAnim");
			yield return new WaitForFrame(1);
			iTween.ValueTo(args: iTween.Hash("name", "ProgressBarAnim", "from", FishProgressBar.fillAmount, "to", newFillAmount, "time", ProgressBarAnimTime, "easetype", iTween.EaseType.easeOutCubic, "onupdate", "ProgressBarAnimUpdate", "onupdatetarget", base.gameObject), target: base.gameObject);
		}

		private IEnumerator showScorePopup(long playerId, int scoreDelta)
		{
			yield return new WaitForSeconds(ScorePopupDelay);
			string scoreString = (scoreDelta <= 0) ? scoreDelta.ToString() : string.Format("+{0}", scoreDelta);
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerScoreEvents.ShowPlayerScore(playerId, scoreString));
		}

		public void OnTurnSequenceComplete(int turnDelta)
		{
			if (turnDelta > 0 && turnIndicatorAnimator != null)
			{
				turnIndicatorAnimator.SetTrigger(ANIMATOR_HASH_TURN_INDICATOR_OPEN);
				turnIndicator.StartCountdown(fishBucketDefinition.TurnTimeInSeconds);
			}
		}

		public void ProgressBarAnimUpdate(float value)
		{
			Color color = ProgressBarColors[0].Color;
			for (int i = 1; i < ProgressBarColors.Length; i++)
			{
				if (value > ProgressBarColors[i].Percent)
				{
					color = ProgressBarColors[i].Color;
				}
			}
			FishProgressBar.color = color;
			FishProgressBar.fillAmount = value;
		}

		public void ShowInkedOverlay(int scoreDelta)
		{
			EventManager.Instance.PostEvent(InkWipeSFXTrigger, EventAction.PlaySound);
			GameObject popup = Object.Instantiate(inkedOverlayPrefab);
			dispatcher.DispatchEvent(new PopupEvents.ShowCameraSpacePopup(popup, false, true, "Accessibility.Popup.Title.FishBUcketInkedOverlay"));
			GameObject gameObject = Object.Instantiate(inkedPopupPrefab);
			gameObject.GetComponent<FishBucketInkedPopup>().SetScore(scoreDelta);
			dispatcher.DispatchEvent(new PopupEvents.ShowPopup(gameObject));
			Object.Destroy(gameObject, InkedPopupDestroyTime);
		}

		public void OnPlayerRemoved(long playerId)
		{
			playerHuds[playerId].SetInactive();
		}
	}
}
