using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class ScaleAndRotateWidget : MonoBehaviour
	{
		private const string HIDDEN_BOOL_ANIM = "IsHidden";

		private const string SHOW_ANIM = "Show";

		private const string HIDE_ANIM = "Hide";

		private EventChannel eventChannel;

		private Animator scaleAndRotateAnimator;

		private SliderWidget sliderWidget;

		private SliderWidget SliderWidget
		{
			get
			{
				if (sliderWidget == null)
				{
					sliderWidget = GetComponentInChildren<SliderWidget>();
					if (sliderWidget != null)
					{
						sliderWidget.onValueChanged.AddListener(onScaleChanged);
					}
				}
				return sliderWidget;
			}
		}

		private void Start()
		{
			scaleAndRotateAnimator = GetComponent<Animator>();
			setupListeners();
			scaleAndRotateAnimator.SetBool("IsHidden", true);
		}

		private void OnEnable()
		{
			if (scaleAndRotateAnimator != null)
			{
				scaleAndRotateAnimator.SetBool("IsHidden", true);
			}
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(CustomizationContext.EventBus);
			eventChannel.AddListener<CustomizerWidgetEvents.ShowScaleAndRotateWidget>(onShowRotateScaleWidgets);
			eventChannel.AddListener<CustomizerWidgetEvents.HideScaleAndRotateWidget>(onHideRotateScaleWidgets);
			eventChannel.AddListener<CustomizerWidgetEvents.SetSliderWidgetValue>(onSetSliderWidgetValue);
			eventChannel.AddListener<CustomizerWidgetEvents.SetIsSliderWidgetInteractable>(onSetIsSliderWidgetInteractable);
			sliderWidget = SliderWidget;
		}

		private bool onShowRotateScaleWidgets(CustomizerWidgetEvents.ShowScaleAndRotateWidget evt)
		{
			if (scaleAndRotateAnimator.GetBool("IsHidden"))
			{
				scaleAndRotateAnimator.SetBool("IsHidden", false);
				scaleAndRotateAnimator.ResetTrigger("Hide");
				scaleAndRotateAnimator.SetTrigger("Show");
			}
			return false;
		}

		private bool onHideRotateScaleWidgets(CustomizerWidgetEvents.HideScaleAndRotateWidget evt)
		{
			if (!scaleAndRotateAnimator.GetBool("IsHidden"))
			{
				scaleAndRotateAnimator.SetBool("IsHidden", true);
				scaleAndRotateAnimator.ResetTrigger("Show");
				scaleAndRotateAnimator.SetTrigger("Hide");
			}
			return false;
		}

		private bool onSetSliderWidgetValue(CustomizerWidgetEvents.SetSliderWidgetValue evt)
		{
			if (SliderWidget != null)
			{
				SliderWidget.value = evt.Value;
			}
			return false;
		}

		private bool onSetIsSliderWidgetInteractable(CustomizerWidgetEvents.SetIsSliderWidgetInteractable evt)
		{
			if (SliderWidget != null)
			{
				SliderWidget.SetInteractable(evt.Value);
			}
			return false;
		}

		private void onScaleChanged(float scale)
		{
			CustomizationContext.EventBus.DispatchEvent(new CustomizerWidgetEvents.SliderWidgetValueChanged(scale));
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			if (SliderWidget != null)
			{
				SliderWidget.onValueChanged.RemoveListener(onScaleChanged);
			}
		}
	}
}
