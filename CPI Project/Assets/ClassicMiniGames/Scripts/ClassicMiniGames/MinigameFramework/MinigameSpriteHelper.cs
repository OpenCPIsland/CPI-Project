using UnityEngine;

namespace MinigameFramework
{
	public static class MinigameSpriteHelper
	{
		private static Color[] _availableColors = new Color[15]
		{
			new Color(0f, 0.2f, 0.4f),
			new Color(0f, 0.6f, 0f),
			new Color(1f, 0.2f, 0.6f),
			new Color(0.2f, 0.2f, 0.2f),
			new Color(0.8f, 0f, 0f),
			new Color(1f, 0.4f, 0f),
			new Color(1f, 0.8f, 0f),
			new Color(0.4f, 0f, 0.6f),
			new Color(0.6f, 0.4f, 0f),
			new Color(1f, 0.4f, 0.4f),
			new Color(0f, 0.4f, 0f),
			new Color(0f, 0.6f, 0.8f),
			new Color(46f / 85f, 227f / 255f, 2f / 255f),
			new Color(2f / 255f, 167f / 255f, 151f / 255f),
			new Color(1f, 1f, 1f)
		};

		public static void FitSpriteToScreen(Camera _camera, GameObject _sprite, bool _preserveAspect = true)
		{
			SpriteRenderer componentInChildren = _sprite.GetComponentInChildren<SpriteRenderer>();
			if (!(componentInChildren == null))
			{
				_sprite.transform.localScale = CalculateScaleToFitScreen(_camera, componentInChildren.sprite.bounds.size, _preserveAspect);
			}
		}

		public static Vector2 CalculateScaleToFitScreen(Camera _camera, Vector2 _imageSize, bool _preserveAspect = true)
		{
			if (!_camera.orthographic)
			{
				return new Vector2(0f, 0f);
			}
			float num = _camera.orthographicSize * 2f;
			float num2 = num / (float)Screen.height * (float)Screen.width;
			float num3 = num / _imageSize.y;
			float x = _preserveAspect ? num3 : (num2 / _imageSize.x);
			return new Vector3(x, num3);
		}

		public static void AssignParentPositionReset(GameObject _child, GameObject _parent)
		{
			Vector2 v = _child.transform.localPosition;
			AssignParent(_child, _parent);
			_child.transform.localPosition = v;
		}

		public static void AssignParent(GameObject _child, GameObject _parent)
		{
			AssignParentTransform(_child, _parent.transform);
		}

		public static void AssignParentTransform(GameObject _child, Transform _parent)
		{
			Vector3 localScale = _child.transform.localScale;
			_child.transform.parent = _parent;
			_child.transform.localScale = localScale;
		}

		public static void SetSpriteLayer(GameObject _sprite, int _layer)
		{
			SpriteRenderer component = _sprite.GetComponent<SpriteRenderer>();
			if (!(component == null))
			{
				component.sortingOrder = _layer;
			}
		}

		public static GameObject CreateContainer(string _name)
		{
			GameObject gameObject = new GameObject(_name);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			return gameObject;
		}

		public static Vector2 GetScreenEdge(EScreenEdge _edge, Camera _camera)
		{
			Vector2 result = new Vector2(0f, 0f);
			switch (_edge)
			{
			case EScreenEdge.RIGHT:
				result.x = _camera.aspect * _camera.orthographicSize;
				break;
			case EScreenEdge.TOP:
				result.y = _camera.orthographicSize;
				break;
			case EScreenEdge.BOTTOM:
				result.y = 0f - _camera.orthographicSize;
				break;
			case EScreenEdge.LEFT:
				result.x = (0f - _camera.orthographicSize) * _camera.aspect;
				break;
			}
			return result;
		}

		public static void ChangeSpriteColor(GameObject _sprite, Color _color)
		{
			SpriteRenderer component = _sprite.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				component.color = _color;
			}
		}

		public static Color RandomPenguinColor()
		{
			int num = Random.Range(0, _availableColors.Length);
			return _availableColors[num];
		}
	}
}
