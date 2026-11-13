using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public static class UnityExtensions
	{
		public static string GetPath(this Transform current)
		{
			if (current.parent == null)
			{
				return current.name;
			}
			return current.parent.GetPath() + "/" + current.name;
		}

		public static string GetPath(this Component current)
		{
			if (current.transform.parent == null)
			{
				return current.name;
			}
			return current.transform.parent.GetPath() + "/" + current.name;
		}
	}
}
