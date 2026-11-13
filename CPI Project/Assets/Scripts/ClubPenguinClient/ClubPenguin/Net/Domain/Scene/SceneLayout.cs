using System;

namespace ClubPenguin.Net.Domain.Scene
{
	[Serializable]
	public class SceneLayout : MutableSceneLayout
	{
		public long? createdDate;

		public long? lastModifiedDate;

		public bool memberOnly;
	}
}
