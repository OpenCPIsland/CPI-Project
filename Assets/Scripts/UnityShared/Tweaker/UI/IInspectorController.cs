using System;

namespace Tweaker.UI
{
	public interface IInspectorController : IViewController
	{
		BaseNode.NodeType NodeType
		{
			get;
		}

		BaseNode CurrentBaseNode
		{
			get;
		}

		string Title
		{
			get;
		}

		event Action Closed;

		void InspectNode(BaseNode node);

		void Resize();
	}
}
