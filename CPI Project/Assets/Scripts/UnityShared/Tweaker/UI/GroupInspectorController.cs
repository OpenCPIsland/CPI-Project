namespace Tweaker.UI
{
	public class GroupInspectorController : InspectorController<GroupNode>
	{
		public override string Title
		{
			get
			{
				return base.CurrentNode.FullName;
			}
		}

		public GroupInspectorController(InspectorView view, IHexGridController gridController)
			: base(view, gridController)
		{
		}

		protected override void OnInspectNode()
		{
			if (base.CurrentNode.Type != BaseNode.NodeType.Group)
			{
				logger.Error("Invalid node type assigned to group controller: {0}", base.CurrentNode.Type);
			}
			else
			{
				base.OnInspectNode();
			}
		}

		public override void Destroy()
		{
			base.Destroy();
		}
	}
}
