using Disney.MobileNetwork;
using Disney.Native.iOS;
using System;
using UnityEngine;

public class HapticFeedbackAnimController : MonoBehaviour
{
	public void HapticFeedbackAnimEvent(string hapticFeedbackType)
	{
		if (!string.IsNullOrEmpty(hapticFeedbackType) && base.gameObject != null && base.gameObject.CompareTag("Player"))
		{
			Service.Get<iOSHapticFeedback>().TriggerHapticFeedback((iOSHapticFeedback.HapticFeedbackType)Enum.Parse(typeof(iOSHapticFeedback.HapticFeedbackType), hapticFeedbackType));
		}
	}
}
