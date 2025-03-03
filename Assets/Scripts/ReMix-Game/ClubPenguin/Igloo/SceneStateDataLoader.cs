using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	public class SceneStateDataLoader : MonoBehaviour
	{
		public SceneStateData.SceneState HandledState;

		private void Start()
		{
			SceneLayoutDataManager sceneLayoutDataManager = Service.Get<SceneLayoutDataManager>();
			switch (HandledState)
			{
			case SceneStateData.SceneState.Preview:
			case SceneStateData.SceneState.StructurePlacement:
				break;
			case SceneStateData.SceneState.Create:
				if (sceneLayoutDataManager.GetActiveSceneLayoutData() == null)
				{
					sceneLayoutDataManager.AddNewActiveLayout();
				}
				break;
			case SceneStateData.SceneState.Play:
			case SceneStateData.SceneState.Edit:
				if (sceneLayoutDataManager.HasCachedLayoutData())
				{
					sceneLayoutDataManager.RestoreCachedLayout();
				}
				break;
			}
		}
	}
}
