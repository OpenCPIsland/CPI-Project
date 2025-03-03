using ClubPenguin.Analytics;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneActivityScreenLocationWidget : AbstractCellPhoneActivityScreenMemberLockableWidget
	{
		private CellPhoneLocationActivityDefinition widgetData;

		protected override void setWidgetData(CellPhoneActivityDefinition widgetData)
		{
			CellPhoneLocationActivityDefinition x = widgetData as CellPhoneLocationActivityDefinition;
			if (x != null)
			{
				this.widgetData = x;
			}
		}

		protected override void onGoButtonClicked()
		{
			if (widgetData != null)
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
				component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(widgetData.LocationInZone).SceneName(widgetData.Scene.SceneName).SpawnedAction(spawnedAction).Build());
				if (Service.Get<SceneTransitionService>().CurrentScene == widgetData.Scene.SceneName)
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
				}
			}
			saveWidgetAsAccessed();
			logGoThereBI((widgetData as CellPhoneFeatureActivityDefinition != null) ? "boost" : "event", widgetData.name);
		}

		private void saveWidgetAsAccessed()
		{
			if (widgetData.IsHiddenAfterAccessed)
			{
				CellPhoneActivityScreenDefinition.AccessedWidgets accessedWidgets = CellPhoneActivityScreenDefinition.AccessedWidgets.GetAccessedWidgets();
				if (!accessedWidgets.Widgets.Contains(widgetData.name))
				{
					accessedWidgets.Widgets.Add(widgetData.name);
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
