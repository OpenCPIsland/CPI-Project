using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[DisallowMultipleComponent]
	public class StoreItemConfirmationPlacement : MonoBehaviour
	{
		[SerializeField]
		private Image topPointer = null;

		[SerializeField]
		private Image bottomPointer = null;

		[SerializeField]
		private float yPadding = 15f;

		private RectTransform scrollRectTransform;

		public void PositionConfirmation(RectTransform shopItem, RectTransform scrollRectTransform)
		{
			this.scrollRectTransform = scrollRectTransform;
			setConfirmationPosition(shopItem);
		}

		private void setConfirmationPosition(RectTransform shopItem)
		{
			float num = calculateYOffset(shopItem);
			float num2 = shopItem.position.y + num;
			float num3 = calculateXOffset(shopItem, num2);
			float x = shopItem.position.x + num3;
			base.transform.position = new Vector3(x, num2, shopItem.position.z);
			showPointer(num3, num);
		}

		private float calculateYOffset(RectTransform shopItem)
		{
			float num = shopItem.rect.height * 0.5f;
			float num2 = ((RectTransform)base.transform).rect.height * 0.5f;
			float num3 = num2 * base.transform.lossyScale.y;
			float num4 = (0f - (num2 + num + yPadding)) * base.transform.lossyScale.y;
			float y = shopItem.position.y + num4 - num3;
			if (!RectTransformUtility.RectangleContainsScreenPoint(scrollRectTransform, new Vector2(shopItem.position.x, y)))
			{
				num4 *= -1f;
			}
			return num4;
		}

		private float calculateXOffset(RectTransform shopItem, float yPosition)
		{
			float x = base.transform.lossyScale.x;
			float num = ((RectTransform)base.transform).rect.width * 0.5f;
			float num2 = 0f;
			float num3 = shopItem.position.x + num2;
			if (!RectTransformUtility.RectangleContainsScreenPoint(scrollRectTransform, new Vector2(num3 - num * x, yPosition)) && !RectTransformUtility.RectangleContainsScreenPoint(scrollRectTransform, new Vector2(num3 - num * x, shopItem.position.y)))
			{
				float num4 = shopItem.rect.width * 0.5f;
				num2 = (num - num4) * x;
			}
			else if (!RectTransformUtility.RectangleContainsScreenPoint(scrollRectTransform, new Vector2(num3 + num * x, yPosition)) && !RectTransformUtility.RectangleContainsScreenPoint(scrollRectTransform, new Vector2(num3 + num * x, shopItem.position.y)))
			{
				float num4 = shopItem.rect.width * 0.5f;
				num2 = (0f - (num - num4)) * x;
			}
			return num2;
		}

		private void showPointer(float xOffset, float yOffset)
		{
			bool flag = yOffset <= 0f;
			float x = (0f - xOffset) / base.transform.lossyScale.x;
			if (topPointer != null)
			{
				topPointer.enabled = flag;
				if (topPointer.enabled)
				{
					topPointer.gameObject.transform.localPosition = new Vector2(x, topPointer.gameObject.transform.localPosition.y);
				}
			}
			if (bottomPointer != null)
			{
				bottomPointer.enabled = !flag;
				if (bottomPointer.enabled)
				{
					bottomPointer.gameObject.transform.localPosition = new Vector2(x, bottomPointer.gameObject.transform.localPosition.y);
				}
			}
		}
	}
}
