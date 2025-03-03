using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using ClubPenguin.Input;
using ClubPenguin.Net;
using ClubPenguin.Newsfeed;
using ClubPenguin.Task;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.Manimal.Common.Util;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenController : MonoBehaviour
	{
		public const string ACTIVITYSCREEN_DEFINITION_KEY = "Definitions/CellPhone/CellPhoneActivityScreenDefinition";

		public Text TimeText;

		public Text AmPmText;

		public Transform WidgetParentTransform;

		public StaticBreadcrumbDefinitionKey DailyChallengeBreadcrumb;

		public StaticBreadcrumbDefinitionKey NewsfeedBreadcrumb;

		private EventDispatcher dispatcher;

		private Timer updateTimer;

		private CPDataEntityCollection dataEntityCollection;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dispatcher.DispatchEvent(default(CellPhoneEvents.HideLoadingScreen));
			dispatcher.DispatchEvent(default(CellPhoneEvents.CellPhoneOpened));
			dispatcher.AddListener<TaskEvents.TaskRewardClaimed>(onTaskRewardClaimed, EventDispatcher.Priority.LAST);
			updateTimer = new Timer(1f, true, delegate
			{
				onTimerTick();
			});
			CoroutineRunner.Start(updateTimer.Start(), this, "");
			updateTime();
			loadActivityScreenDefinition();
			checkNewsFeed();
		}

		private void loadActivityScreenDefinition()
		{
			Content.LoadAsync<ScriptableObject>("Definitions/CellPhone/CellPhoneActivityScreenDefinition", onActivityScreenDefinitionLoaded);
		}

		private void onActivityScreenDefinitionLoaded(string Path, ScriptableObject definition)
		{
			DateTime date = Service.Get<ContentSchedulerService>().ScheduledEventDate();
			List<CellPhoneActivityDefinition> widgetDatas = new CellPhoneActivityScreenWidgetListBuilder(date, (CellPhoneActivityScreenDefinition)definition).Build();
			CoroutineRunner.Start(loadWidgets(widgetDatas), this, "loadWidgets");
		}

		private IEnumerator loadWidgets(List<CellPhoneActivityDefinition> widgetDatas)
		{
			CoroutineGroup coroutineGroup = new CoroutineGroup();
			Dictionary<CellPhoneActivityDefinition, GameObject> widgetDataToPrefab = new Dictionary<CellPhoneActivityDefinition, GameObject>();
			for (int i = 0; i < widgetDatas.Count; i++)
			{
				ICoroutine coroutine = CoroutineRunner.Start(loadWidget(widgetDatas[i], widgetDataToPrefab), this, "loadWidget");
				if (!coroutine.Disposed && !coroutine.Cancelled && !coroutine.Completed)
				{
					coroutineGroup.Add(coroutine);
				}
			}
			while (!coroutineGroup.IsFinished)
			{
				yield return null;
			}
			instantiateWidgets(widgetDatas, widgetDataToPrefab);
		}

		private void instantiateWidgets(List<CellPhoneActivityDefinition> widgetDatas, Dictionary<CellPhoneActivityDefinition, GameObject> widgetDataToPrefab)
		{
			for (int i = 0; i < widgetDatas.Count; i++)
			{
				if (widgetDataToPrefab.ContainsKey(widgetDatas[i]))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(widgetDataToPrefab[widgetDatas[i]], WidgetParentTransform);
					gameObject.GetComponent<ICellPhoneAcitivtyScreenWidget>().SetWidgetData(widgetDatas[i]);
				}
			}
		}

		private IEnumerator loadWidget(CellPhoneActivityDefinition widgetData, Dictionary<CellPhoneActivityDefinition, GameObject> widgetDataToPrefab)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(widgetData.WidgetPrefabKey);
			yield return request;
			widgetDataToPrefab.Add(widgetData, request.Asset);
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<TaskEvents.TaskRewardClaimed>(onTaskRewardClaimed);
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<NewPostData>>(onNewPostDataAdded);
		}

		public void OnCloseButtonPressed(ButtonClickListener.ClickType clickType)
		{
			dispatcher.DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
		}

		private void onTimerTick()
		{
			updateTime();
		}

		private void updateTime()
		{
			DateTime dateTime = Service.Get<INetworkServicesManager>().GameTimeMilliseconds.MsToDateTime();
			if (TimeText != null)
			{
				TimeText.text = dateTime.ToString("h:mm");
			}
			if (AmPmText != null)
			{
				if (dateTime.Hour > 12)
				{
					AmPmText.text = "PM";
				}
				else
				{
					AmPmText.text = "AM";
				}
			}
		}

		private bool onNewPostDataAdded(DataEntityEvents.ComponentAddedEvent<NewPostData> evt)
		{
			Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(NewsfeedBreadcrumb);
			return false;
		}

		private bool onTaskRewardClaimed(TaskEvents.TaskRewardClaimed evt)
		{
			Service.Get<NotificationBreadcrumbController>().RemoveBreadcrumb(DailyChallengeBreadcrumb);
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
			return false;
		}

		private void checkNewsFeed()
		{
			NewPostData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				Service.Get<NotificationBreadcrumbController>().AddBreadcrumb(NewsfeedBreadcrumb);
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<NewPostData>>(onNewPostDataAdded);
			}
		}
	}
}
