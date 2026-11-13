using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Domain.Scene
{
	[Serializable]
	public class MutableSceneLayout
	{
		public string name;

		public string zoneId;

		public int lightingId;

		public int musicId;

		public Dictionary<string, string> extraInfo = new Dictionary<string, string>();

		public List<DecorationLayout> decorationsLayout;
	}
}
