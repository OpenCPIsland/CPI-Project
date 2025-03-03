using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(TrayInputButton))]
	public class TrayInputButtonHider : MonoBehaviour
	{
		private static int INTRO_TRIGGER = Animator.StringToHash("Intro");

		private static int EXIT_TRIGGER = Animator.StringToHash("Exit");

		[SerializeField]
		private Animator containerAnimator = null;

		private TrayInputButton inputButton;

		private void OnValidate()
		{
		}

		private void Awake()
		{
			inputButton = GetComponent<TrayInputButton>();
		}

		private void OnEnable()
		{
			inputButton.OnStateChanged += onStateChanged;
		}

		private void OnDisable()
		{
			inputButton.OnStateChanged -= onStateChanged;
		}

		private void onStateChanged(TrayInputButton.ButtonState buttonState)
		{
			if (buttonState != 0)
			{
				containerAnimator.ResetTrigger(EXIT_TRIGGER);
				containerAnimator.SetTrigger(INTRO_TRIGGER);
			}
			else
			{
				containerAnimator.ResetTrigger(INTRO_TRIGGER);
				containerAnimator.SetTrigger(EXIT_TRIGGER);
			}
		}
	}
}
