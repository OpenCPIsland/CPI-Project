using System.Runtime.InteropServices;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public static class CustomizerWidgetEvents
	{
		public struct RotationWdigetSetIsInteractable
		{
			public readonly bool IsInteractable;

			public RotationWdigetSetIsInteractable(bool isInteractable)
			{
				IsInteractable = isInteractable;
			}
		}

		public struct RotationWidgetSetValue
		{
			public readonly float Value;

			public RotationWidgetSetValue(float value)
			{
				Value = value;
			}
		}

		public struct RotationWidgetRotated
		{
			public readonly float TotalRotationDegrees;

			public RotationWidgetRotated(float totalRotationDegrees)
			{
				TotalRotationDegrees = totalRotationDegrees;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowTileWidget
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideTileWidget
		{
		}

		public struct SetTileValue
		{
			public readonly bool Value;

			public SetTileValue(bool value)
			{
				Value = value;
			}
		}

		public struct SetIsTileInteractable
		{
			public readonly bool Value;

			public SetIsTileInteractable(bool value)
			{
				Value = value;
			}
		}

		public struct TileValueChanged
		{
			public readonly bool Value;

			public TileValueChanged(bool value)
			{
				Value = value;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowCoinCountWidget
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideCoinCountWidget
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ResetCoinCountWidgetCount
		{
		}

		public struct RemoveCoinsFromWidget
		{
			public readonly int Amount;

			public RemoveCoinsFromWidget(int amount)
			{
				Amount = amount;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowScaleAndRotateWidget
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideScaleAndRotateWidget
		{
		}

		public struct SliderWidgetValueChanged
		{
			public readonly float Value;

			public SliderWidgetValueChanged(float value)
			{
				Value = value;
			}
		}

		public struct SetSliderWidgetValue
		{
			public readonly float Value;

			public SetSliderWidgetValue(float value)
			{
				Value = value;
			}
		}

		public struct SetIsSliderWidgetInteractable
		{
			public readonly bool Value;

			public SetIsSliderWidgetInteractable(bool value)
			{
				Value = value;
			}
		}
	}
}
