using NUnit.Framework;

namespace JetpackReboot.EnumExtensions
{
	public static class mg_jr_EnvironmentLayerExtensions
	{
		public static string FileNameFragment(this EnvironmentLayer _layer)
		{
			string result = "";
			switch (_layer)
			{
			case EnvironmentLayer.BACKGROUND:
				result = "background";
				break;
			case EnvironmentLayer.PARALLAX_A:
				result = "parallaxA";
				break;
			case EnvironmentLayer.PARALLAX_B:
				result = "parallaxB";
				break;
			case EnvironmentLayer.TOP_BORDER:
				result = "top";
				break;
			case EnvironmentLayer.BOTTOM_BORDER:
				result = "bottom";
				break;
			default:
				Assert.Fail(string.Concat("No case for environment layer '", _layer, "'"));
				break;
			}
			return result;
		}

		public static mg_jr_SpriteDrawingLayers.DrawingLayers ConvertToDrawingLayer(this EnvironmentLayer _layer)
		{
			mg_jr_SpriteDrawingLayers.DrawingLayers result = mg_jr_SpriteDrawingLayers.DrawingLayers.MAX;
			switch (_layer)
			{
			case EnvironmentLayer.BACKGROUND:
				result = mg_jr_SpriteDrawingLayers.DrawingLayers.BACKGROUND;
				break;
			case EnvironmentLayer.PARALLAX_A:
				result = mg_jr_SpriteDrawingLayers.DrawingLayers.PARALLAX_A;
				break;
			case EnvironmentLayer.PARALLAX_B:
				result = mg_jr_SpriteDrawingLayers.DrawingLayers.PARALLAX_B;
				break;
			case EnvironmentLayer.TOP_BORDER:
				result = mg_jr_SpriteDrawingLayers.DrawingLayers.TOP_BORDER;
				break;
			case EnvironmentLayer.BOTTOM_BORDER:
				result = mg_jr_SpriteDrawingLayers.DrawingLayers.BOTTOM_BORDER;
				break;
			default:
				Assert.Fail(string.Concat("No case for environment layer '", _layer, "'"));
				break;
			}
			return result;
		}
	}
}
