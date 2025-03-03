using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class UIManagerUGUI : UIManager
	{
		protected override void DarkMaskSetDepth(int depth)
		{
		}

		protected override void DarkMaskTweenStart(float targetAlpha, bool isFadeIn)
		{
		}

		public override void SetEventReceiverMask(int mask)
		{
		}

		public override void AdjustScreenDeapth(GameObject uiObj, int depth)
		{
		}

		public override void SetUIEventMessageObject(GameObject mobj)
		{
		}

		protected override GameObject GetDefaultScreenRoot()
		{
			return base.gameObject;
		}
	}
}
