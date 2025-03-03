using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(LayoutElement))]
	public class LayoutElementWidthUpdater : MonoBehaviour
	{
		public RectTransform UpdateTarget;

		private LayoutElement layoutElement;

		private void Awake()
		{
			layoutElement = GetComponent<LayoutElement>();
		}

		private void Update()
		{
			if (Math.Abs(UpdateTarget.sizeDelta.x - layoutElement.preferredWidth) > float.Epsilon)
			{
				layoutElement.preferredWidth = UpdateTarget.sizeDelta.x;
				layoutElement.minWidth = UpdateTarget.sizeDelta.x;
			}
		}
	}
}
