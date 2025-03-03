using Disney.Kelowna.Common;
using Fabric;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner
{
	[RequireComponent(typeof(Animator))]
	public class DragContainer : MonoBehaviour
	{
		private const string SHOW_ANIM = "Show";

		private const string HIDE_ANIM = "Hide";

		[SerializeField]
		private Image itemImage;

		private Animator animator;

		private bool isShowing;

		public Vector2 ImageOffset;

		public bool IsShowing
		{
			get
			{
				return isShowing;
			}
		}

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Start()
		{
			itemImage.transform.localPosition = ImageOffset;
		}

		public void SetImage(Sprite sprite)
		{
			itemImage.sprite = sprite;
		}

		public Sprite GetSprite()
		{
			return itemImage.sprite;
		}

		public void Show()
		{
			if (!isShowing)
			{
				isShowing = true;
				base.gameObject.SetActive(true);
				CoroutineRunner.StopAllForOwner(this);
				animator.SetTrigger("Show");
			}
		}

		public void Hide(bool instant = false)
		{
			if (isShowing || instant)
			{
				isShowing = false;
				if (instant)
				{
					base.gameObject.SetActive(false);
				}
				else
				{
					CoroutineRunner.Start(delayedHide(), this, "delayedHide");
				}
			}
		}

		private IEnumerator delayedHide()
		{
			if (animator.isInitialized)
			{
				animator.SetTrigger("Hide");
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ItemDropFail", EventAction.PlaySound);
				yield return new WaitForEndOfFrame();
				float seconds = animator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(seconds);
				if (!isShowing)
				{
					base.gameObject.SetActive(false);
				}
			}
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
