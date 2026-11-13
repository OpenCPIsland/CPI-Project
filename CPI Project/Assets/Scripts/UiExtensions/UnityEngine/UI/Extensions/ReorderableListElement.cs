using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(RectTransform))]
	public class ReorderableListElement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
	{
		private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

		private ReorderableList _currentReorderableListRaycasted;

		private RectTransform _draggingObject;

		private LayoutElement _draggingObjectLE;

		private Vector2 _draggingObjectOriginalSize;

		private RectTransform _fakeElement;

		private LayoutElement _fakeElementLE;

		private int _fromIndex;

		private bool _isDragging;

		private RectTransform _rect;

		private ReorderableList _reorderableList;

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (_reorderableList == null)
			{
				return;
			}
			if (!_reorderableList.IsDraggable)
			{
				_draggingObject = null;
				return;
			}
			if (!_reorderableList.CloneDraggedObject)
			{
				_draggingObject = _rect;
				_fromIndex = _rect.GetSiblingIndex();
				if (_reorderableList.OnElementRemoved != null)
				{
					_reorderableList.OnElementRemoved.Invoke(new ReorderableList.ReorderableListEventStruct
					{
						DroppedObject = _draggingObject.gameObject,
						IsAClone = _reorderableList.CloneDraggedObject,
						SourceObject = ((!_reorderableList.CloneDraggedObject) ? _draggingObject.gameObject : base.gameObject),
						FromList = _reorderableList,
						FromIndex = _fromIndex
					});
				}
			}
			else
			{
				GameObject gameObject = Object.Instantiate(base.gameObject);
				_draggingObject = gameObject.GetComponent<RectTransform>();
			}
			_draggingObjectOriginalSize = base.gameObject.GetComponent<RectTransform>().rect.size;
			_draggingObjectLE = _draggingObject.GetComponent<LayoutElement>();
			_draggingObject.SetParent(_reorderableList.DraggableArea, false);
			_draggingObject.SetAsLastSibling();
			_fakeElement = new GameObject("Fake").AddComponent<RectTransform>();
			_fakeElementLE = _fakeElement.gameObject.AddComponent<LayoutElement>();
			RefreshSizes();
			if (_reorderableList.OnElementGrabbed != null)
			{
				_reorderableList.OnElementGrabbed.Invoke(new ReorderableList.ReorderableListEventStruct
				{
					DroppedObject = _draggingObject.gameObject,
					IsAClone = _reorderableList.CloneDraggedObject,
					SourceObject = ((!_reorderableList.CloneDraggedObject) ? _draggingObject.gameObject : base.gameObject),
					FromList = _reorderableList,
					FromIndex = _fromIndex
				});
			}
			_isDragging = true;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!_isDragging)
			{
				return;
			}
			_draggingObject.position = eventData.position;
			EventSystem.current.RaycastAll(eventData, _raycastResults);
			for (int i = 0; i < _raycastResults.Count; i++)
			{
				_currentReorderableListRaycasted = _raycastResults[i].gameObject.GetComponent<ReorderableList>();
				if (_currentReorderableListRaycasted != null)
				{
					break;
				}
			}
			if (_currentReorderableListRaycasted == null || !_currentReorderableListRaycasted.IsDropable)
			{
				RefreshSizes();
				_fakeElement.transform.SetParent(_reorderableList.DraggableArea, false);
				return;
			}
			if (_fakeElement.parent != _currentReorderableListRaycasted)
			{
				_fakeElement.SetParent(_currentReorderableListRaycasted.Content, false);
			}
			float num = float.PositiveInfinity;
			int siblingIndex = 0;
			float num2 = 0f;
			for (int j = 0; j < _currentReorderableListRaycasted.Content.childCount; j++)
			{
				RectTransform component = _currentReorderableListRaycasted.Content.GetChild(j).GetComponent<RectTransform>();
				if (_currentReorderableListRaycasted.ContentLayout is VerticalLayoutGroup)
				{
					Vector3 position = component.position;
					float y = position.y;
					Vector2 position2 = eventData.position;
					num2 = Mathf.Abs(y - position2.y);
				}
				else if (_currentReorderableListRaycasted.ContentLayout is HorizontalLayoutGroup)
				{
					Vector3 position3 = component.position;
					float x = position3.x;
					Vector2 position4 = eventData.position;
					num2 = Mathf.Abs(x - position4.x);
				}
				else if (_currentReorderableListRaycasted.ContentLayout is GridLayoutGroup)
				{
					Vector3 position5 = component.position;
					float x2 = position5.x;
					Vector2 position6 = eventData.position;
					float num3 = Mathf.Abs(x2 - position6.x);
					Vector3 position7 = component.position;
					float y2 = position7.y;
					Vector2 position8 = eventData.position;
					num2 = num3 + Mathf.Abs(y2 - position8.y);
				}
				if (num2 < num)
				{
					num = num2;
					siblingIndex = j;
				}
			}
			RefreshSizes();
			_fakeElement.SetSiblingIndex(siblingIndex);
			_fakeElement.gameObject.SetActive(true);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			_isDragging = false;
			if (_draggingObject != null)
			{
				if (_currentReorderableListRaycasted != null && _currentReorderableListRaycasted.IsDropable)
				{
					RefreshSizes();
					_draggingObject.SetParent(_currentReorderableListRaycasted.Content, false);
					_draggingObject.SetSiblingIndex(_fakeElement.GetSiblingIndex());
					if (_reorderableList.OnElementDropped != null)
					{
						_reorderableList.OnElementDropped.Invoke(new ReorderableList.ReorderableListEventStruct
						{
							DroppedObject = _draggingObject.gameObject,
							IsAClone = _reorderableList.CloneDraggedObject,
							SourceObject = ((!_reorderableList.CloneDraggedObject) ? _draggingObject.gameObject : base.gameObject),
							FromList = _reorderableList,
							FromIndex = _fromIndex,
							ToList = _currentReorderableListRaycasted,
							ToIndex = _fakeElement.GetSiblingIndex() - 1
						});
					}
				}
				else if (_reorderableList.CloneDraggedObject)
				{
					Object.Destroy(_draggingObject.gameObject);
				}
				else
				{
					RefreshSizes();
					_draggingObject.SetParent(_reorderableList.Content, false);
					_draggingObject.SetSiblingIndex(_fromIndex);
				}
			}
			if (_fakeElement != null)
			{
				Object.Destroy(_fakeElement.gameObject);
			}
		}

		private void RefreshSizes()
		{
			Vector2 sizeDelta = _draggingObjectOriginalSize;
			if (_currentReorderableListRaycasted != null && _currentReorderableListRaycasted.IsDropable && _currentReorderableListRaycasted.Content.childCount > 0)
			{
				Transform child = _currentReorderableListRaycasted.Content.GetChild(0);
				if (child != null)
				{
					sizeDelta = child.GetComponent<RectTransform>().rect.size;
				}
			}
			_draggingObject.sizeDelta = sizeDelta;
			LayoutElement fakeElementLE = _fakeElementLE;
			float y = sizeDelta.y;
			_draggingObjectLE.preferredHeight = y;
			fakeElementLE.preferredHeight = y;
			LayoutElement fakeElementLE2 = _fakeElementLE;
			y = sizeDelta.x;
			_draggingObjectLE.preferredWidth = y;
			fakeElementLE2.preferredWidth = y;
		}

		public void Init(ReorderableList reorderableList)
		{
			_reorderableList = reorderableList;
			_rect = GetComponent<RectTransform>();
		}
	}
}
