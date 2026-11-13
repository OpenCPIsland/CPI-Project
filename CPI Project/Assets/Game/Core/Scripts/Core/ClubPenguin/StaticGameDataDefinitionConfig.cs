using UnityEngine;

namespace ClubPenguin
{
	[CreateAssetMenu]
	public class StaticGameDataDefinitionConfig : ScriptableObject
	{
		public string DefinitionPath;

		public string ExportPath;

		public string ManifestPath;

		public bool IsExportable = true;
	}
}
