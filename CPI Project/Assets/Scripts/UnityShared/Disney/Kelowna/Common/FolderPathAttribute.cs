using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class FolderPathAttribute : PropertyAttribute
	{
		public bool isResouceFolder;

		public FolderPathAttribute(bool isResource)
		{
			isResouceFolder = isResource;
		}
	}
}
