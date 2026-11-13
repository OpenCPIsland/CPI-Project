using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(SpriteSelector))]
	public class TrayInputButton : MonoBehaviour
	{
		public enum ButtonState
		{
			None = -1,
			Disabled,
			Default,
			Pulsing,
			Highlighted
		}

		private static readonly int INTRO_TRIGGER = Animator.StringToHash("Intro");

		private static readonly int EXIT_TRIGGER = Animator.StringToHash("Exit");

		public SpriteSelector BackgroundSprite;

		public SpriteSelector IconSprite;

		public TintSelector IconTint;

		public Animator BackgroundAnimator;

		public Image Icon;

		public GameObject Behaviour;

		public TrayInputButtonAnimationEvent AnimationEvent;

		private bool isLocked;

		private ButtonState pendingViewState;

		private Animator containerAnimator;

		public int Index
		{
			get;
			set;
		}

		public bool IsBackgroundVisible
		{
			get;
			set;
		}

		public ButtonState DefaultState
		{
			get;
			set;
		}

		public bool IsLocked
		{
			get
			{
				return isLocked;
			}
		}

		public ButtonState CurrentState
		{
			get;
			private set;
		}

		public ButtonState CurrentViewState
		{
			get;
			private set;
		}

		public event Action<int> OnReady;

		public event Action<ButtonState> OnStateChanged;

		private void Awake()
		{
			if (AnimationEvent != null)
			{
				AnimationEvent.OnTransitionReady += onTransitionReady;
			}
		}

		public void Lock(ButtonState buttonState)
		{
			isLocked = true;
			if (buttonState != ButtonState.None)
			{
				setVisualState(buttonState);
			}
		}

		public void Unlock()
		{
			isLocked = false;
			setVisualState(CurrentState);
		}

		public void SetState(ButtonState buttonState)
		{
			if (buttonState != CurrentState)
			{
				if (this.OnStateChanged != null)
				{
					this.OnStateChanged(buttonState);
				}
				CurrentState = buttonState;
				if (!isLocked)
				{
					setVisualState(buttonState);
				}
			}
		}

		private void setVisualState(ButtonState buttonState)
		{
			pendingViewState = buttonState;
			if (BackgroundAnimator != null)
			{
				setAnimatorState(buttonState);
				if (!requiresTransition(buttonState))
				{
					onTransitionReady();
				}
			}
			GetComponent<Button>().interactable = (buttonState != ButtonState.Disabled);
		}

		private void onTransitionReady()
		{
			if (pendingViewState != ButtonState.None)
			{
				int sprites = (int)pendingViewState;
				CurrentViewState = pendingViewState;
				pendingViewState = ButtonState.None;
				setSprites(sprites);
			}
		}

		private void setSprites(int buttonStateInt)
		{
			if (IsBackgroundVisible && BackgroundSprite != null)
			{
				BackgroundSprite.SelectSprite(buttonStateInt);
			}
			if (IconSprite != null && IconSprite.Sprites != null && IconSprite.Sprites.Length > buttonStateInt)
			{
				IconSprite.SelectSprite(buttonStateInt);
			}
			if (IconTint != null && IconTint.Colors != null && IconTint.Colors.Length > buttonStateInt)
			{
				IconTint.SelectColor(buttonStateInt);
			}
		}

		private void setAnimatorState(ButtonState buttonState)
		{
			if (BackgroundAnimator.isInitialized)
			{
				foreach (ButtonState value in Enum.GetValues(typeof(ButtonState)))
				{
					if (value != ButtonState.None)
					{
						BackgroundAnimator.SetBool(value.ToString(), false);
					}
				}
				BackgroundAnimator.SetBool(buttonState.ToString(), true);
			}
		}

		private bool requiresTransition(ButtonState buttonState)
		{
			if (buttonState != 0 && CurrentViewState != 0)
			{
				return false;
			}
			return true;
		}

		public void ResetButton(bool playIntro = true)
		{
			if (containerAnimator == null)
			{
				containerAnimator = base.transform.parent.parent.GetComponent<Animator>();
			}
			IconSprite.Sprites = null;
			IconTint.Colors = null;
			if (containerAnimator != null)
			{
				containerAnimator.ResetTrigger(INTRO_TRIGGER);
				containerAnimator.SetTrigger(EXIT_TRIGGER);
				float seconds = 0f;
				for (int i = 0; i < containerAnimator.runtimeAnimatorController.animationClips.Length; i++)
				{
					AnimationClip animationClip = containerAnimator.runtimeAnimatorController.animationClips[i];
					if (animationClip.name == "Exit")
					{
						seconds = animationClip.length;
						break;
					}
				}
				CoroutineRunner.Start(waitForExitAnimation(seconds, playIntro), this, "waitForExitAnimation");
			}
			else
			{
				destroyChildren();
				Log.LogError(this, "Could not find container animator in parent");
			}
		}

		public void InitializeView(ButtonState buttonState, ButtonState buttonVisualState = ButtonState.None)
		{
			if (buttonVisualState == ButtonState.None)
			{
				buttonVisualState = buttonState;
			}
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(buttonVisualState);
			}
			CurrentState = buttonState;
			CurrentViewState = buttonVisualState;
			int sprites = (int)buttonVisualState;
			pendingViewState = ButtonState.None;
			setSprites(sprites);
			GetComponent<Button>().interactable = (buttonState != ButtonState.Disabled);
			setAnimatorState(buttonVisualState);
			BackgroundAnimator.Play(buttonVisualState.ToString());
		}

		private void destroyChildren()
		{
			for (int num = Behaviour.transform.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(Behaviour.transform.GetChild(num).gameObject);
			}
		}

		private IEnumerator waitForExitAnimation(float seconds, bool playIntro)
		{
			yield return new WaitForSeconds(seconds);
			destroyChildren();
			if (containerAnimator != null && playIntro)
			{
				containerAnimator.ResetTrigger(EXIT_TRIGGER);
				containerAnimator.SetTrigger(INTRO_TRIGGER);
			}
			if (this.OnReady != null)
			{
				this.OnReady(Index);
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
			if (AnimationEvent != null)
			{
				AnimationEvent.OnTransitionReady -= onTransitionReady;
			}
			AnimationEvent = null;
			this.OnReady = null;
			this.OnStateChanged = null;
		}
	}
}
