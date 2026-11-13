using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public class CaptchaSolution
	{
		public string id;

		public CaptchaDimensions captchaDimensions;

		public List<SolutionPoint> solution;
	}
}
