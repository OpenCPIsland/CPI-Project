namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("UI/Extensions/Raycast Mask")]
	public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
	{
		private Image _image;

		private Sprite _sprite;

		private void Start()
		{
			_image = GetComponent<Image>();
		}

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			_sprite = _image.sprite;
			RectTransform rectTransform = (RectTransform)base.transform;
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)base.transform, sp, eventCamera, out localPoint);
			float x = localPoint.x;
			Vector2 pivot = rectTransform.pivot;
			float x2 = x + pivot.x * rectTransform.rect.width;
			float y = localPoint.y;
			Vector2 pivot2 = rectTransform.pivot;
			Vector2 vector = new Vector2(x2, y + pivot2.y * rectTransform.rect.height);
			Rect textureRect = _sprite.textureRect;
			Rect rect = rectTransform.rect;
			int num = 0;
			int num2 = 0;
			switch (_image.type)
			{
			case Image.Type.Sliced:
			{
				Vector4 border = _sprite.border;
				num = ((vector.x < border.x) ? Mathf.FloorToInt(textureRect.x + vector.x) : ((!(vector.x > rect.width - border.z)) ? Mathf.FloorToInt(textureRect.x + border.x + (vector.x - border.x) / (rect.width - border.x - border.z) * (textureRect.width - border.x - border.z)) : Mathf.FloorToInt(textureRect.x + textureRect.width - (rect.width - vector.x))));
				num2 = ((!(vector.y < border.y)) ? ((!(vector.y > rect.height - border.w)) ? Mathf.FloorToInt(textureRect.y + border.y + (vector.y - border.y) / (rect.height - border.y - border.w) * (textureRect.height - border.y - border.w)) : Mathf.FloorToInt(textureRect.y + textureRect.height - (rect.height - vector.y))) : Mathf.FloorToInt(textureRect.y + vector.y));
				break;
			}
			default:
				num = Mathf.FloorToInt(textureRect.x + textureRect.width * vector.x / rect.width);
				num2 = Mathf.FloorToInt(textureRect.y + textureRect.height * vector.y / rect.height);
				break;
			}
			try
			{
				Color pixel = _sprite.texture.GetPixel(num, num2);
				return pixel.a > 0f;
			}
			catch (UnityException)
			{
				Debug.LogWarning("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'");
				Object.Destroy(this);
				return false;
			}
		}
	}
}
