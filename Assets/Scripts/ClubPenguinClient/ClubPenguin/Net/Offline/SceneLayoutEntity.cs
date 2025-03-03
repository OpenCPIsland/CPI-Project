using ClubPenguin.Net.Domain.Scene;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	public struct SceneLayoutEntity : IOfflineData
	{
		public List<SavedSceneLayout> Layouts;

		public SavedSceneLayout this[long value]
		{
			get
			{
				foreach (SavedSceneLayout layout in Layouts)
				{
					if (layout.layoutId == value)
					{
						return layout;
					}
				}
				return null;
			}
		}

		public void Init()
		{
			Layouts = new List<SavedSceneLayout>();
		}
	}
}
