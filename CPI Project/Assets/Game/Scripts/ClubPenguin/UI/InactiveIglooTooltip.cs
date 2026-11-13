using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class InactiveIglooTooltip : MonoBehaviour
	{
		private const string ANIM_ISOPEN = "IsOpen";

		[SerializeField]
		private Animator anim;

		[SerializeField]
		private float displayTime = 3f;

		private bool isShown = false;

		public void Show()
		{
			base.gameObject.SetActive(true);
			CoroutineRunner.Start(showToolTipTimed(), this, "showTooltipTimed_inactiveIgloo");
		}

		public IEnumerator showToolTipTimed()
		{
			anim.SetBool("IsOpen", true);
			yield return new WaitForSeconds(displayTime);
			Hide();
		}

		public void Hide()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (base.gameObject.activeSelf)
			{
				anim.SetBool("IsOpen", false);
			}
		}

		public void OnTooltipOpenAnimationComplete()
		{
			isShown = true;
		}

		public void OnTooltipCloseAnimationComplete()
		{
			if (isShown)
			{
				isShown = false;
				base.gameObject.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
