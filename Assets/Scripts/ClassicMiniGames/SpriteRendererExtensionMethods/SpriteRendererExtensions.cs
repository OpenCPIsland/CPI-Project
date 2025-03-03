using UnityEngine;

namespace SpriteRendererExtensionMethods
{
	public static class SpriteRendererExtensions
	{
		public static void AlignSpriteToRightOf(this SpriteRenderer _toMove, SpriteRenderer _targetToAlignWith)
		{
			Bounds bounds = _targetToAlignWith.bounds;
			_toMove.transform.position = new Vector3(bounds.max.x, _toMove.transform.position.y, 0f);
		}
	}
}
