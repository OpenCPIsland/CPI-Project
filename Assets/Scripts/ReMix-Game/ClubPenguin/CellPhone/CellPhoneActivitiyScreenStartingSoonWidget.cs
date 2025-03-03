using ClubPenguin.Analytics;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.Utils;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivitiyScreenStartingSoonWidget : AbstractCellPhoneActivityScreenMemberLockableWidget
	{
		public Text TimeRemainingText;

		private CellPhoneRecurringLocationActivityDefinition widgetData;

		private DateTime startTime;

		private bool isTimerRunning;

		public event System.Action CountdownCompleteAction;

		protected override void setWidgetData(CellPhoneActivityDefinition widgetData)
		{
			CellPhoneRecurringLocationActivityDefinition x = widgetData as CellPhoneRecurringLocationActivityDefinition;
			if (x != null)
			{
				this.widgetData = x;
				resetStartTime(this.widgetData.ActivityStartScheduleCron);
				CoroutineRunner.Start(updateTimer(), this, "updateTimer");
			}
		}

		protected override void onGoButtonClicked()
		{
			goToLocationInZone();
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			this.CountdownCompleteAction = null;
		}

		private void goToLocationInZone()
		{
			PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
			if (component != null)
			{
				SpawnedAction spawnedAction = new SpawnedAction();
				spawnedAction.Action = SpawnedAction.SPAWNED_ACTION.None;
				component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(widgetData.LocationInZone).SceneName(widgetData.Scene.SceneName).SpawnedAction(spawnedAction).Build());
				if (Service.Get<SceneTransitionService>().CurrentScene == widgetData.Scene.SceneName)
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
				}
			}
			Service.Get<ICPSwrveService>().Action("activity_tracker", "go", "starting_soon", widgetData.name);
		}

		private void resetStartTime(string scheduleCron)
		{
			CronExpressionEvaluator.EvaluatesTrue(Service.Get<ContentSchedulerService>().PresentTime(), scheduleCron, out startTime);
		}

		private IEnumerator updateTimer()
		{
			isTimerRunning = true;
			WaitForSeconds waitForOneSecond = new WaitForSeconds(1f);
			while (isTimerRunning)
			{
				TimeSpan remainingTime = startTime - Service.Get<ContentSchedulerService>().PresentTime();
				showTimerTime(remainingTime);
				yield return waitForOneSecond;
				if (remainingTime.TotalSeconds <= 0.0)
				{
					isTimerRunning = false;
					this.CountdownCompleteAction.InvokeSafe();
				}
			}
		}

		private void showTimerTime(TimeSpan time)
		{
			TimeRemainingText.text = string.Format("{0}:{1:00}", Math.Max((int)time.TotalMinutes, 0), Math.Max(time.Seconds, 0));
		}
	}
}
