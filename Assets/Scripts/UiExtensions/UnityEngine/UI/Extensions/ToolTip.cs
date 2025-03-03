namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/Tooltip")]
	public class ToolTip : MonoBehaviour
	{
		private Text _text;

		private RectTransform _rectTransform;

		private bool _inside;

		private float width;

		private float height;

		private float YShift;

		private float xShift;

		private RenderMode _guiMode;

		private Camera _guiCamera;

		public void Awake()
		{
			Canvas componentInParent = GetComponentInParent<Canvas>();
			_guiCamera = componentInParent.worldCamera;
			_guiMode = componentInParent.renderMode;
			_rectTransform = GetComponent<RectTransform>();
			_text = GetComponentInChildren<Text>();
			_inside = false;
			xShift = 0f;
			YShift = -30f;
			base.gameObject.SetActive(false);
		}

		public void SetTooltip(string ttext)
		{
			if (_guiMode == RenderMode.ScreenSpaceCamera)
			{
				_text.text = ttext;
				_rectTransform.sizeDelta = new Vector2(_text.preferredWidth + 40f, _text.preferredHeight + 25f);
				OnScreenSpaceCamera();
			}
		}

		public void HideTooltip()
		{
			if (_guiMode == RenderMode.ScreenSpaceCamera)
			{
				base.gameObject.SetActive(false);
				_inside = false;
			}
		}

		private void FixedUpdate()
		{
			if (_inside && _guiMode == RenderMode.ScreenSpaceCamera)
			{
				OnScreenSpaceCamera();
			}
		}

		public void OnScreenSpaceCamera()
		{
			Vector3 position = _guiCamera.ScreenToViewportPoint(Input.mousePosition - new Vector3(xShift, YShift, 0f));
			Vector3 vector = _guiCamera.ViewportToWorldPoint(position);
			width = _rectTransform.sizeDelta[0];
			height = _rectTransform.sizeDelta[1];
			Vector3 vector2 = _guiCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
			Vector3 vector3 = _guiCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
			float num = vector.x + width / 2f;
			if (num > vector3.x)
			{
				Vector3 vector4 = new Vector3(num - vector3.x, 0f, 0f);
				Vector3 position2 = new Vector3(vector.x - vector4.x, position.y, 0f);
				Vector3 vector5 = _guiCamera.WorldToViewportPoint(position2);
				position.x = vector5.x;
			}
			num = vector.x - width / 2f;
			if (num < vector2.x)
			{
				Vector3 vector6 = new Vector3(vector2.x - num, 0f, 0f);
				Vector3 position3 = new Vector3(vector.x + vector6.x, position.y, 0f);
				Vector3 vector7 = _guiCamera.WorldToViewportPoint(position3);
				position.x = vector7.x;
			}
			num = vector.y + height / 2f;
			if (num > vector3.y)
			{
				Vector3 vector8 = new Vector3(0f, 35f + height / 2f, 0f);
				Vector3 position4 = new Vector3(position.x, vector.y - vector8.y, 0f);
				Vector3 vector9 = _guiCamera.WorldToViewportPoint(position4);
				position.y = vector9.y;
			}
			num = vector.y - height / 2f;
			if (num < vector2.y)
			{
				Vector3 vector10 = new Vector3(0f, 35f + height / 2f, 0f);
				Vector3 position5 = new Vector3(position.x, vector.y + vector10.y, 0f);
				Vector3 vector11 = _guiCamera.WorldToViewportPoint(position5);
				position.y = vector11.y;
			}
			base.transform.position = new Vector3(vector.x, vector.y, 0f);
			base.gameObject.SetActive(true);
			_inside = true;
		}
	}
}
