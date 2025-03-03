using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Utils;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	[RequireComponent(typeof(LayoutElement))]
	public class CellPhoneActivityScreenRecurringWidgetLoader : MonoBehaviour, ICellPhoneAcitivtyScreenWidget
	{
		private CellPhoneActivitiyScreenStartingSoonWidget currentLoadedWidget;

		private List<CellPhoneRecurringLocationActivityDefinition> recurringActivityDefinitions;

		public void SetWidgetData(CellPhoneActivityDefinition widgetData)
		{
		}

		private void Start()
		{
			loadActivityScreenDefinition();
		}

		private void loadActivityScreenDefinition()
		{
			Content.LoadAsync<ScriptableObject>("Definitions/CellPhone/CellPhoneActivityScreenDefinition", onActivityScreenDefinitionLoaded);
		}

		private void onActivityScreenDefinitionLoaded(string Path, ScriptableObject definition)
		{
			recurringActivityDefinitions = ((CellPhoneActivityScreenDefinition)definition).ScheduledRecurringActivities;
			CoroutineRunner.Start(loadWidget(), this, "loadLatestWidget");
		}

		private IEnumerator loadWidget()
		{
			CellPhoneRecurringLocationActivityDefinition widgetDefinition = GetActiveRecurringActivityDefinition(recurringActivityDefinitions);
			if (widgetDefinition != null)
			{
				AssetRequest<GameObject> assetRequest = Content.LoadAsync(widgetDefinition.WidgetPrefabKey);
				yield return assetRequest;
				removeCurrentWidget();
				instantiateWidget(assetRequest.Asset, widgetDefinition);
			}
			else
			{
				removeCurrentWidget();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void instantiateWidget(GameObject widgetPrefab, CellPhoneRecurringLocationActivityDefinition definition)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(widgetPrefab, base.transform, false);
			currentLoadedWidget = gameObject.GetComponent<CellPhoneActivitiyScreenStartingSoonWidget>();
			currentLoadedWidget.SetWidgetData(definition);
			currentLoadedWidget.CountdownCompleteAction += onWidgetCountdownComplete;
		}

		private void onWidgetCountdownComplete()
		{
			currentLoadedWidget.CountdownCompleteAction -= onWidgetCountdownComplete;
			CoroutineRunner.Start(loadWidget(), this, "loadLatestWidget");
		}

		private void removeCurrentWidget()
		{
			if (currentLoadedWidget != null)
			{
				UnityEngine.Object.Destroy(currentLoadedWidget.gameObject);
				currentLoadedWidget.CountdownCompleteAction -= onWidgetCountdownComplete;
				currentLoadedWidget = null;
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (currentLoadedWidget != null)
			{
				currentLoadedWidget.CountdownCompleteAction -= onWidgetCountdownComplete;
			}
		}

		public static CellPhoneRecurringLocationActivityDefinition GetActiveRecurringActivityDefinition(List<CellPhoneRecurringLocationActivityDefinition> activities)
		{
			CellPhoneRecurringLocationActivityDefinition result = null;
			for (int i = 0; i < activities.Count; i++)
			{
				DateTime dateTime = Service.Get<ContentSchedulerService>().ScheduledEventDate();
				if (CronExpressionEvaluator.EvaluatesTrue(dateTime, activities[i].ShowWidgetScheduleCron) && DateTimeUtils.DoesDateFallBetween(dateTime, activities[i].GetStartingDate().Date, activities[i].GetEndingDate().Date))
				{
					result = activities[i];
					break;
				}
			}
			return result;
		}
	}
}
