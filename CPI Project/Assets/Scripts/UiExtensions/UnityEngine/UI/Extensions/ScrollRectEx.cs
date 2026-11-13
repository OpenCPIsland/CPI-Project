using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/ScrollRectEx")]
	public class ScrollRectEx : ScrollRect
	{
		private bool routeToParent = false;

		private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
		{
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				Component[] components = parent.GetComponents<Component>();
				foreach (Component component in components)
				{
					if (component is T)
					{
						action((T)(IEventSystemHandler)component);
					}
				}
				parent = parent.parent;
			}
		}

		public override void OnInitializePotentialDrag(PointerEventData eventData)
		{
			DoForParents(delegate(IInitializePotentialDragHandler parent)
			{
				parent.OnInitializePotentialDrag(eventData);
			});
			base.OnInitializePotentialDrag(eventData);
		}

		public override void OnDrag(PointerEventData eventData)
		{
			if (routeToParent)
			{
				DoForParents(delegate(IDragHandler parent)
				{
					parent.OnDrag(eventData);
				});
			}
			else
			{
				base.OnDrag(eventData);
			}
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			if (!base.horizontal)
			{
				Vector2 delta = eventData.delta;
				float num = Math.Abs(delta.x);
				Vector2 delta2 = eventData.delta;
				if (num > Math.Abs(delta2.y))
				{
					routeToParent = true;
					goto IL_00ae;
				}
			}
			if (!base.vertical)
			{
				Vector2 delta3 = eventData.delta;
				float num2 = Math.Abs(delta3.x);
				Vector2 delta4 = eventData.delta;
				if (num2 < Math.Abs(delta4.y))
				{
					routeToParent = true;
					goto IL_00ae;
				}
			}
			routeToParent = false;
			goto IL_00ae;
			IL_00ae:
			if (routeToParent)
			{
				DoForParents(delegate(IBeginDragHandler parent)
				{
					parent.OnBeginDrag(eventData);
				});
			}
			else
			{
				base.OnBeginDrag(eventData);
			}
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			if (routeToParent)
			{
				DoForParents(delegate(IEndDragHandler parent)
				{
					parent.OnEndDrag(eventData);
				});
			}
			else
			{
				base.OnEndDrag(eventData);
			}
			routeToParent = false;
		}

		public override void OnScroll(PointerEventData eventData)
		{
			if (!base.horizontal)
			{
				Vector2 scrollDelta = eventData.scrollDelta;
				float num = Math.Abs(scrollDelta.x);
				Vector2 scrollDelta2 = eventData.scrollDelta;
				if (num > Math.Abs(scrollDelta2.y))
				{
					routeToParent = true;
					goto IL_00b4;
				}
			}
			if (!base.vertical)
			{
				Vector2 scrollDelta3 = eventData.scrollDelta;
				float num2 = Math.Abs(scrollDelta3.x);
				Vector2 scrollDelta4 = eventData.scrollDelta;
				if (num2 < Math.Abs(scrollDelta4.y))
				{
					routeToParent = true;
					goto IL_00b4;
				}
			}
			routeToParent = false;
			goto IL_00b4;
			IL_00b4:
			if (routeToParent)
			{
				DoForParents(delegate(IScrollHandler parent)
				{
					parent.OnScroll(eventData);
				});
			}
			else
			{
				base.OnScroll(eventData);
			}
		}
	}
}
