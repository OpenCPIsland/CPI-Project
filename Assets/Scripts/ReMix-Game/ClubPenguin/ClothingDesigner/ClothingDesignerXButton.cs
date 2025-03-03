using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner
{
	[RequireComponent(typeof(Animator))]
	public class ClothingDesignerXButton : MonoBehaviour
	{
		private const string INTRO_TRIGGER = "Intro";

		private const string OUTRO_TRIGGER = "Outro";

		private Animator animator;

		private EventChannel eventChannel;

		private bool isActive;

		private int introAnimatorId;

		private int outroAnimatorId;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			eventChannel = new EventChannel(ClothingDesignerContext.EventBus);
			eventChannel.AddListener<ClothingDesignerUIEvents.ShowCloseButton>(onShowCloseButton);
			eventChannel.AddListener<ClothingDesignerUIEvents.HideCloseButton>(onHideCloseButton);
			isActive = true;
			introAnimatorId = Animator.StringToHash("Intro");
			outroAnimatorId = Animator.StringToHash("Outro");
		}

		public void OnCloseButton()
		{
			if (isActive)
			{
				ClothingDesignerContext.EventBus.DispatchEvent(default(ClothingDesignerUIEvents.CloseButton));
			}
		}

		private bool onShowCloseButton(ClothingDesignerUIEvents.ShowCloseButton evt)
		{
			if (!isActive)
			{
				animator.SetTrigger(introAnimatorId);
				isActive = true;
			}
			return false;
		}

		private bool onHideCloseButton(ClothingDesignerUIEvents.HideCloseButton evt)
		{
			if (isActive)
			{
				animator.SetTrigger(outroAnimatorId);
				isActive = false;
			}
			return false;
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}
	}
}
