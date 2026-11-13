using ClubPenguin.ClothingDesigner;
using ClubPenguin.ClothingDesigner.ItemCustomizer;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class WaitForCustomizerEventAction : FsmStateAction
	{
		public enum CustomizeType
		{
			Fabric,
			Decal,
			Template
		}

		public CustomizeType Type;

		public FsmEvent StartDragEvent;

		public FsmEvent EndDragEvent;

		public FsmEvent ApplyEvent;

		public override void OnEnter()
		{
			switch (Type)
			{
			case CustomizeType.Fabric:
				CustomizationContext.EventBus.AddListener<CustomizerDragEvents.StartDragFabricButton>(onStartDragFabric);
				CustomizationContext.EventBus.AddListener<CustomizerDragEvents.EndDragFabricButton>(onEndDragFabric);
				CustomizationContext.EventBus.AddListener<CustomizerUIEvents.OnApplyFabric>(onApplyFabric);
				break;
			case CustomizeType.Decal:
				CustomizationContext.EventBus.AddListener<CustomizerDragEvents.StartDragDecalButton>(onStartDragDecal);
				CustomizationContext.EventBus.AddListener<CustomizerDragEvents.EndDragDecalButton>(onEndDragDecal);
				CustomizationContext.EventBus.AddListener<CustomizerUIEvents.OnApplyDecal>(onApplyDecal);
				break;
			case CustomizeType.Template:
				CustomizationContext.EventBus.AddListener<ClothingDesignerUIEvents.CustomizerChosenState>(onCustomizerChosenState);
				break;
			}
		}

		public override void OnExit()
		{
			switch (Type)
			{
			case CustomizeType.Fabric:
				CustomizationContext.EventBus.RemoveListener<CustomizerDragEvents.StartDragFabricButton>(onStartDragFabric);
				CustomizationContext.EventBus.RemoveListener<CustomizerDragEvents.EndDragFabricButton>(onEndDragFabric);
				CustomizationContext.EventBus.RemoveListener<CustomizerUIEvents.OnApplyFabric>(onApplyFabric);
				break;
			case CustomizeType.Decal:
				CustomizationContext.EventBus.RemoveListener<CustomizerDragEvents.StartDragDecalButton>(onStartDragDecal);
				CustomizationContext.EventBus.RemoveListener<CustomizerDragEvents.EndDragDecalButton>(onEndDragDecal);
				CustomizationContext.EventBus.RemoveListener<CustomizerUIEvents.OnApplyDecal>(onApplyDecal);
				break;
			case CustomizeType.Template:
				CustomizationContext.EventBus.RemoveListener<ClothingDesignerUIEvents.CustomizerChosenState>(onCustomizerChosenState);
				break;
			}
		}

		private bool onStartDragFabric(CustomizerDragEvents.StartDragFabricButton evt)
		{
			base.Fsm.Event(StartDragEvent);
			return false;
		}

		private bool onEndDragFabric(CustomizerDragEvents.EndDragFabricButton evt)
		{
			base.Fsm.Event(EndDragEvent);
			return false;
		}

		private bool onApplyFabric(CustomizerUIEvents.OnApplyFabric evt)
		{
			base.Fsm.Event(ApplyEvent);
			return false;
		}

		private bool onStartDragDecal(CustomizerDragEvents.StartDragDecalButton evt)
		{
			base.Fsm.Event(StartDragEvent);
			return false;
		}

		private bool onEndDragDecal(CustomizerDragEvents.EndDragDecalButton evt)
		{
			base.Fsm.Event(EndDragEvent);
			return false;
		}

		private bool onApplyDecal(CustomizerUIEvents.OnApplyDecal evt)
		{
			base.Fsm.Event(ApplyEvent);
			return false;
		}

		private bool onCustomizerChosenState(ClothingDesignerUIEvents.CustomizerChosenState evt)
		{
			base.Fsm.Event(ApplyEvent);
			return false;
		}
	}
}
