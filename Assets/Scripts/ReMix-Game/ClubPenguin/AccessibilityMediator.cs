using ClubPenguin.Accessibility;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;

namespace ClubPenguin
{
	public class AccessibilityMediator
	{
		private readonly EventDispatcher eventDispatcher;

		public AccessibilityMediator(EventDispatcher eventDispatcher)
		{
			this.eventDispatcher = eventDispatcher;
			eventDispatcher.AddListener<AccessibilityEvents.AccessibilityScaleUpdated>(onAccessibilityScaleUpdated);
		}

		private bool onAccessibilityScaleUpdated(AccessibilityEvents.AccessibilityScaleUpdated evt)
		{
			eventDispatcher.DispatchEvent(new UIEvents.CanvasScalerModifierUpdated(evt.Scale));
			return false;
		}
	}
}
