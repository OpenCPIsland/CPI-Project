using UnityEngine;

namespace Disney.Native.iOS
{
	public class iOSHapticFeedback : MonoBehaviour
	{
		public enum ImpactFeedbackStyle
		{
			Light,
			Medium,
			Heavy
		}

		public enum NotificationFeedbackType
		{
			Error,
			Success,
			Warning
		}

		public enum HapticFeedbackType
		{
			None,
			ImpactLight,
			ImpactMedium,
			ImpactHeavy,
			NotificationError,
			NotificationSuccess,
			NotificationWarning,
			Selection
		}

		private void Start()
		{
		}

		private void OnDestroy()
		{
		}

		public void TriggerImpactFeedback(ImpactFeedbackStyle style)
		{
		}

		public void TriggerNotificationFeedback(NotificationFeedbackType type)
		{
		}

		public void TriggerSelectionFeedback()
		{
		}

		public void TriggerHapticFeedback(HapticFeedbackType hapticFeedback)
		{
			switch (hapticFeedback)
			{
			case HapticFeedbackType.ImpactLight:
				TriggerImpactFeedback(ImpactFeedbackStyle.Light);
				break;
			case HapticFeedbackType.ImpactMedium:
				TriggerImpactFeedback(ImpactFeedbackStyle.Medium);
				break;
			case HapticFeedbackType.ImpactHeavy:
				TriggerImpactFeedback(ImpactFeedbackStyle.Heavy);
				break;
			case HapticFeedbackType.NotificationError:
				TriggerNotificationFeedback(NotificationFeedbackType.Error);
				break;
			case HapticFeedbackType.NotificationSuccess:
				TriggerNotificationFeedback(NotificationFeedbackType.Success);
				break;
			case HapticFeedbackType.NotificationWarning:
				TriggerNotificationFeedback(NotificationFeedbackType.Warning);
				break;
			case HapticFeedbackType.Selection:
				TriggerSelectionFeedback();
				break;
			}
		}
	}
}
