using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class ShowNotificationAction : FsmStateAction
	{
		public string PrefabLocation;

		public string i18nMessage;

		public bool ContainsButtons;

		public bool AutoClose = true;

		public float PopUpDelayTime;

		public bool PersistBetweenScenes;

		public override void OnEnter()
		{
			DNotification dNotification = new DNotification();
			dNotification.PrefabLocation = new PrefabContentKey(PrefabLocation);
			dNotification.Message = Service.Get<Localizer>().GetTokenTranslation(i18nMessage);
			dNotification.ContainsButtons = ContainsButtons;
			dNotification.AutoClose = AutoClose;
			dNotification.PopUpDelayTime = PopUpDelayTime;
			dNotification.PersistBetweenScenes = PersistBetweenScenes;
			Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
			Finish();
		}
	}
}
