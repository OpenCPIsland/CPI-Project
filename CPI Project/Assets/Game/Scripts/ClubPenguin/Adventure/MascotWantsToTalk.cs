using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class MascotWantsToTalk : FsmStateAction
	{
		public string MascotName;

		public bool ZoomIn;

		public bool ZoomOut;

		public bool LowerTray;

		public bool RestoreTray;

		public bool ShowIndicator;

		public bool SuppressQuestNotifier;

		public bool RestoreQuestNotifier;

		public bool MoveToTalkSpot = true;

		public bool OverrideInteracteeTxform;

		public FsmGameObject DesiredInteracteeTxform;

		public override void Reset()
		{
			ZoomIn = true;
			ZoomOut = true;
			LowerTray = true;
			RestoreTray = true;
			ShowIndicator = true;
			SuppressQuestNotifier = false;
			RestoreQuestNotifier = false;
			MoveToTalkSpot = true;
			OverrideInteracteeTxform = false;
			DesiredInteracteeTxform = new FsmGameObject
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(MascotName);
			mascot.InteractionBehaviours.ZoomIn = ZoomIn;
			mascot.InteractionBehaviours.ZoomOut = ZoomOut;
			mascot.InteractionBehaviours.LowerTray = LowerTray;
			mascot.InteractionBehaviours.RestoreTray = RestoreTray;
			mascot.InteractionBehaviours.ShowIndicator = ShowIndicator;
			mascot.InteractionBehaviours.SuppressQuestNotifier = SuppressQuestNotifier;
			mascot.InteractionBehaviours.RestoreQuestNotifier = RestoreQuestNotifier;
			mascot.InteractionBehaviours.MoveToTalkSpot = MoveToTalkSpot;
			if (DesiredInteracteeTxform.Value != null)
			{
				mascot.InteractionBehaviours.OverrideInteracteeTxform = OverrideInteracteeTxform;
				mascot.InteractionBehaviours.DesiredInteracteeTxform = DesiredInteracteeTxform.Value.transform;
			}
			else
			{
				mascot.InteractionBehaviours.OverrideInteracteeTxform = false;
			}
			mascot.WantsToTalk = true;
			Finish();
		}

		public override void OnExit()
		{
			Service.Get<MascotService>().GetMascot(MascotName).WantsToTalk = false;
		}
	}
}
