using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class OpenReportPlayerCommand
	{
		private DataEntityHandle handle;

		private PrefabContentKey ReportPlayerContentKey = new PrefabContentKey("ReportingBansPrefabs/ReportPopupBackgroundPrefab");

		public OpenReportPlayerCommand(DataEntityHandle handle)
		{
			this.handle = handle;
		}

		public void Execute()
		{
			Content.LoadAsync(onReportPlayerLoaded, ReportPlayerContentKey);
		}

		private void onReportPlayerLoaded(string path, GameObject reportPlayerPrefab)
		{
			ReportPlayerController component = Object.Instantiate(reportPlayerPrefab).GetComponent<ReportPlayerController>();
			component.Initialize(handle);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(component.gameObject, false, true, "Accessibility.Popup.Title.ReportPlayer"));
		}
	}
}
