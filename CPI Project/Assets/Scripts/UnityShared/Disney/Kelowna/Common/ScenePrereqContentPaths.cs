using System.IO;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[CreateAssetMenu(menuName = "Scene/Prerequisite Content Paths", fileName = "ScenePrereq_[SceneName]")]
	public class ScenePrereqContentPaths : ScriptableObject
	{
		[Scene]
		[Tooltip("The Scene object to reference")]
		public string Scene;

		[Tooltip("The path to the folder content")]
		[FolderPath(true)]
		[Header("Path to the Required Content")]
		public ContentPath[] ContentPaths;

		public string SceneName
		{
			get
			{
				return Path.GetFileNameWithoutExtension(Scene);
			}
		}
	}
}
