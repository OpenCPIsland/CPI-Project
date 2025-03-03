using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public abstract class MaximumPreferredSize<T> : MonoBehaviour where T : class, ILayoutElement
	{
		public float MaxPreferredWidth;

		public float MaxPreferredHeight;

		private LayoutElement layoutElement;

		protected T _targetLayoutElement;

		protected virtual T targetLayoutElement
		{
			get
			{
				if (_targetLayoutElement == null)
				{
					_targetLayoutElement = base.gameObject.GetComponent<T>();
				}
				return _targetLayoutElement;
			}
		}

		private void Update()
		{
			if (this.targetLayoutElement == null)
			{
				return;
			}
			T targetLayoutElement;
			if (MaxPreferredWidth > 0f)
			{
				targetLayoutElement = this.targetLayoutElement;
				if (targetLayoutElement.preferredWidth > MaxPreferredWidth)
				{
					if (layoutElement == null)
					{
						layoutElement = base.gameObject.AddComponent<LayoutElement>();
					}
					layoutElement.preferredWidth = MaxPreferredWidth;
				}
				else if (layoutElement != null)
				{
					layoutElement.preferredWidth = -1f;
				}
			}
			if (!(MaxPreferredHeight > 0f))
			{
				return;
			}
			targetLayoutElement = this.targetLayoutElement;
			if (targetLayoutElement.preferredHeight > MaxPreferredHeight)
			{
				if (layoutElement == null)
				{
					layoutElement = base.gameObject.AddComponent<LayoutElement>();
				}
				layoutElement.preferredHeight = MaxPreferredHeight;
			}
			else if (layoutElement != null)
			{
				layoutElement.preferredHeight = -1f;
			}
		}
	}
}
