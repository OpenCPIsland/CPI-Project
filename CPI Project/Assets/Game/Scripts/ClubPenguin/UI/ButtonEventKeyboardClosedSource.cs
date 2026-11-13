using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.UI
{
	public class ButtonEventKeyboardClosedSource : ButtonEventSource
	{
		protected override void OnDisable()
		{
			base.OnDisable();
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardHidden>(onKeyboardClosed);
		}

		protected override void handleEvent()
		{
			if (ClubPenguin.Core.SceneRefs.Get<IScreenContainerStateHandler>().IsKeyboardShown)
			{
				Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardHidden>(onKeyboardClosed);
			}
			else
			{
				base.handleEvent();
			}
		}

		private bool onKeyboardClosed(KeyboardEvents.KeyboardHidden evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardHidden>(onKeyboardClosed);
			sendEvent();
			return false;
		}
	}
}
