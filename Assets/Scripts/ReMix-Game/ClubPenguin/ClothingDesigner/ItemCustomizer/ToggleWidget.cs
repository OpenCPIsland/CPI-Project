using Disney.Kelowna.Common;
using Fabric;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class ToggleWidget : Toggle
	{
		private const string HIDDEN_BOOL_ANIM = "IsHidden";

		private const string SHOW_ANIM = "Show";

		private const string HIDE_ANIM = "Hide";

		private GameObject _disabledCover = null;

		private bool _hasInput = false;

		private Animator toggleAnimator;

		private EventChannel eventChannel;

		protected override void Awake()
		{
			base.Awake();
			Transform transform = base.transform.Find("_disabledCover");
			if (transform != null)
			{
				_disabledCover = transform.gameObject;
			}
			setupListeners();
			toggleAnimator = GetComponent<Animator>();
			toggleAnimator.SetBool("IsHidden", true);
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerWidgetEvents.ShowTileWidget>(onShowTileWidget);
			eventChannel.AddListener<CustomizerWidgetEvents.HideTileWidget>(onHideTileWidget);
			eventChannel.AddListener<CustomizerWidgetEvents.SetTileValue>(onSetTileValue);
			eventChannel.AddListener<CustomizerWidgetEvents.SetIsTileInteractable>(onSetInteractable);
			onValueChanged.AddListener(dispatchValueChanged);
		}

		private bool onShowTileWidget(CustomizerWidgetEvents.ShowTileWidget evt)
		{
			if (toggleAnimator.GetBool("IsHidden"))
			{
				toggleAnimator.SetBool("IsHidden", false);
				toggleAnimator.ResetTrigger("Hide");
				toggleAnimator.SetTrigger("Show");
			}
			return false;
		}

		private bool onHideTileWidget(CustomizerWidgetEvents.HideTileWidget evt)
		{
			if (!toggleAnimator.GetBool("IsHidden"))
			{
				toggleAnimator.SetBool("IsHidden", true);
				toggleAnimator.ResetTrigger("Show");
				toggleAnimator.SetTrigger("Hide");
			}
			return false;
		}

		private bool onSetTileValue(CustomizerWidgetEvents.SetTileValue evt)
		{
			base.isOn = evt.Value;
			return false;
		}

		private bool onSetInteractable(CustomizerWidgetEvents.SetIsTileInteractable evt)
		{
			bool value = evt.Value;
			bool flag = true;
			if (!value && _hasInput)
			{
				flag = false;
			}
			if (flag)
			{
				base.interactable = value;
				if (_disabledCover != null)
				{
					_disabledCover.SetActive(!base.interactable);
				}
			}
			return false;
		}

		private void dispatchValueChanged(bool updatedValue)
		{
			CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.TileValueChanged(updatedValue));
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (IsInteractable())
			{
				base.OnPointerDown(eventData);
				_hasInput = true;
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			if (IsInteractable())
			{
				base.OnPointerUp(eventData);
				_hasInput = false;
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/Toggle", EventAction.PlaySound);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			onValueChanged.RemoveListener(dispatchValueChanged);
		}
	}
}
