namespace Tweaker.UI
{
	public class GroupNode : BaseNode
	{
		public string ShortName
		{
			get;
			private set;
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.Group;
			}
		}

		public GroupNode(string fullName, string shortName)
			: base(fullName)
		{
			ShortName = shortName;
		}
	}
}
