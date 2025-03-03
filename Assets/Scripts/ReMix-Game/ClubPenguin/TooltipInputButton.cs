using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Button))]
	public class TooltipInputButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private const string ANIMATOR_BOOL_ISOPEN = "IsOpen";

		public Animator TooltipAnimator;

		public bool TooltipEnabled;

		public bool IsOpen;

		public void OnPointerDown(PointerEventData data)
		{
			if (TooltipEnabled)
			{
				IsOpen = !IsOpen;
				if (TooltipAnimator != null)
				{
					TooltipAnimator.SetBool("IsOpen", IsOpen);
				}
			}
		}

		public void CloseTooltip()
		{
			if (TooltipEnabled && IsOpen && TooltipAnimator != null)
			{
				IsOpen = false;
				TooltipAnimator.SetBool("IsOpen", IsOpen);
			}
		}
	}
}
