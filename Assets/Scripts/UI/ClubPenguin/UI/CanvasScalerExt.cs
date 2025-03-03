using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Canvas))]
	[AddComponentMenu("Layout/Canvas Scaler Ext", 102)]
	[ExecuteInEditMode]
	public class CanvasScalerExt : CanvasScaler
	{
		protected const float DEFAULT_SCREEN_DPI = 96f;

		protected const float IPAD_DIAGONAL_INCHES = 10f;

		protected const float IPHONE5_DIAGONAL_INCHES = 4f;

		private const string SCALE_PLAYERPREFS_KEY = "accessibility_scale";

		[FormerlySerializedAs("scaleModifierIphone")]
		public float ScaleModifierSmallDevice = 1f;

		[FormerlySerializedAs("scaleModifierIpad")]
		public float ScaleModifierLargeDevice = 0.7f;

		public float AccessibilityMultiplier = 1f;

		private float scaleAccessibilityModifier = 1f;

		private bool scaleFactorSet = false;

		private float screenDPI;

		public bool ScaleFactorSet
		{
			get
			{
				return scaleFactorSet;
			}
		}

		public float ScaleModifier
		{
			get;
			set;
		}

		private float screenWidth
		{
			get
			{
				return Screen.width;
			}
		}

		private float screenHeight
		{
			get
			{
				return Screen.height;
			}
		}

		public float ReferenceResolutionX
		{
			get
			{
				return base.referenceResolution.x;
			}
			set
			{
				Vector2 referenceResolution = base.referenceResolution;
				referenceResolution.x = value;
				base.referenceResolution = referenceResolution;
			}
		}

		public float ReferenceResolutionY
		{
			get
			{
				return base.referenceResolution.y;
			}
			set
			{
				Vector2 referenceResolution = base.referenceResolution;
				referenceResolution.y = value;
				base.referenceResolution = referenceResolution;
			}
		}

		public void SetScaleAccessibilityModifier(float modifier)
		{
			scaleAccessibilityModifier = modifier;
			HandleScaleWithScreenSize();
		}

		protected override void Start()
		{
			if (Service.IsSet<EventDispatcher>())
			{
				Service.Get<EventDispatcher>().AddListener<UIEvents.CanvasScalerModifierUpdated>(onCanvasScalerModifierUpdated);
			}
			if (PlayerPrefs.HasKey("accessibility_scale"))
			{
				updateScaleModifier(PlayerPrefs.GetFloat("accessibility_scale"));
			}
		}

		protected override void OnDestroy()
		{
			if (Service.IsSet<EventDispatcher>())
			{
				Service.Get<EventDispatcher>().RemoveListener<UIEvents.CanvasScalerModifierUpdated>(onCanvasScalerModifierUpdated);
			}
		}

		private bool onCanvasScalerModifierUpdated(UIEvents.CanvasScalerModifierUpdated evt)
		{
			updateScaleModifier(evt.Scale);
			return false;
		}

		private void updateScaleModifier(float scale)
		{
			scaleAccessibilityModifier = scale;
			HandleScaleWithScreenSize();
		}

		protected override void HandleScaleWithScreenSize()
		{
			Vector2 vector = new Vector2(screenWidth, screenHeight);
			float num = 0f;
			switch (m_ScreenMatchMode)
			{
			case ScreenMatchMode.MatchWidthOrHeight:
			{
				float a = Mathf.Log(vector.x / m_ReferenceResolution.x, 2f);
				float b = Mathf.Log(vector.y / m_ReferenceResolution.y, 2f);
				float p = Mathf.Lerp(a, b, m_MatchWidthOrHeight);
				num = Mathf.Pow(2f, p);
				break;
			}
			case ScreenMatchMode.Expand:
				num = Mathf.Min(vector.x / m_ReferenceResolution.x, vector.y / m_ReferenceResolution.y);
				break;
			case ScreenMatchMode.Shrink:
				num = Mathf.Max(vector.x / m_ReferenceResolution.x, vector.y / m_ReferenceResolution.y);
				break;
			}
			float deviceSize = GetDeviceSize();
			float num2 = NormalizeScaleProperty(ScaleModifierLargeDevice, ScaleModifierSmallDevice);
			float num3 = ScaleModifierSmallDevice + (deviceSize - 4f) * num2;
			float num4 = Mathf.Lerp(1f, scaleAccessibilityModifier, AccessibilityMultiplier);
			ScaleModifier = num3 * num4;
			num *= ScaleModifier;
			SetScaleFactor(num);
			SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
			scaleFactorSet = true;
		}

		protected float GetDeviceSize()
		{
			return Mathf.Sqrt(Mathf.Pow(screenWidth / GetScreenDPI(), 2f) + Mathf.Pow(screenHeight / GetScreenDPI(), 2f));
		}

		protected float NormalizeScaleProperty(float iPadValue, float iPhoneValue)
		{
			return (iPadValue - iPhoneValue) / 6f;
		}

		protected float GetScreenDPI()
		{
			if (screenDPI == 0f)
			{
				screenDPI = Screen.dpi;
				if (screenDPI == 0f || screenDPI == -1f)
				{
					screenDPI = 96f;
				}
			}
			return screenDPI;
		}
	}
}
