using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/Extensions/BoxSlider")]
	public class BoxSlider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement, IEventSystemHandler
	{
		public enum Direction
		{
			LeftToRight,
			RightToLeft,
			BottomToTop,
			TopToBottom
		}

		[Serializable]
		public class BoxSliderEvent : UnityEvent<float, float>
		{
		}

		private enum Axis
		{
			Horizontal,
			Vertical
		}

		[SerializeField]
		private RectTransform m_HandleRect;

		[Space(6f)]
		[SerializeField]
		private float m_MinValue = 0f;

		[SerializeField]
		private float m_MaxValue = 1f;

		[SerializeField]
		private bool m_WholeNumbers = false;

		[SerializeField]
		private float m_ValueX = 1f;

		[SerializeField]
		private float m_ValueY = 1f;

		[Space(6f)]
		[SerializeField]
		private BoxSliderEvent m_OnValueChanged = new BoxSliderEvent();

		private Transform m_HandleTransform;

		private RectTransform m_HandleContainerRect;

		private Vector2 m_Offset = Vector2.zero;

		private DrivenRectTransformTracker m_Tracker;

		public RectTransform HandleRect
		{
			get
			{
				return m_HandleRect;
			}
			set
			{
				if (SetClass(ref m_HandleRect, value))
				{
					UpdateCachedReferences();
					UpdateVisuals();
				}
			}
		}

		public float MinValue
		{
			get
			{
				return m_MinValue;
			}
			set
			{
				if (SetStruct(ref m_MinValue, value))
				{
					SetX(m_ValueX);
					SetY(m_ValueY);
					UpdateVisuals();
				}
			}
		}

		public float MaxValue
		{
			get
			{
				return m_MaxValue;
			}
			set
			{
				if (SetStruct(ref m_MaxValue, value))
				{
					SetX(m_ValueX);
					SetY(m_ValueY);
					UpdateVisuals();
				}
			}
		}

		public bool WholeNumbers
		{
			get
			{
				return m_WholeNumbers;
			}
			set
			{
				if (SetStruct(ref m_WholeNumbers, value))
				{
					SetX(m_ValueX);
					SetY(m_ValueY);
					UpdateVisuals();
				}
			}
		}

		public float ValueX
		{
			get
			{
				if (WholeNumbers)
				{
					return Mathf.Round(m_ValueX);
				}
				return m_ValueX;
			}
			set
			{
				SetX(value);
			}
		}

		public float NormalizedValueX
		{
			get
			{
				if (Mathf.Approximately(MinValue, MaxValue))
				{
					return 0f;
				}
				return Mathf.InverseLerp(MinValue, MaxValue, ValueX);
			}
			set
			{
				ValueX = Mathf.Lerp(MinValue, MaxValue, value);
			}
		}

		public float ValueY
		{
			get
			{
				if (WholeNumbers)
				{
					return Mathf.Round(m_ValueY);
				}
				return m_ValueY;
			}
			set
			{
				SetY(value);
			}
		}

		public float NormalizedValueY
		{
			get
			{
				if (Mathf.Approximately(MinValue, MaxValue))
				{
					return 0f;
				}
				return Mathf.InverseLerp(MinValue, MaxValue, ValueY);
			}
			set
			{
				ValueY = Mathf.Lerp(MinValue, MaxValue, value);
			}
		}

		public BoxSliderEvent OnValueChanged
		{
			get
			{
				return m_OnValueChanged;
			}
			set
			{
				m_OnValueChanged = value;
			}
		}

		private float StepSize
		{
			get
			{
				return (!WholeNumbers) ? ((MaxValue - MinValue) * 0.1f) : 1f;
			}
		}

		protected BoxSlider()
		{
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
		}

		public void LayoutComplete()
		{
		}

		public void GraphicUpdateComplete()
		{
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateCachedReferences();
			SetX(m_ValueX, false);
			SetY(m_ValueY, false);
			UpdateVisuals();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			base.OnDisable();
		}

		private void UpdateCachedReferences()
		{
			if ((bool)m_HandleRect)
			{
				m_HandleTransform = m_HandleRect.transform;
				if (m_HandleTransform.parent != null)
				{
					m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
				}
			}
			else
			{
				m_HandleContainerRect = null;
			}
		}

		private void SetX(float input)
		{
			SetX(input, true);
		}

		private void SetX(float input, bool sendCallback)
		{
			float num = Mathf.Clamp(input, MinValue, MaxValue);
			if (WholeNumbers)
			{
				num = Mathf.Round(num);
			}
			if (m_ValueX != num)
			{
				m_ValueX = num;
				UpdateVisuals();
				if (sendCallback)
				{
					m_OnValueChanged.Invoke(num, ValueY);
				}
			}
		}

		private void SetY(float input)
		{
			SetY(input, true);
		}

		private void SetY(float input, bool sendCallback)
		{
			float num = Mathf.Clamp(input, MinValue, MaxValue);
			if (WholeNumbers)
			{
				num = Mathf.Round(num);
			}
			if (m_ValueY != num)
			{
				m_ValueY = num;
				UpdateVisuals();
				if (sendCallback)
				{
					m_OnValueChanged.Invoke(ValueX, num);
				}
			}
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			UpdateVisuals();
		}

		private void UpdateVisuals()
		{
			m_Tracker.Clear();
			if (m_HandleContainerRect != null)
			{
				m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
				Vector2 zero = Vector2.zero;
				Vector2 one = Vector2.one;
				float num2 = zero[0] = (one[0] = NormalizedValueX);
				num2 = (zero[1] = (one[1] = NormalizedValueY));
				m_HandleRect.anchorMin = zero;
				m_HandleRect.anchorMax = one;
			}
		}

		private void UpdateDrag(PointerEventData eventData, Camera cam)
		{
			RectTransform handleContainerRect = m_HandleContainerRect;
			Vector2 localPoint;
			if (handleContainerRect != null && handleContainerRect.rect.size[0] > 0f && RectTransformUtility.ScreenPointToLocalPointInRectangle(handleContainerRect, eventData.position, cam, out localPoint))
			{
				localPoint -= handleContainerRect.rect.position;
				float num2 = NormalizedValueX = Mathf.Clamp01((localPoint - m_Offset)[0] / handleContainerRect.rect.size[0]);
				float num4 = NormalizedValueY = Mathf.Clamp01((localPoint - m_Offset)[1] / handleContainerRect.rect.size[1]);
			}
		}

		private bool CanDrag(PointerEventData eventData)
		{
			return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}
			base.OnPointerDown(eventData);
			m_Offset = Vector2.zero;
			if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position, eventData.enterEventCamera))
			{
				Vector2 localPoint;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.position, eventData.pressEventCamera, out localPoint))
				{
					m_Offset = localPoint;
				}
				m_Offset.y = 0f - m_Offset.y;
			}
			else
			{
				UpdateDrag(eventData, eventData.pressEventCamera);
			}
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (CanDrag(eventData))
			{
				UpdateDrag(eventData, eventData.pressEventCamera);
			}
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		/*Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}*/

		bool ICanvasElement.IsDestroyed()
		{
			return IsDestroyed();
		}
	}
}
