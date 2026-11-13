using System;

namespace UnityEngine.UI.Extensions.ColorPicker
{
	public class ColorPickerControl : MonoBehaviour
	{
		private float _hue = 0f;

		private float _saturation = 0f;

		private float _brightness = 0f;

		private float _red = 0f;

		private float _green = 0f;

		private float _blue = 0f;

		private float _alpha = 1f;

		public ColorChangedEvent onValueChanged = new ColorChangedEvent();

		public HSVChangedEvent onHSVChanged = new HSVChangedEvent();

		public Color CurrentColor
		{
			get
			{
				return new Color(_red, _green, _blue, _alpha);
			}
			set
			{
				if (!(CurrentColor == value))
				{
					_red = value.r;
					_green = value.g;
					_blue = value.b;
					_alpha = value.a;
					RGBChanged();
					SendChangedEvent();
				}
			}
		}

		public float H
		{
			get
			{
				return _hue;
			}
			set
			{
				if (_hue != value)
				{
					_hue = value;
					HSVChanged();
					SendChangedEvent();
				}
			}
		}

		public float S
		{
			get
			{
				return _saturation;
			}
			set
			{
				if (_saturation != value)
				{
					_saturation = value;
					HSVChanged();
					SendChangedEvent();
				}
			}
		}

		public float V
		{
			get
			{
				return _brightness;
			}
			set
			{
				if (_brightness != value)
				{
					_brightness = value;
					HSVChanged();
					SendChangedEvent();
				}
			}
		}

		public float R
		{
			get
			{
				return _red;
			}
			set
			{
				if (_red != value)
				{
					_red = value;
					RGBChanged();
					SendChangedEvent();
				}
			}
		}

		public float G
		{
			get
			{
				return _green;
			}
			set
			{
				if (_green != value)
				{
					_green = value;
					RGBChanged();
					SendChangedEvent();
				}
			}
		}

		public float B
		{
			get
			{
				return _blue;
			}
			set
			{
				if (_blue != value)
				{
					_blue = value;
					RGBChanged();
					SendChangedEvent();
				}
			}
		}

		private float A
		{
			get
			{
				return _alpha;
			}
			set
			{
				if (_alpha != value)
				{
					_alpha = value;
					SendChangedEvent();
				}
			}
		}

		private void Start()
		{
			SendChangedEvent();
		}

		private void RGBChanged()
		{
			HsvColor hsvColor = HSVUtil.ConvertRgbToHsv(CurrentColor);
			_hue = hsvColor.NormalizedH;
			_saturation = hsvColor.NormalizedS;
			_brightness = hsvColor.NormalizedV;
		}

		private void HSVChanged()
		{
			Color color = HSVUtil.ConvertHsvToRgb(_hue * 360f, _saturation, _brightness, _alpha);
			_red = color.r;
			_green = color.g;
			_blue = color.b;
		}

		private void SendChangedEvent()
		{
			onValueChanged.Invoke(CurrentColor);
			onHSVChanged.Invoke(_hue, _saturation, _brightness);
		}

		public void AssignColor(ColorValues type, float value)
		{
			switch (type)
			{
			case ColorValues.R:
				R = value;
				break;
			case ColorValues.G:
				G = value;
				break;
			case ColorValues.B:
				B = value;
				break;
			case ColorValues.A:
				A = value;
				break;
			case ColorValues.Hue:
				H = value;
				break;
			case ColorValues.Saturation:
				S = value;
				break;
			case ColorValues.Value:
				V = value;
				break;
			}
		}

		public float GetValue(ColorValues type)
		{
			switch (type)
			{
			case ColorValues.R:
				return R;
			case ColorValues.G:
				return G;
			case ColorValues.B:
				return B;
			case ColorValues.A:
				return A;
			case ColorValues.Hue:
				return H;
			case ColorValues.Saturation:
				return S;
			case ColorValues.Value:
				return V;
			default:
				throw new NotImplementedException("");
			}
		}
	}
}
