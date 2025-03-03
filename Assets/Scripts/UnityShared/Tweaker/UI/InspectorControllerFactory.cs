using Tweaker.Core;

namespace Tweaker.UI
{
	public static class InspectorControllerFactory
	{
		public static IInspectorController MakeController(InspectorView view, IHexGridController grid, BaseNode.NodeType type)
		{
			IInspectorController result = null;
			switch (type)
			{
			case BaseNode.NodeType.Group:
				result = new GroupInspectorController(view, grid);
				break;
			case BaseNode.NodeType.Invokable:
				result = new InvokableInspectorController(view, grid);
				break;
			case BaseNode.NodeType.Tweakable:
				result = new TweakableInspectorController(view, grid);
				break;
			default:
				LogManager.GetCurrentClassLogger().Error("Cannot inspect node of type {0}", type);
				result = null;
				break;
			case BaseNode.NodeType.Watchable:
				break;
			}
			return result;
		}
	}
}
