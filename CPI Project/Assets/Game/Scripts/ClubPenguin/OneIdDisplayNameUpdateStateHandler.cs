using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Runtime.InteropServices;

namespace ClubPenguin
{
	public class OneIdDisplayNameUpdateStateHandler : AbstractAccountStateHandler
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct OneIdDisplayNameUpdateStarted
		{
		}

		public void OnStateChanged(string state)
		{
			if (state == HandledState)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(OneIdDisplayNameUpdateStarted));
			}
		}
	}
}
