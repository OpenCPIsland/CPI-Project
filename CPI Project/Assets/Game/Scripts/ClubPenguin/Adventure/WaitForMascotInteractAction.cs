using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class WaitForMascotInteractAction : FsmStateAction
	{
		public FsmEvent InteractEvent;

		public MascotDefinition Mascot;

		public bool DisableDefaultInteract = true;

		private Mascot mascotComponent;

		public override void OnEnter()
		{
			mascotComponent = Service.Get<MascotService>().GetMascot(Mascot.name);
			mascotComponent.IsDefaultInteractDisabled = DisableDefaultInteract;
			mascotComponent.InteractionBehaviours.ZoomIn = false;
			mascotComponent.InteractionBehaviours.ZoomOut = false;
			mascotComponent.InteractionBehaviours.LowerTray = false;
			mascotComponent.InteractionBehaviours.RestoreTray = false;
			mascotComponent.InteractionBehaviours.ShowIndicator = false;
			mascotComponent.InteractionBehaviours.SuppressQuestNotifier = false;
			mascotComponent.InteractionBehaviours.RestoreQuestNotifier = false;
			mascotComponent.InteractionBehaviours.MoveToTalkSpot = false;
			mascotComponent.InteractionBehaviours.OverrideInteracteeTxform = false;
			Service.Get<EventDispatcher>().AddListener<QuestEvents.OnMascotInteract>(onMascotInteract);
		}

		public bool onMascotInteract(QuestEvents.OnMascotInteract evt)
		{
			if (evt.Mascot.Name == Mascot.name)
			{
				base.Fsm.Event(InteractEvent);
			}
			return false;
		}

		public override void OnExit()
		{
			mascotComponent.IsDefaultInteractDisabled = false;
			Service.Get<EventDispatcher>().RemoveListener<QuestEvents.OnMascotInteract>(onMascotInteract);
		}
	}
}
