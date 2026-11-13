using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class XPHud : MonoBehaviour
	{
		public enum XPHudState
		{
			closed,
			opening,
			addingXP,
			waitingToClose,
			closing
		}

		private const string ANIMATOR_IS_SHOWING_XP = "isShown";

		private const int DEFAULT_LAYOUTELEMENT_HEIGHT = 130;

		private const float XP_ADDITION_TIME = 2f;

		public Transform XpHudParentTransform;

		public LayoutElement XPLayoutElement;

		private string currentMascotName;

		private long targetMacotLevel;

		private long currentMascotLevel;

		private CPDataEntityCollection dataEntityCollection;

		private XPHudItem currentXpDisplay;

		private Queue pendingXP;

		private Queue suppressedXP;

		private XPHudState state;

		private bool isLevelUpScreenSuppressed = false;

		[SerializeField]
		private float HIDE_DELAY_TIME = 1f;

		private static PrefabContentKey mascotXPContentKey = new PrefabContentKey("Prefabs/Quest/MascotXPHUDs/XPPanel_*");

		public XPHudState State
		{
			get
			{
				return state;
			}
		}

		public string CurrentMascotName
		{
			get
			{
				return currentMascotName;
			}
		}

		public event Action<GameObject> HudOpened;

		public event Action<GameObject> HudClosed;

		public void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			state = XPHudState.closed;
			pendingXP = new Queue();
			suppressedXP = new Queue();
			XPLayoutElement.preferredHeight = 0f;
			checkPendingLevelUp();
			Service.Get<EventDispatcher>().AddListener<RewardEvents.AddXP>(onAddXP);
			Service.Get<EventDispatcher>().AddListener<RewardEvents.ShowSuppressedAddXP>(onShowSuppressedAddXP);
			Service.Get<EventDispatcher>().AddListener<RewardEvents.SuppressLevelUpPopup>(onSuppressLevelUpPopup);
			Service.Get<EventDispatcher>().AddListener<RewardEvents.UnsuppressLevelUpPopup>(onUnsuppressLevelUpPopup);
		}

		public void OnXPIntroAnimationComplete()
		{
			CoroutineRunner.Start(updateXpDisplay(), this, "updateXpDisplay");
			currentXpDisplay.EnableParticleSystems();
		}

		public void OnXPOutroAnimationComplete()
		{
			state = XPHudState.closed;
			XPHudItem xPHudItem = currentXpDisplay;
			xPHudItem.IntroAnimationCompleteAction = (System.Action)Delegate.Remove(xPHudItem.IntroAnimationCompleteAction, new System.Action(OnXPIntroAnimationComplete));
			XPHudItem xPHudItem2 = currentXpDisplay;
			xPHudItem2.OutroAnimationCompleteAction = (System.Action)Delegate.Remove(xPHudItem2.OutroAnimationCompleteAction, new System.Action(OnXPOutroAnimationComplete));
			currentMascotName = null;
			UnityEngine.Object.Destroy(currentXpDisplay.gameObject);
			XPLayoutElement.preferredHeight = 0f;
			if (pendingXP.Count > 0)
			{
				onAddXP((RewardEvents.AddXP)pendingXP.Dequeue());
			}
			if (this.HudClosed != null)
			{
				this.HudClosed(base.gameObject);
			}
		}

		public void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<RewardEvents.AddXP>(onAddXP);
			Service.Get<EventDispatcher>().RemoveListener<RewardEvents.ShowSuppressedAddXP>(onShowSuppressedAddXP);
			Service.Get<EventDispatcher>().RemoveListener<RewardEvents.SuppressLevelUpPopup>(onSuppressLevelUpPopup);
			Service.Get<EventDispatcher>().RemoveListener<RewardEvents.UnsuppressLevelUpPopup>(onUnsuppressLevelUpPopup);
			CoroutineRunner.StopAllForOwner(this);
			checkRemainingXP();
		}

		private void checkRemainingXP()
		{
			if (currentMascotLevel < targetMacotLevel && ProgressionService.GetMascotLevelFromXP(currentMascotLevel) != Service.Get<ProgressionService>().MascotLevel(currentMascotName))
			{
				Reward rewardForProgressionLevel = RewardUtils.GetRewardForProgressionLevel(Service.Get<ProgressionService>().Level);
				ShowRewardPopup pendingLevelUpPopup = new ShowRewardPopup.Builder(DRewardPopup.RewardPopupType.levelUp, rewardForProgressionLevel).setMascotName(currentMascotName).Build();
				Service.Get<ProgressionService>().PendingLevelUpPopup = pendingLevelUpPopup;
			}
		}

		private void checkPendingLevelUp()
		{
			if (Service.Get<ProgressionService>().PendingLevelUpPopup != null)
			{
				Service.Get<ProgressionService>().PendingLevelUpPopup.Execute();
				Service.Get<ProgressionService>().PendingLevelUpPopup = null;
			}
		}

		private bool onAddXP(RewardEvents.AddXP evt)
		{
			if (!evt.ShowReward)
			{
				suppressedXP.Enqueue(evt);
			}
			else
			{
				switch (state)
				{
				case XPHudState.closed:
					currentMascotName = evt.MascotName;
					if (!Service.Get<ProgressionService>().IsMascotMaxLevel(currentMascotName, evt.PreviousLevel))
					{
						Mascot mascot = Service.Get<MascotService>().GetMascot(currentMascotName);
						if (mascot != null)
						{
							Service.Get<EventDispatcher>().DispatchEvent(new PlayerScoreEvents.ShowPlayerScore(dataEntityCollection.LocalPlayerSessionId, string.Format("+{0}", evt.XPAdded), PlayerScoreEvents.ParticleType.XP, mascot.Definition.XPTintColor));
						}
						CoroutineRunner.Start(loadXPHudPrefab(mascotXPContentKey, evt.MascotName), this, "XPHud.loadXPHudPrefab");
						currentMascotLevel = evt.PreviousLevel;
						targetMacotLevel = evt.CurrentLevel;
						state = XPHudState.opening;
					}
					break;
				case XPHudState.opening:
				case XPHudState.addingXP:
					if (currentMascotName == evt.MascotName)
					{
						targetMacotLevel = Math.Max(targetMacotLevel, evt.CurrentLevel);
					}
					else
					{
						pendingXP.Enqueue(evt);
					}
					break;
				case XPHudState.waitingToClose:
					if (currentMascotName == evt.MascotName)
					{
						CancelInvoke();
						targetMacotLevel = Math.Max(targetMacotLevel, evt.CurrentLevel);
						CoroutineRunner.Start(updateXpDisplay(), this, "updateXpDisplay");
					}
					else
					{
						pendingXP.Enqueue(evt);
					}
					break;
				case XPHudState.closing:
					pendingXP.Enqueue(evt);
					break;
				}
				if (ProgressionService.GetMascotLevelFromXP(targetMacotLevel) != ProgressionService.GetMascotLevelFromXP(currentMascotLevel))
				{
					disableUI();
				}
			}
			return false;
		}

		private void disableUI()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideCellPhoneHud));
			SceneRefs.UiTrayRoot.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "minNPC"));
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
		}

		private bool onShowSuppressedAddXP(RewardEvents.ShowSuppressedAddXP evt)
		{
			if (suppressedXP.Count > 0)
			{
				RewardEvents.AddXP evt2 = (RewardEvents.AddXP)suppressedXP.Dequeue();
				evt2.ShowReward = true;
				onAddXP(evt2);
			}
			return false;
		}

		private IEnumerator updateXpDisplay()
		{
			ProgressionService progressionService = Service.Get<ProgressionService>();
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.XPAdditionStart));
			state = XPHudState.addingXP;
			int xpDelta = progressionService.MascotLevelXpDelta(currentMascotName, currentMascotLevel);
			int mascotLevelWithOffset = ProgressionService.GetMascotLevelFromXP(currentMascotLevel);
			float additionDelay = 2f / ((float)(targetMacotLevel - currentMascotLevel) / (float)xpDelta);
			bool leveledUp = false;
			while (currentMascotLevel < targetMacotLevel)
			{
				if (mascotLevelWithOffset != ProgressionService.GetMascotLevelFromXP(currentMascotLevel))
				{
					mascotLevelWithOffset = ProgressionService.GetMascotLevelFromXP(currentMascotLevel);
					ShowLevelUp(Service.Get<ProgressionService>().Level);
					leveledUp = true;
					xpDelta = progressionService.MascotLevelXpDelta(currentMascotName, currentMascotLevel);
				}
				float percent = ProgressionService.GetMascotLevelPercentFromXP(currentMascotLevel);
				currentXpDisplay.LevelProgressImage.fillAmount = percent;
				yield return new WaitForSeconds(additionDelay);
				currentMascotLevel += xpDelta;
			}
			if (!leveledUp && mascotLevelWithOffset != ProgressionService.GetMascotLevelFromXP(targetMacotLevel))
			{
				ShowLevelUp(Service.Get<ProgressionService>().Level);
			}
			currentXpDisplay.DisableParticleSystems();
			state = XPHudState.waitingToClose;
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.XPAdditionStop));
			Invoke("hideXpHud", HIDE_DELAY_TIME);
		}

		private void ShowLevelUp(int level)
		{
			Reward rewardForProgressionLevel = RewardUtils.GetRewardForProgressionLevel(level);
			ShowRewardPopup showRewardPopup = new ShowRewardPopup.Builder(DRewardPopup.RewardPopupType.levelUp, rewardForProgressionLevel).setMascotName(currentMascotName).Build();
			if (!isLevelUpScreenSuppressed)
			{
				showRewardPopup.Execute();
			}
			else
			{
				Service.Get<ProgressionService>().PendingLevelUpPopup = showRewardPopup;
			}
		}

		private void showXPHud()
		{
			currentXpDisplay.XPAnimator.SetBool("isShown", true);
			if (state == XPHudState.closed && this.HudOpened != null)
			{
				this.HudOpened(base.gameObject);
			}
			GetComponentInChildren<MascotLevelDisplay>().SetUpMascotLevel(true, null, currentMascotLevel);
		}

		private void hideXpHud()
		{
			state = XPHudState.closing;
			currentXpDisplay.XPAnimator.SetBool("isShown", false);
		}

		private IEnumerator loadXPHudPrefab(PrefabContentKey contentKey, string mascotName)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(contentKey, mascotName);
			yield return assetRequest;
			GameObject xpGO = UnityEngine.Object.Instantiate(assetRequest.Asset);
			xpGO.transform.SetParent(XpHudParentTransform, false);
			currentXpDisplay = xpGO.GetComponent<XPHudItem>();
			currentXpDisplay.LevelProgressImage.fillAmount = ProgressionService.GetMascotLevelPercentFromXP(currentMascotLevel);
			XPHudItem xPHudItem = currentXpDisplay;
			xPHudItem.IntroAnimationCompleteAction = (System.Action)Delegate.Combine(xPHudItem.IntroAnimationCompleteAction, new System.Action(OnXPIntroAnimationComplete));
			XPHudItem xPHudItem2 = currentXpDisplay;
			xPHudItem2.OutroAnimationCompleteAction = (System.Action)Delegate.Combine(xPHudItem2.OutroAnimationCompleteAction, new System.Action(OnXPOutroAnimationComplete));
			XPLayoutElement.preferredHeight = 130f;
			showXPHud();
		}

		private bool onSuppressLevelUpPopup(RewardEvents.SuppressLevelUpPopup evt)
		{
			isLevelUpScreenSuppressed = true;
			return false;
		}

		private bool onUnsuppressLevelUpPopup(RewardEvents.UnsuppressLevelUpPopup evt)
		{
			isLevelUpScreenSuppressed = false;
			checkPendingLevelUp();
			return false;
		}
	}
}
