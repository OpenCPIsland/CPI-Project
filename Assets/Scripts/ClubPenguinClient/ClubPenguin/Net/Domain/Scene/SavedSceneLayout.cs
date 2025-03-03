using System;

namespace ClubPenguin.Net.Domain.Scene
{
	[Serializable]
	public class SavedSceneLayout : SceneLayout
	{
		public long layoutId;

		public static SavedSceneLayout FromSceneLayout(SceneLayout layout, long sceneLayoutId)
		{
			SavedSceneLayout savedSceneLayout = new SavedSceneLayout();
			savedSceneLayout.createdDate = layout.createdDate;
			savedSceneLayout.decorationsLayout = layout.decorationsLayout;
			savedSceneLayout.extraInfo = layout.extraInfo;
			savedSceneLayout.lastModifiedDate = layout.lastModifiedDate;
			savedSceneLayout.layoutId = sceneLayoutId;
			savedSceneLayout.lightingId = layout.lightingId;
			savedSceneLayout.musicId = layout.musicId;
			savedSceneLayout.memberOnly = layout.memberOnly;
			savedSceneLayout.name = layout.name;
			savedSceneLayout.zoneId = layout.zoneId;
			return savedSceneLayout;
		}
	}
}
