using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class SolutionPoint
	{
		public int x;

		public int y;

		public CaptchaOrigin origin = CaptchaOrigin.BOTTOM_LEFT;
	}
}
