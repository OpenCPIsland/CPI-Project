using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.SpecialEvents
{
	[Serializable]
	public class ScheduledCutSceneData
	{
		[Scene]
		public string CutSceneAdditiveScene;

		public string PlayedKeyName;
	}
}
