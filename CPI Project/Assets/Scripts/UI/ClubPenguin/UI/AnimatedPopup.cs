using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Animator))]
	public class AnimatedPopup : MonoBehaviour
	{
		public bool AutoOpen = true;

		public bool ShowBackground = false;

		public float OpenDelay = 0f;

		public bool CombinedOpenAnim = false;

		protected Animator animator;

		protected bool openDelayComplete = false;

		private bool hasStarted = false;

		private bool hasOpened = false;

		private bool isClosing = false;

		public event Action DoneOpen;

		public event Action DoneClose;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			awake();
		}

		protected virtual void awake()
		{
		}

		private void Start()
		{
			start();
			hasStarted = true;
			if (OpenDelay == 0f)
			{
				openDelayComplete = true;
				if (AutoOpen)
				{
					OpenPopup();
				}
			}
			else
			{
				CoroutineRunner.Start(waitForOpenDelay(), this, "AnimatedPopup.waitForOpenDelay");
			}
		}

		protected virtual void start()
		{
		}

		private void OnDestroy()
		{
			onDestroy();
			this.DoneOpen = null;
			this.DoneClose = null;
			CoroutineRunner.StopAllForOwner(this);
		}

		protected virtual void onDestroy()
		{
		}

		public virtual void OpenPopup()
		{
			if (hasStarted)
			{
				if (animator.runtimeAnimatorController != null)
				{
					animator.SetTrigger("Open");
				}
				if (ShowBackground)
				{
					base.gameObject.AddComponent<ModalBackground>();
				}
			}
			else
			{
				AutoOpen = true;
			}
		}

		public virtual void ClosePopup(bool closeImmediate = false)
		{
			if (isClosing)
			{
				return;
			}
			if (closeImmediate || animator.runtimeAnimatorController == null)
			{
				if (this.DoneClose != null)
				{
					this.DoneClose();
				}
				destroyPopup();
			}
			else
			{
				animator.SetTrigger("Close");
			}
			isClosing = true;
		}

		protected virtual void destroyPopup()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		protected virtual void popupOpenAnimationComplete()
		{
			hasOpened = true;
			if (this.DoneOpen != null)
			{
				this.DoneOpen();
			}
		}

		protected virtual void popupCloseAnimationComplete()
		{
			if (!CombinedOpenAnim || hasOpened)
			{
				if (this.DoneClose != null)
				{
					this.DoneClose();
				}
				destroyPopup();
			}
		}

		private IEnumerator waitForOpenDelay()
		{
			yield return new WaitForSeconds(OpenDelay);
			openDelayComplete = true;
			OpenPopup();
		}
	}
}
