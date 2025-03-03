using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class RaycastMask : MonoBehaviour, ICanvasRaycastFilter
{
	private Image image;

	private Sprite sprite;

	private void Start()
	{
		image = GetComponent<Image>();
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		sprite = image.sprite;
		RectTransform rectTransform = (RectTransform)base.transform;
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)base.transform, sp, eventCamera, out localPoint);
		Vector2 vector = new Vector2(localPoint.x + rectTransform.pivot.x * rectTransform.rect.width, localPoint.y + rectTransform.pivot.y * rectTransform.rect.height);
		Rect textureRect = sprite.textureRect;
		Rect rect = rectTransform.rect;
		int num = 0;
		int num2 = 0;
		switch (image.type)
		{
		case Image.Type.Sliced:
		{
			Vector4 border = sprite.border;
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
			return sprite.texture.GetPixel(num, num2).a > 0f;
		}
		catch
		{
			Log.LogError(this, "Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled': " + base.gameObject.name);
			Object.Destroy(this);
			return false;
		}
	}
}
