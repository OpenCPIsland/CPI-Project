using Disney.Kelowna.Common;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class BuyTemplatePopup : MonoBehaviour
	{
		private const string INTRO_ANIM_TRIGGER = "Intro";

		private const string OUTRO_ANIM_TRIGGER = "Outro";

		[SerializeField]
		private RectTransform popup;

		[SerializeField]
		private Animator popupAnimator;

		[SerializeField]
		private Button buyButton;

		[SerializeField]
		private Button dismissButton;

		[SerializeField]
		private Button infoButton;

		public event Action BuyButtonPressed;

		public event Action PopupDismissed;

		public event Action InfoButtonPressed;

		public void SetPosition(Vector3 newPosition)
		{
			popup.position = newPosition;
			float width = (base.transform as RectTransform).rect.width;
			float num = width / 2f;
			float num2 = popup.localPosition.x - popup.rect.width / 2f;
			float num3 = popup.localPosition.x + popup.rect.width / 2f;
			Vector3 localPosition;
			if (num2 < 0f - num)
			{
				float num4 = num2 + num;
				float x = popup.localPosition.x - num4;
				localPosition = popup.localPosition;
				localPosition.x = x;
				popup.localPosition = localPosition;
			}
			else if (num3 > num)
			{
				float num4 = num3 - num;
				float x = popup.localPosition.x - num4;
				localPosition = popup.localPosition;
				localPosition.x = x;
				popup.localPosition = localPosition;
			}
			float height = (base.transform as RectTransform).rect.height;
			float num5 = height / 2f;
			float num6 = popup.localPosition.y + popup.rect.height / 2f;
			float num7 = popup.localPosition.y - popup.rect.height / 2f;
			if (num7 < 0f - num5)
			{
				float num4 = num7 + num5;
				float y = popup.localPosition.y - num4;
				localPosition = popup.localPosition;
				localPosition.y = y;
				popup.localPosition = localPosition;
			}
			else if (num6 > num5)
			{
				float num4 = num6 - num5;
				float y = popup.localPosition.y - num4;
				localPosition = popup.localPosition;
				localPosition.y = y;
				popup.localPosition = localPosition;
			}
			popup.localPosition = new Vector3(popup.localPosition.x, popup.localPosition.y, 0f);
		}

		private void OnEnable()
		{
			buyButton.onClick.AddListener(onBuyButton);
			dismissButton.onClick.AddListener(onDismissButton);
			infoButton.onClick.AddListener(onInfoButton);
			popupAnimator.SetTrigger("Intro");
		}

		private void OnDisable()
		{
			removeListeners();
		}

		private void removeListeners()
		{
			buyButton.onClick.RemoveListener(onBuyButton);
			dismissButton.onClick.RemoveListener(onDismissButton);
			infoButton.onClick.RemoveListener(onInfoButton);
		}

		private void onBuyButton()
		{
			removeListeners();
			CoroutineRunner.Start(delayedHideAnimation(false), this, "delayedHideAnimation");
		}

		private void onDismissButton()
		{
			removeListeners();
			CoroutineRunner.Start(delayedHideAnimation(true), this, "delayedHideAnimation");
		}

		private void onInfoButton()
		{
			if (this.InfoButtonPressed != null)
			{
				this.InfoButtonPressed();
			}
		}

		private IEnumerator delayedHideAnimation(bool isDismissed)
		{
			popupAnimator.SetTrigger("Outro");
			yield return new WaitForSeconds(0.25f);
			if (isDismissed)
			{
				if (this.PopupDismissed != null)
				{
					this.PopupDismissed();
				}
			}
			else if (this.BuyButtonPressed != null)
			{
				this.BuyButtonPressed();
			}
		}

		private void OnDestroy()
		{
			removeListeners();
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
