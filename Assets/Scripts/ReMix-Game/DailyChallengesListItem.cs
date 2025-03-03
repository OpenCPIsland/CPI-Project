using ClubPenguin;
using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.DailyChallenge;
using ClubPenguin.NPC;
using ClubPenguin.Progression;
using ClubPenguin.Task;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DailyChallengesListItem : MonoBehaviour
{
	public Image MascotImage;

	public Text ChallengeTitle;

	public Text ChallengeText;

	public Text CoinsText;

	public Image XPImage;

	public Text XPText;

	public GameObject MaxLevelIcon;

	public Text ProgressText;

	public Text QuantityText;

	public Image BackgroundBorder;

	public Image ProgressBarImage;

	public GameObject CheckmarkOverlay;

	public GameObject[] ProgressObjects;

	public GameObject[] ClaimObjects;

	public GameObject[] ClaimCompleteObjects;

	public GameObject RewardButtonGroup;

	public Button ClaimButton;

	public GameObject CommunityTitleItem;

	public GameObject[] LockedOverlay;

	public GameObject[] ProgressionOverlay;

	public Text ProgressionText;

	public GameObject[] QuestOverlay;

	private int playerProgressionLevel;

	private int mascotQuestLevel;

	private bool hasDoneInit = false;

	private MembershipData membershipData;

	private Task _task;

	private MembershipData MembershipData
	{
		get
		{
			if (membershipData == null)
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out membershipData))
				{
					membershipData.MembershipDataUpdated += onMembershipDataUpdated;
				}
			}
			return membershipData;
		}
		set
		{
			if (membershipData == null && value != null)
			{
				membershipData = value;
				membershipData.MembershipDataUpdated += onMembershipDataUpdated;
			}
		}
	}

	public Task Task
	{
		get
		{
			return _task;
		}
		set
		{
			_task = value;
			string subGroupByTaskName = Service.Get<TaskService>().GetSubGroupByTaskName(value.Id);
			Mascot mascot = Service.Get<MascotService>().GetMascot(subGroupByTaskName);
			ChallengeTitle.text = Service.Get<Localizer>().GetTokenTranslation(value.Definition.Title);
			ChallengeText.text = Service.Get<Localizer>().GetTokenTranslation(value.Definition.Description);
			if (CoinsText != null)
			{
				CoinsText.text = CoinRewardableDefinition.Coins(value.Definition.Reward).ToString();
			}
			if (XPText != null)
			{
				XPText.text = getXPfromMascot(mascot).ToString();
			}
			if (ProgressText != null)
			{
				ProgressText.text = value.Counter.ToString();
			}
			if (QuantityText != null)
			{
				QuantityText.text = value.Goal.ToString();
			}
			if (CommunityTitleItem != null)
			{
				if (value.Definition.Group == TaskDefinition.TaskGroup.Community || value.Definition.Group == TaskDefinition.TaskGroup.Teamwork)
				{
					CommunityTitleItem.SetActive(true);
				}
				else
				{
					CommunityTitleItem.SetActive(false);
				}
			}
			bool flag = Service.Get<ProgressionService>().IsMascotMaxLevel(subGroupByTaskName);
			if (MaxLevelIcon != null)
			{
				MaxLevelIcon.SetActive(flag);
			}
			if (XPText != null)
			{
				XPText.gameObject.SetActive(!flag);
			}
			mascotQuestLevel = mascot.GetHighestCompletedQuest();
			if (hasDoneInit)
			{
				SetTaskProgress();
			}
			if (ProgressBarImage != null && !_task.IsComplete)
			{
				Image component = ProgressBarImage.GetComponent<Image>();
				if ((bool)component)
				{
					component.fillAmount = (_task.IsComplete ? 1f : ((float)Task.Counter / (float)Task.Goal));
				}
			}
		}
	}

	private void Awake()
	{
		if (ClaimButton != null)
		{
			ClaimButton.onClick.AddListener(onClaimButtonClicked);
		}
	}

	private void Start()
	{
	}

	private void onMembershipDataUpdated(MembershipData membershipData)
	{
		enableGroup(LockedOverlay, Task.IsMemberLocked(isMember()));
	}

	private void OnDestroy()
	{
		if (ClaimButton != null)
		{
			ClaimButton.onClick.RemoveListener(onClaimButtonClicked);
		}
		if (membershipData != null)
		{
			membershipData.MembershipDataUpdated -= onMembershipDataUpdated;
		}
	}

	public void Init(Task task)
	{
		Task = task;
		playerProgressionLevel = Service.Get<ProgressionService>().Level;
		if (!hasDoneInit)
		{
			SetTaskProgress();
			hasDoneInit = true;
		}
	}

	private int getXPfromMascot(Mascot mascot)
	{
		foreach (MascotXPRewardDefinition definition in Task.Definition.Reward.GetDefinitions<MascotXPRewardDefinition>())
		{
			if (definition.Mascot != null)
			{
				string name = definition.Mascot.name;
				if (name == mascot.Name)
				{
					return definition.XP;
				}
			}
		}
		return 0;
	}

	private void enableGroup(GameObject[] gos, bool isEnabled)
	{
		if (gos == null)
		{
			return;
		}
		foreach (GameObject gameObject in gos)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(isEnabled);
			}
		}
	}

	public void SetTaskProgress()
	{
		enableGroup(ClaimObjects, Task.IsComplete);
		if (Task.IsComplete)
		{
			setRewardClaimed(Task.IsRewardClaimed);
			if (RewardButtonGroup != null)
			{
				RewardButtonGroup.SetActive(true);
			}
		}
		enableGroup(ProgressObjects, !Task.IsComplete);
		if (Task.IsComplete && Task.Definition.Group == TaskDefinition.TaskGroup.Community && Task.IsMemberLocked(isMember()))
		{
			enableGroup(LockedOverlay, false);
		}
		else
		{
			enableGroup(LockedOverlay, Task.IsMemberLocked(isMember()));
		}
		if (!Task.IsMemberLocked(isMember()))
		{
			if (Task.IsProgressionLocked(playerProgressionLevel))
			{
				ProgressionText.text = Task.Definition.LevelRequired.ToString();
				enableGroup(ProgressionOverlay, true);
			}
			else if (Task.IsQuestLocked(mascotQuestLevel))
			{
				enableGroup(QuestOverlay, true);
			}
		}
	}

	private bool isMember()
	{
		return MembershipData != null && MembershipData.IsMember;
	}

	public void onClaimButtonClicked()
	{
		setRewardClaimed(true);
		ClaimDailyTaskReward(Task);
	}

	private IEnumerator playClaimPenguinAnimation()
	{
		yield return new WaitForSeconds(1f);
		ClubPenguin.SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<Animator>().SetTrigger(AnimationHashes.Params.Woohoo);
	}

	private void setRewardClaimed(bool isClaimed)
	{
		enableGroup(ClaimObjects, !isClaimed);
		enableGroup(ClaimCompleteObjects, isClaimed);
	}

	public static void ClaimDailyTaskReward(Task dailyTask)
	{
		Service.Get<DailyChallengeService>().ClaimTaskReward(dailyTask);
		PresenceData component = Service.Get<CPDataEntityCollection>().GetComponent<PresenceData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
		string tier = "";
		if (component != null)
		{
			tier = component.World;
		}
		Service.Get<ICPSwrveService>().Action("daily_task." + dailyTask.Definition.Group.ToString().ToLower() + ".claim_daily_reward", dailyTask.Definition.name, tier);
		Service.Get<EventDispatcher>().DispatchEvent(new TaskEvents.TaskRewardClaimed(dailyTask));
	}
}
