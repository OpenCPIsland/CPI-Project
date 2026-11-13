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
	public class ScavengerHuntHud : MonoBehaviour
	{
		private const int MAX_MARBLE_CREATION = 10;

		private const string EMPTY_CLOCK_TIME = "0:00";

		private const string RESET_ANIM_TRIGGER = "Reset";

		private const string CLOCK_IS_PULSING_BOOL = "IsPulsing";

		private const string PICKING_ROLES_TOKEN = "PartyGames.ScavengerHunt.PickingRoles";

		private const string START_TOKEN = "PartyGames.ScavengerHunt.Start";

		private const string WAITING_FOR_TOKEN = "PartyGames.ScavengerHunt.WaitingForPlayer";

		private const string WHO_HIDES_TOKEN = "PartyGames.ScavengerHunt.Role.Seeker";

		private const string WHO_FINDS_TOKEN = "PartyGames.ScavengerHunt.Role.Hider";

		private const string HIDE_ITEMS_TOKEN = "PartyGames.ScavengerHunt.HideInZone";

		private const string THEY_ARE_HIDING_TOKEN = "PartyGames.ScavengerHunt.TheyHideInZone";

		private const string FIND_ITEMS_TOKEN = "PartyGames.ScavengerHunt.FindItemsInZone";

		private const string THEY_ARE_FINDING_TOKEN = "PartyGames.ScavengerHunt.PlayerIsFinding";

		private const string READY_TOKEN = "PartyGames.ScavengerHunt.Ready";

		private const string SET_TOKEN = "PartyGames.ScavengerHunt.Set";

		private const string GO_TOKEN = "PartyGames.ScavengerHunt.Go";

		private const string TIMES_UP_TOKEN = "PartyGames.ScavengerHunt.TimesUp";

		public int SecondsLeftForClockTimer = 5;

		public SpriteSelector ItemUIPrefab;

		public Text CountDownText;

		public Text HeaderText;

		public Text InstructionText;

		public Transform ItemObjectContainer;

		public Text TimeText;

		public Image TimerImage;

		public ColorUtils.ColorAtPercent[] TimerColors;

		public GameObject TimeInactiveObject;

		public GameObject ClockParentGameObject;

		public Animator ClockAnimator;

		public GameObject FinderBlinkGameObject;

		public SpriteSelector FinderBlinkImage;

		private Animator hudAnimator;

		private int resetAnimationTriggerHash;

		private int clockPulseBoolHash;

		private bool isClockPulsing;

		private ScavengerHuntData scavengerHuntData;

		private List<SpriteSelector> itemsList;

		private bool timerIsRunning;

		private float timerCount;

		private float timerMax;

		private Color TimerColor;

		private int itemsHidden;

		private int itemsFound;

		private bool isPlayingTimeOutSound;

		private void Awake()
		{
			hudAnimator = GetComponent<Animator>();
			resetAnimationTriggerHash = Animator.StringToHash("Reset");
			clockPulseBoolHash = Animator.StringToHash("IsPulsing");
			isClockPulsing = false;
			itemsList = new List<SpriteSelector>();
			timerIsRunning = false;
			FinderBlinkGameObject.SetActive(false);
		}

		public void Init(ScavengerHuntData scavengerHuntData)
		{
			this.scavengerHuntData = scavengerHuntData;
		}

		public void SetToSelectingRoleState()
		{
			ClockParentGameObject.SetActive(false);
			setSingleTextToActive(true);
			CountDownText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.PickingRoles");
		}

		public void SetToHidingState(float waitTime, float timerTime)
		{
			setSingleTextToActive(true);
			hudAnimator.SetTrigger(resetAnimationTriggerHash);
			if (scavengerHuntData.LocalPlayerRole == ScavengerHunt.ScavengerHuntRoles.Hider)
			{
				CountDownText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.Role.Hider");
			}
			else
			{
				CountDownText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.Role.Seeker");
			}
			int num = scavengerHuntData.TotalMarbleCount;
			if (num > 10)
			{
				num = 10;
			}
			for (int i = 0; i < num; i++)
			{
				createItem(ItemUIPrefab);
			}
			itemsList.Reverse();
			itemsHidden = 0;
			CoroutineRunner.Start(showDelayedStateBeforeHider(waitTime, timerTime), this, "showDelayedStateBeforeHider");
		}

		private IEnumerator showDelayedStateBeforeHider(float waitTime, float timerTime)
		{
			yield return new WaitForSecondsRealtime(waitTime);
			ClockParentGameObject.SetActive(true);
			if (ClockAnimator.GetBool(clockPulseBoolHash))
			{
				ClockAnimator.SetBool(clockPulseBoolHash, false);
				isClockPulsing = false;
			}
			setSingleTextToActive(false);
			hudAnimator.SetTrigger(resetAnimationTriggerHash);
			if (scavengerHuntData.LocalPlayerRole == ScavengerHunt.ScavengerHuntRoles.Hider)
			{
				HeaderText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.Start");
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.HideInZone");
				InstructionText.text = string.Format(tokenTranslation, scavengerHuntData.RoomName);
			}
			else
			{
				string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.WaitingForPlayer");
				HeaderText.text = string.Format(tokenTranslation2, scavengerHuntData.OtherPlayerName);
				string tokenTranslation3 = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.TheyHideInZone");
				InstructionText.text = string.Format(tokenTranslation3, scavengerHuntData.RoomName);
			}
			startTimer(timerTime, scavengerHuntData.LocalPlayerRole != ScavengerHunt.ScavengerHuntRoles.Hider);
		}

		public void SetToFindingState(float waitTime, float timerTime)
		{
			ClockParentGameObject.SetActive(true);
			if (ClockAnimator.GetBool(clockPulseBoolHash))
			{
				ClockAnimator.SetBool(clockPulseBoolHash, false);
				isClockPulsing = false;
			}
			setSingleTextToActive(false);
			hudAnimator.SetTrigger(resetAnimationTriggerHash);
			itemsFound = itemsHidden - 1;
			if (scavengerHuntData.LocalPlayerRole == ScavengerHunt.ScavengerHuntRoles.Finder)
			{
				HeaderText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.Start");
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.FindItemsInZone");
				InstructionText.text = string.Format(tokenTranslation, scavengerHuntData.RoomName);
				FinderBlinkGameObject.SetActive(true);
				Service.Get<EventDispatcher>().AddListener<ScavengerHuntEvents.FinderBulbBlink>(onFinderBulbBlink);
			}
			else
			{
				string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.WaitingForPlayer");
				HeaderText.text = string.Format(tokenTranslation2, scavengerHuntData.OtherPlayerName);
				string tokenTranslation3 = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.PlayerIsFinding");
				InstructionText.text = string.Format(tokenTranslation3, scavengerHuntData.OtherPlayerName);
			}
			resetClock();
			CoroutineRunner.Start(delayedTextUpdateForReadySetGo(waitTime, timerTime), this, "delayedTextUpdateForReadySetGo");
		}

		private bool onFinderBulbBlink(ScavengerHuntEvents.FinderBulbBlink evt)
		{
			if (FinderBlinkGameObject.activeSelf)
			{
				int index = (!evt.IsOn) ? 1 : 0;
				FinderBlinkImage.SelectSprite(index);
			}
			return false;
		}

		private void setSingleTextToActive(bool isSingleText)
		{
			HeaderText.gameObject.SetActive(!isSingleText);
			InstructionText.gameObject.SetActive(!isSingleText);
			ItemObjectContainer.parent.gameObject.SetActive(!isSingleText);
			CountDownText.gameObject.SetActive(isSingleText);
		}

		private IEnumerator delayedTextUpdateForReadySetGo(float totalWaitTime, float timerTime)
		{
			float slicedWaitTime = totalWaitTime / 3f;
			setSingleTextToActive(true);
			CountDownText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.Ready");
			yield return new WaitForSeconds(slicedWaitTime);
			CountDownText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.Set");
			yield return new WaitForSeconds(slicedWaitTime);
			CountDownText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.Go");
			yield return new WaitForSeconds(slicedWaitTime);
			setSingleTextToActive(false);
			startTimer(timerTime, scavengerHuntData.LocalPlayerRole != ScavengerHunt.ScavengerHuntRoles.Finder);
		}

		private void startTimer(float duration, bool showInactiveTimerImage)
		{
			timerMax = duration;
			timerCount = 0f;
			TimerImage.fillAmount = 1f;
			TimeInactiveObject.SetActive(showInactiveTimerImage);
			timerIsRunning = true;
		}

		private void Update()
		{
			if (timerIsRunning)
			{
				updateTimerImage();
			}
		}

		private void updateTimerImage()
		{
			timerCount += Time.deltaTime;
			float num = timerCount / timerMax;
			TimerImage.fillAmount -= 1f / timerMax * Time.deltaTime;
			for (int i = 0; i < TimerColors.Length; i++)
			{
				if (num > TimerColors[i].Percent)
				{
					TimerColor = TimerColors[i].Color;
					if (!isPlayingTimeOutSound && i == TimerColors.Length - 1)
					{
						EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/TimeRunningOut", EventAction.PlaySound);
						isPlayingTimeOutSound = true;
					}
				}
			}
			TimerImage.color = TimerColor;
			float num2 = timerMax - timerCount;
			int num3 = Mathf.FloorToInt(num2 / 60f);
			int num4 = Mathf.FloorToInt(num2 - (float)(num3 * 60));
			TimeText.text = string.Format("{0:0}:{1:00}", num3, num4);
			if (!isClockPulsing && num4 < SecondsLeftForClockTimer && num3 == 0)
			{
				isClockPulsing = true;
				ClockAnimator.SetBool(clockPulseBoolHash, true);
			}
			if (num3 <= 0 && num4 <= 0)
			{
				TimeText.text = "0:00";
				InstructionText.gameObject.SetActive(false);
				ClockAnimator.SetBool(clockPulseBoolHash, false);
				isClockPulsing = false;
				timerIsRunning = false;
				HeaderText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.TimesUp");
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/TimeRunningOut", EventAction.StopSound);
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/TimesUp", EventAction.PlaySound);
				isPlayingTimeOutSound = false;
			}
		}

		private void resetClock()
		{
			TimerImage.color = TimerColors[0].Color;
			timerCount = 0f;
			TimerImage.fillAmount = 1f;
			timerIsRunning = false;
			TimeText.text = "0:00";
			TimeInactiveObject.gameObject.SetActive(true);
		}

		public void HideItem()
		{
			if (itemsHidden >= itemsList.Count)
			{
				Log.LogErrorFormatted(this, "An item was hidden with an index count higher than icons. Hidden Index: {0} Total Icon Count: {1}", itemsHidden, itemsList.Count);
				return;
			}
			itemsList[itemsHidden].SelectSprite(1);
			itemsHidden++;
		}

		public void FoundItem()
		{
			if (itemsFound < 0)
			{
				Log.LogErrorFormatted(this, "More items were found than icons that were created. Found Index: {0} Total Icon Count: {1}", itemsFound, itemsList.Count);
				return;
			}
			itemsList[itemsFound].SelectSprite(0);
			itemsFound--;
		}

		private void createItem(SpriteSelector itemToDuplicate)
		{
			SpriteSelector spriteSelector = Object.Instantiate(itemToDuplicate);
			spriteSelector.transform.SetParent(ItemObjectContainer, false);
			itemsList.Add(spriteSelector);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<ScavengerHuntEvents.FinderBulbBlink>(onFinderBulbBlink);
		}
	}
}
