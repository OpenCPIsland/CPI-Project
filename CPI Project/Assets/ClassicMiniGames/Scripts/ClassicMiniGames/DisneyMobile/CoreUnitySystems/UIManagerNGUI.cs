using DisneyMobile.CoreUnitySystems.Utility;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class UIManagerNGUI : UIManager
	{
		public bool disableListeners = true;

		protected void DisableAllListeners(GameObject gobj)
		{
			if (disableListeners)
			{
				AudioListener[] componentsInChildren = gobj.GetComponentsInChildren<AudioListener>();
				AudioListener[] array = componentsInChildren;
				foreach (AudioListener audioListener in array)
				{
					audioListener.enabled = false;
				}
			}
		}

		public override void ControlLoaded(UIControlBase cbase)
		{
			int layer = cbase.transform.parent.gameObject.layer;
			DisableAllListeners(cbase.gameObject);
			Utilities.SetLayers(cbase.transform.parent.gameObject, layer);
			GetUICamera().cullingMask = 1 << layer;
			SetEventReceiverMask(1 << layer);
		}

		public override Camera GetUICamera()
		{
			return UICamera.mainCamera;
		}

		public override void AdjustScreenDeapth(GameObject uiObj, int depth)
		{
			NGUITools.AdjustDepth(uiObj, depth);
		}

		public override void SetEventReceiverMask(int mask)
		{
			UICamera.eventHandler.eventReceiverMask = mask;
		}

		protected override void DarkMaskTweenStart(float targetAlpha, bool isFadeIn)
		{
			TweenAlpha tweenAlpha = TweenAlpha.Begin(DarkBackground.transform.GetChild(0).gameObject, DarkBackgroundFadeDuration, DarkBackgroundAlpha);
			if (isFadeIn)
			{
				tweenAlpha.SetOnFinished(base.DarkMaskFadeInFinished);
			}
			else
			{
				tweenAlpha.SetOnFinished(base.DarkMaskFadeOutFinished);
			}
		}

		protected override void DarkMaskSetDepth(int depth)
		{
			if (DarkBackground != null)
			{
				UIPanel component = DarkBackground.GetComponent<UIPanel>();
				if (component != null)
				{
					component.depth = depth;
				}
				else
				{
					Logger.LogFatal(this, "darkBackground must have UIPanel component");
				}
			}
		}

		public override void SetUIEventMessageObject(GameObject mobj)
		{
			UIButtonMessage[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIButtonMessage>();
			UIButtonMessage[] array = componentsInChildren;
			foreach (UIButtonMessage uIButtonMessage in array)
			{
				if (uIButtonMessage.target == null)
				{
					uIButtonMessage.target = mobj;
				}
			}
		}

		protected override GameObject GetDefaultScreenRoot()
		{
			return GameObject.Find("Anchor");
		}
	}
}
