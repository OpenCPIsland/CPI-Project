using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public static class TileSettings
	{
		[Tweakable("Tweaker.UI.SelectedTileScale", Description = "Display scale of tile when it is selected")]
		public static Vector3 selectedTileScale = new Vector3(1.6f, 1.6f, 1f);

		[Tweakable("Tweaker.UI.DeselectedTileScale", Description = "Display scale of tile when it is not selected")]
		public static Vector3 deselectedTileScale = new Vector3(0.95f, 0.95f, 1f);
	}
}
