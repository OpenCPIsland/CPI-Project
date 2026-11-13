using ClubPenguin.Adventure;
using ClubPenguin.CellPhone;
using ClubPenguin.Core;
using ClubPenguin.DailyChallenge;
using ClubPenguin.Net.Domain;
using ClubPenguin.Progression;
using ClubPenguin.Task;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DailyChallengesScrollRect : PagedScrollRect
	{
		public List<TaskDefinition.TaskCategory> IgnoredTaskCategories;

		public GameObject LoadingPanel;

		private static PrefabContentKey itemPrefabContentKey = new PrefabContentKey("DailyChallengesPrefabs/DailyChallengeItem_*");

		private List<ClubPenguin.Task.Task> dailyChallenges;

		private int playerProgressionLevel;

		private EventDispatcher dispatcher;

		protected override void start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dailyChallenges = new List<ClubPenguin.Task.Task>();
			playerProgressionLevel = Service.Get<ProgressionService>().Level;
			Service.Get<DailyChallengeService>().RecordShown();
		}

		protected override void onDestroy()
		{
			dispatcher.RemoveListener<TaskEvents.TaskRewardClaimed>(onTaskClaimed);
			dispatcher.RemoveListener<CatalogServiceProxyEvents.ChallengesReponse>(onThemesRetrieved);
			CoroutineRunner.StopAllForOwner(this);
		}

		protected override void loadContent()
		{
			if (Service.Get<TaskService>().HasLoadedTasks)
			{
				getCatalogThemes();
			}
			else
			{
				dispatcher.AddListener<TaskServiceEvents.TasksLoaded>(onTasksLoaded, EventDispatcher.Priority.LAST);
			}
			dispatcher.AddListener<TaskEvents.TaskRewardClaimed>(onTaskClaimed);
		}

		private bool onTaskClaimed(TaskEvents.TaskRewardClaimed evt)
		{
			dailyChallenges.Remove(evt.Task);
			dailyChallenges.Add(evt.Task);
			reloadList();
			return false;
		}

		private bool onTasksLoaded(TaskServiceEvents.TasksLoaded evt)
		{
			dispatcher.RemoveListener<TaskServiceEvents.TasksLoaded>(onTasksLoaded);
			getCatalogThemes();
			return false;
		}

		private void getCatalogThemes()
		{
			dispatcher.AddListener<CatalogServiceProxyEvents.ChallengesReponse>(onThemesRetrieved);
			Service.Get<CatalogServiceProxy>().GetCurrentThemes();
		}

		private bool onThemesRetrieved(CatalogServiceProxyEvents.ChallengesReponse evt)
		{
			dispatcher.RemoveListener<CatalogServiceProxyEvents.ChallengesReponse>(onThemesRetrieved);
			if (evt.Themes.Count > 0)
			{
				loadCatalogDaily(evt.Themes[0]);
				loadDailies();
				contentLoaded = true;
			}
			else
			{
				GetComponentInParent<CellPhoneActivityScreenSimpleWidget>().gameObject.SetActive(false);
			}
			return false;
		}

		private void loadCatalogDaily(CurrentThemeData currentTheme)
		{
			CatalogThemeDefinition themeByScheduelId = Service.Get<CatalogServiceProxy>().GetThemeByScheduelId(currentTheme.scheduledThemeChallengeId);
			TaskDefinition clothingCatalogChallenge = Service.Get<TaskService>().ClothingCatalogChallenge;
			clothingCatalogChallenge.Title = themeByScheduelId.Title;
			clothingCatalogChallenge.CompletionMessage = themeByScheduelId.CompleteMessage;
			clothingCatalogChallenge.Description = themeByScheduelId.Description;
		}

		private void loadDailies()
		{
			if (LoadingPanel != null)
			{
				LoadingPanel.SetActive(false);
			}
			dailyChallenges.Clear();
			List<ClubPenguin.Task.Task> list = new List<ClubPenguin.Task.Task>();
			List<ClubPenguin.Task.Task> list2 = new List<ClubPenguin.Task.Task>();
			List<ClubPenguin.Task.Task> list3 = new List<ClubPenguin.Task.Task>();
			List<ClubPenguin.Task.Task> list4 = new List<ClubPenguin.Task.Task>();
			foreach (ClubPenguin.Task.Task task in Service.Get<TaskService>().Tasks)
			{
				if (!IgnoredTaskCategories.Contains(task.Definition.Category))
				{
					if (task.IsComplete)
					{
						if (!task.IsRewardClaimed)
						{
							list.Add(task);
						}
						else
						{
							list4.Add(task);
						}
					}
					else
					{
						string subGroupByTaskName = Service.Get<TaskService>().GetSubGroupByTaskName(task.Id);
						Mascot mascot = Service.Get<MascotService>().GetMascot(subGroupByTaskName);
						int highestCompletedQuest = mascot.GetHighestCompletedQuest();
						bool isMember = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
						if (task.IsTaskAvailable(isMember, playerProgressionLevel, highestCompletedQuest))
						{
							list2.Add(task);
						}
						else
						{
							list3.Add(task);
						}
					}
				}
			}
			dailyChallenges.AddRange(list);
			dailyChallenges.AddRange(list2);
			dailyChallenges.AddRange(list3);
			dailyChallenges.AddRange(list4);
			totalItems = dailyChallenges.Count;
			initList();
			Content.LoadAsync(base.onPageIconLoaded, ScrollPageIconKey);
		}

		protected override void initList()
		{
			clearContentItems();
			CoroutineRunner.Start(loadNewDailyItem(dailyChallenges[0], ScrollDirection.Right, 0), this, "LoadDailyChallengeItem");
			CoroutineRunner.Start(loadNewDailyItem(getNextDaily(ScrollDirection.Left, 1), ScrollDirection.Left, 1), this, "LoadDailyChallengeItem");
			CoroutineRunner.Start(loadNewDailyItem(getNextDaily(ScrollDirection.Right, 1), ScrollDirection.Right, 1), this, "LoadDailyChallengeItem");
		}

		protected override void loadNewItem(ScrollDirection direction)
		{
			CoroutineRunner.Start(loadNewDailyItem(getNextDaily(direction, 2), direction, 2), this, "LoadDailyItem");
		}

		private IEnumerator loadNewDailyItem(ClubPenguin.Task.Task task, ScrollDirection addDirection, int aheadCount)
		{
			AssetRequest<GameObject> request;
			if (task.Definition.DailyChallengeItemContentKey != null && !string.IsNullOrEmpty(task.Definition.DailyChallengeItemContentKey.Key))
			{
				request = Content.LoadAsync(task.Definition.DailyChallengeItemContentKey);
			}
			else
			{
				string subGroupByTaskName = Service.Get<TaskService>().GetSubGroupByTaskName(task.Id);
				request = Content.LoadAsync(itemPrefabContentKey, subGroupByTaskName);
			}
			yield return request;
			GameObject dailyObject = Object.Instantiate(request.Asset, ContentPanel, false);
			dailyObject.GetComponent<DailyChallengesListItem>().Init(task);
			yield return new WaitForEndOfFrame();
			addItem(dailyObject, addDirection, aheadCount);
		}

		private ClubPenguin.Task.Task getNextDaily(ScrollDirection direction, int aheadCount)
		{
			int nextItemIndex = getNextItemIndex(direction, aheadCount);
			return dailyChallenges[nextItemIndex];
		}
	}
}
