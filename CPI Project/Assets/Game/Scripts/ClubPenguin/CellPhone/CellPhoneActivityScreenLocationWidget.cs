using ClubPenguin.Analytics;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenLocationWidget : AbstractCellPhoneActivityScreenMemberLockableWidget
	{
		[System.NonSerialized]
		private CellPhoneLocationActivityDefinition locationWidgetData;

		protected override void setWidgetData(CellPhoneActivityDefinition widgetData)
		{
			CellPhoneLocationActivityDefinition x = widgetData as CellPhoneLocationActivityDefinition;
			if (x != null)
			{
				this.locationWidgetData = x;
			}
		}

		protected override void onGoButtonClicked()
		{
			if (locationWidgetData != null)
			{
				goToLocationInZone();
			}
		}

		private void goToLocationInZone()
		{
			PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
			if (component != null)
			{
				SpawnedAction spawnedAction = new SpawnedAction();
				spawnedAction.Action = SpawnedAction.SPAWNED_ACTION.None;
				component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(locationWidgetData.LocationInZone).SceneName(locationWidgetData.Scene.SceneName).SpawnedAction(spawnedAction).Build());
				if (Service.Get<SceneTransitionService>().CurrentScene == locationWidgetData.Scene.SceneName)
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
				}
			}
			saveWidgetAsAccessed();
			logGoThereBI((locationWidgetData as CellPhoneFeatureActivityDefinition != null) ? "boost" : "event", locationWidgetData.name);
		}

		private void saveWidgetAsAccessed()
		{
			if (locationWidgetData.IsHiddenAfterAccessed)
			{
				CellPhoneActivityScreenDefinition.AccessedWidgets accessedWidgets = CellPhoneActivityScreenDefinition.AccessedWidgets.GetAccessedWidgets();
				if (!accessedWidgets.Widgets.Contains(locationWidgetData.name))
				{
					accessedWidgets.Widgets.Add(locationWidgetData.name);
					CellPhoneActivityScreenDefinition.AccessedWidgets.SaveAccessedWidgets(accessedWidgets);
				}
			}
		}

		private void logGoThereBI(string widgetType, string widgetDescription)
		{
			Service.Get<ICPSwrveService>().Action("activity_tracker", "go", widgetType, widgetDescription);
		}
	}
}
