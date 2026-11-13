using UnityEngine;
using UnityEngine.UI;

namespace Disney.Kelowna.Common
{
	[DisallowMultipleComponent]
	public class FormFieldFocusItem : MonoBehaviour
	{
		private LayoutGroup layoutGroup;

		public UISelectionBroadcaster SelectableItem
		{
			get;
			private set;
		}

		public UILayoutSystemBroadcaster LayoutBroadcaster
		{
			get;
			private set;
		}

		public float Height
		{
			get
			{
				return (layoutGroup != null) ? layoutGroup.preferredHeight : 0f;
			}
		}

		public float Width
		{
			get
			{
				return (layoutGroup != null) ? layoutGroup.preferredWidth : 0f;
			}
		}

		private void Awake()
		{
			SelectableItem = GetComponentInChildren<UISelectionBroadcaster>();
			LayoutBroadcaster = GetComponentInChildren<UILayoutSystemBroadcaster>();
			layoutGroup = GetComponent<LayoutGroup>();
		}
	}
}
