using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("Layout/Extensions/Tile Size Fitter")]
	public class TileSizeFitter : UIBehaviour, ILayoutSelfController, ILayoutController
	{
		[SerializeField]
		private Vector2 m_Border = Vector2.zero;

		[SerializeField]
		private Vector2 m_TileSize = Vector2.zero;

		[NonSerialized]
		private RectTransform m_Rect;

		private DrivenRectTransformTracker m_Tracker;

		public Vector2 Border
		{
			get
			{
				return m_Border;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_Border, value))
				{
					SetDirty();
				}
			}
		}

		public Vector2 TileSize
		{
			get
			{
				return m_TileSize;
			}
			set
			{
				if (SetPropertyUtility.SetStruct(ref m_TileSize, value))
				{
					SetDirty();
				}
			}
		}

		private RectTransform rectTransform
		{
			get
			{
				if (m_Rect == null)
				{
					m_Rect = GetComponent<RectTransform>();
				}
				return m_Rect;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			base.OnDisable();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			UpdateRect();
		}

		private void UpdateRect()
		{
			if (IsActive())
			{
				m_Tracker.Clear();
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY);
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
				rectTransform.anchoredPosition = Vector2.zero;
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDelta);
				Vector2 a = GetParentSize() - Border;
				Vector2 tileSize = TileSize;
				if (tileSize.x > 0.001f)
				{
					float x = a.x;
					float x2 = a.x;
					Vector2 tileSize2 = TileSize;
					float num = Mathf.Floor(x2 / tileSize2.x);
					Vector2 tileSize3 = TileSize;
					a.x = x - num * tileSize3.x;
				}
				else
				{
					a.x = 0f;
				}
				Vector2 tileSize4 = TileSize;
				if (tileSize4.y > 0.001f)
				{
					float y = a.y;
					float y2 = a.y;
					Vector2 tileSize5 = TileSize;
					float num2 = Mathf.Floor(y2 / tileSize5.y);
					Vector2 tileSize6 = TileSize;
					a.y = y - num2 * tileSize6.y;
				}
				else
				{
					a.y = 0f;
				}
				rectTransform.sizeDelta = -a;
			}
		}

		private Vector2 GetParentSize()
		{
			RectTransform rectTransform = this.rectTransform.parent as RectTransform;
			if (!rectTransform)
			{
				return Vector2.zero;
			}
			return rectTransform.rect.size;
		}

		public virtual void SetLayoutHorizontal()
		{
		}

		public virtual void SetLayoutVertical()
		{
		}

		protected void SetDirty()
		{
			if (IsActive())
			{
				UpdateRect();
			}
		}
	}
}
