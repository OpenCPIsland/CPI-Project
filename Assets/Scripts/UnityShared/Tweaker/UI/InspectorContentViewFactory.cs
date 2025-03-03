using System;
using System.Collections;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class InspectorContentViewFactory
	{
		private InspectorView inspectorView;

		private ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		private Color errorColor = new Color(1f, 0.5f, 0.5f);

		private Color successColor;

		public InspectorContentViewFactory(InspectorView inspectorView)
		{
			this.inspectorView = inspectorView;
			successColor = inspectorView.StringSmallEditPrefab.InputText.targetGraphic.color;
		}

		public InspectorDescriptionView MakeDescriptionView(string description)
		{
			InspectorDescriptionView inspectorDescriptionView = inspectorView.InstantiateInspectorComponent(inspectorView.DescriptionPrefab);
			if (string.IsNullOrEmpty(description))
			{
				inspectorDescriptionView.DescriptionText.text = "[No Description]";
			}
			else
			{
				inspectorDescriptionView.DescriptionText.text = description;
			}
			return inspectorDescriptionView;
		}

		public InspectorStringView MakeEditStringView(ITweakable tweakable)
		{
			InspectorStringView inspectorStringView = inspectorView.InstantiateInspectorComponent(inspectorView.StringEditPrefab);
			inspectorStringView.gameObject.SetActive(true);
			inspectorStringView.InputText.targetGraphic.color = successColor;
			inspectorStringView.StartCoroutine(delayInitEditStringView(inspectorStringView, tweakable));
			return inspectorStringView;
		}

		private IEnumerator delayInitEditStringView(InspectorStringView stringView, ITweakable tweakable)
		{
			yield return null;
			object value = tweakable.GetValue();
			if (value != null)
			{
				stringView.InputText.text = value.ToString();
			}
			else
			{
				stringView.InputText.text = "";
			}
			Action<object, object> tweakableValueChanged = delegate(object oldValue, object newValue)
			{
				stringView.InputText.text = newValue.ToString();
			};
			tweakable.ValueChanged += tweakableValueChanged;
			stringView.Destroyed += delegate
			{
				tweakable.ValueChanged -= tweakableValueChanged;
			};
			stringView.ValueChanged += tweakable.SetValue;
		}

		public InspectorStringView MakeEditSerializedStringView(ITweakable tweakable, ITweakerSerializer serializer)
		{
			InspectorStringView inspectorStringView = inspectorView.InstantiateInspectorComponent(inspectorView.StringEditPrefab);
			inspectorStringView.InputText.targetGraphic.color = successColor;
			inspectorStringView.StartCoroutine(delayInitEditSerializedStringView(inspectorStringView, tweakable, serializer));
			return inspectorStringView;
		}

		public IEnumerator delayInitEditSerializedStringView(InspectorStringView stringView, ITweakable tweakable, ITweakerSerializer serializer)
		{
			yield return null;
			object value = tweakable.GetValue();
			if (value != null)
			{
				stringView.InputText.text = serializer.Serialize(value);
			}
			else
			{
				stringView.InputText.text = "";
			}
			stringView.ValueChanged += delegate(string newValue)
			{
				object obj = serializer.Deserialize(newValue, tweakable.TweakableType);
				if (obj != null)
				{
					tweakable.SetValue(obj);
				}
				else
				{
					logger.Warn("Failed to deserialize string to type '" + tweakable.TweakableType.FullName + "': " + newValue);
				}
			};
			Action<object, object> tweakableValueChanged = delegate(object oldValue, object newValue)
			{
				stringView.InputText.text = serializer.Serialize(newValue);
			};
			tweakable.ValueChanged += tweakableValueChanged;
			stringView.Destroyed += delegate
			{
				tweakable.ValueChanged -= tweakableValueChanged;
			};
			stringView.gameObject.SetActive(true);
		}

		public InspectorStringView MakeEditNumericView(ITweakable tweakable)
		{
			InspectorStringView stringView = inspectorView.InstantiateInspectorComponent(inspectorView.StringSmallEditPrefab);
			stringView.InputText.text = tweakable.GetValue().ToString();
			stringView.ValueChanged += delegate(string newValue)
			{
				object obj = null;
				long result;
				double result2;
				decimal result3;
				if (long.TryParse(newValue, out result))
				{
					obj = result;
				}
				else if (double.TryParse(newValue, out result2))
				{
					obj = result2;
				}
				else if (decimal.TryParse(newValue, out result3))
				{
					obj = result3;
				}
				if (obj == null)
				{
					logger.Warn("Failed to parse string to numeric type: {0}", newValue);
					stringView.InputText.targetGraphic.color = errorColor;
				}
				else
				{
					object obj2 = Convert.ChangeType(obj, tweakable.TweakableType);
					if (obj2 == null)
					{
						logger.Warn("Failed to convert value '{0}' of type {1} to tweakable of type {2}.", obj.ToString(), obj.GetType().FullName, tweakable.TweakableType.FullName);
						stringView.InputText.targetGraphic.color = errorColor;
					}
					else
					{
						tweakable.SetValue(obj2);
						stringView.InputText.targetGraphic.color = successColor;
						stringView.InputText.text = tweakable.GetValue().ToString();
					}
				}
			};
			Action<object, object> tweakableValueChanged = delegate(object oldValue, object newValue)
			{
				stringView.InputText.text = newValue.ToString();
				stringView.InputText.targetGraphic.color = successColor;
			};
			tweakable.ValueChanged += tweakableValueChanged;
			stringView.Destroyed += delegate
			{
				tweakable.ValueChanged -= tweakableValueChanged;
			};
			stringView.gameObject.SetActive(true);
			return stringView;
		}

		public InspectorBoolView MakeEditBoolView(ITweakable tweakable)
		{
			InspectorBoolView boolView = inspectorView.InstantiateInspectorComponent(inspectorView.BoolEditPrefab);
			bool isOn = (bool)tweakable.GetValue();
			boolView.Toggle.isOn = isOn;
			boolView.ToggleText.text = isOn.ToString();
			boolView.ValueChanged += delegate(bool newValue)
			{
				tweakable.SetValue(newValue);
				boolView.ToggleText.text = newValue.ToString();
			};
			Action<object, object> tweakableValueChanged = delegate(object oldValue, object newValue)
			{
				boolView.Toggle.isOn = (bool)newValue;
				boolView.ToggleText.text = newValue.ToString();
			};
			tweakable.ValueChanged += tweakableValueChanged;
			boolView.Destroyed += delegate
			{
				tweakable.ValueChanged -= tweakableValueChanged;
			};
			boolView.gameObject.SetActive(true);
			return boolView;
		}

		public InspectorStepperView MakeStepperView(ITweakable tweakable)
		{
			InspectorStepperView inspectorStepperView = inspectorView.InstantiateInspectorComponent(inspectorView.StepperPrefab);
			inspectorStepperView.NextClicked += delegate
			{
				if (tweakable.HasStep)
				{
					IStepTweakable step2 = tweakable.Step;
					step2.StepNext();
				}
			};
			inspectorStepperView.PrevClicked += delegate
			{
				if (tweakable.HasStep)
				{
					IStepTweakable step = tweakable.Step;
					step.StepPrevious();
				}
			};
			return inspectorStepperView;
		}

		public InspectorToggleGroupView MakeToggleGroupView()
		{
			return inspectorView.InstantiateInspectorComponent(inspectorView.ToggleGroupPrefab);
		}

		public InspectorToggleValueView MakeToggleValueView(ITweakable tweakable, IToggleTweakable toggleTweakable, int toggleIndex, ToggleGroup group)
		{
			InspectorToggleValueView valueView = inspectorView.InstantiateInspectorComponent(inspectorView.ToggleValuePrefab);
			valueView.Toggle.group = group;
			valueView.Toggle.isOn = (toggleTweakable.CurrentIndex == toggleIndex);
			valueView.ToggleText.text = toggleTweakable.GetNameByIndex(toggleIndex);
			Action<object, object> tweakableValueChanged = delegate
			{
				if (toggleTweakable.CurrentIndex == toggleIndex && !valueView.Toggle.isOn)
				{
					valueView.Toggle.isOn = true;
				}
			};
			tweakable.ValueChanged += tweakableValueChanged;
			valueView.Destroyed += delegate
			{
				tweakable.ValueChanged -= tweakableValueChanged;
			};
			valueView.Toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn && toggleIndex != toggleTweakable.CurrentIndex)
				{
					toggleTweakable.SetValueByName(toggleTweakable.GetNameByIndex(toggleIndex));
				}
			});
			return valueView;
		}

		public InspectorSliderView MakeSliderView(ITweakable tweakable)
		{
			InspectorSliderView sliderView = inspectorView.InstantiateInspectorComponent(inspectorView.SliderPrefab);
			Type tweakableType = tweakable.TweakableType;
			if (!tweakableType.IsNumericType())
			{
				return null;
			}
			if (tweakableType == typeof(int) || tweakableType == typeof(uint) || tweakableType == typeof(long) || tweakableType == typeof(ulong) || tweakableType == typeof(short) || tweakableType == typeof(ushort))
			{
				sliderView.Slider.wholeNumbers = true;
			}
			else
			{
				sliderView.Slider.wholeNumbers = false;
			}
			sliderView.Slider.minValue = (float)Convert.ChangeType(tweakable.MinValue, typeof(float));
			sliderView.Slider.maxValue = (float)Convert.ChangeType(tweakable.MaxValue, typeof(float));
			sliderView.Slider.value = (float)Convert.ChangeType(tweakable.GetValue(), typeof(float));
			sliderView.ValueChanged += delegate(float newValue)
			{
				tweakable.SetValue(Convert.ChangeType(newValue, tweakable.TweakableType));
			};
			Action<object, object> tweakableValueChanged = delegate(object oldValue, object newValue)
			{
				sliderView.Slider.value = (float)Convert.ChangeType(newValue, typeof(float));
			};
			tweakable.ValueChanged += tweakableValueChanged;
			sliderView.Destroyed += delegate
			{
				tweakable.ValueChanged -= tweakableValueChanged;
			};
			return sliderView;
		}
	}
}
