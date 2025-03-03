using JetpackReboot.EnumExtensions;

namespace JetpackReboot
{
	public class mg_jr_SpriteDrawingLayers
	{
		public enum DrawingLayers
		{
			BACKGROUND,
			PARALLAX_B,
			PARALLAX_A,
			TOP_BORDER,
			BOTTOM_BORDER,
			START_PLATFORM,
			OBSTACLE_0,
			OBSTACLE_1,
			OBSTACLE_2,
			OBSTACLE_3,
			OBSTACLE_4,
			TRANSITION_BG,
			PLAYER_PENGUIN_THRUST,
			TINT,
			PLAYER_PENGUIN,
			DOME,
			FX_OVERLAY_0,
			FX_OVERLAY_1,
			MAX
		}

		private static readonly mg_jr_SpriteDrawingLayers instance = new mg_jr_SpriteDrawingLayers();

		public static mg_jr_SpriteDrawingLayers Instance
		{
			get
			{
				return instance;
			}
		}

		private mg_jr_SpriteDrawingLayers()
		{
		}

		public int SpriteOrder(EnvironmentLayer _layer)
		{
			return (int)_layer.ConvertToDrawingLayer();
		}

		public int SpriteOrder(DrawingLayers _layer)
		{
			return (int)_layer;
		}
	}
}
