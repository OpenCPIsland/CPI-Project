using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tweaker.Core;

namespace Tweaker.UI
{
	public class TweakerTree
	{
		public ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		public TreeGraph<BaseNode> Tree
		{
			get;
			private set;
		}

		public Tweaker Tweaker
		{
			get;
			private set;
		}

		private Dictionary<string, GroupNode> GroupNodes
		{
			get;
			set;
		}

		public TweakerTree(Tweaker tweaker)
		{
			Tweaker = tweaker;
		}

		public void BuildTree(SearchOptions searchOptions = null)
		{
			Tree = new TreeGraph<BaseNode>(new GroupNode("Root", "Root"));
			GroupNodes = new Dictionary<string, GroupNode>();
			TweakerDictionary<IInvokable> invokables = Tweaker.Invokables.GetInvokables(searchOptions);
			TweakerDictionary<ITweakable> tweakables = Tweaker.Tweakables.GetTweakables(searchOptions);
			List<ITweakerObject> list = new List<ITweakerObject>();
			list.AddRange(invokables.Values.Where(PublicTweak.IsUnlocked).ToArray());
			list.AddRange(tweakables.Values.Where(PublicTweak.IsUnlocked).ToArray());
			foreach (ITweakerObject item in list)
			{
				string name = item.Name;
				string text = "";
				int num = name.LastIndexOf('.');
				if (num >= 0)
				{
					text = name.Substring(0, num);
				}
				TreeNode<BaseNode> parent = Tree.Root;
				if (!string.IsNullOrEmpty(text))
				{
					parent = EnsureGroupExists(text);
				}
				CreateTweakerNode(parent, item);
			}
			SortGroupChildren();
		}

		private void SortGroupChildren()
		{
			List<BaseNode> list = new List<BaseNode>();
			list.Add(Tree.Root.Value);
			List<BaseNode> list2 = list;
			foreach (TreeNode<BaseNode> branchNode in Tree.Root.GetBranchNodes())
			{
				list2.Add(branchNode.Value);
			}
			foreach (BaseNode item in list2)
			{
				List<GroupNode> list3 = new List<GroupNode>();
				List<InvokableNode> list4 = new List<InvokableNode>();
				List<TweakableNode> list5 = new List<TweakableNode>();
				List<WatchableNode> list6 = new List<WatchableNode>();
				List<BaseNode> list7 = new List<BaseNode>();
				foreach (BaseNode child in item.Children)
				{
					switch (child.Value.Type)
					{
					case BaseNode.NodeType.Group:
						list3.Add(child.Value as GroupNode);
						break;
					case BaseNode.NodeType.Invokable:
						list4.Add(child.Value as InvokableNode);
						break;
					case BaseNode.NodeType.Tweakable:
						list5.Add(child.Value as TweakableNode);
						break;
					case BaseNode.NodeType.Watchable:
						list6.Add(child.Value as WatchableNode);
						break;
					default:
						list7.Add(child.Value);
						break;
					}
				}
				List<BaseNode> sortedNodes = new List<BaseNode>(item.Children.Count);
				list3.Sort((GroupNode a, GroupNode b) => a.ShortName.CompareTo(b.ShortName));
				list3.ForEach(delegate(GroupNode n)
				{
					sortedNodes.Add(n);
				});
				list4.Sort((InvokableNode a, InvokableNode b) => a.FullName.CompareTo(b.FullName));
				list4.ForEach(delegate(InvokableNode n)
				{
					sortedNodes.Add(n);
				});
				list5.Sort((TweakableNode a, TweakableNode b) => a.FullName.CompareTo(b.FullName));
				list5.ForEach(delegate(TweakableNode n)
				{
					sortedNodes.Add(n);
				});
				list6.Sort((WatchableNode a, WatchableNode b) => a.FullName.CompareTo(b.FullName));
				list6.ForEach(delegate(WatchableNode n)
				{
					sortedNodes.Add(n);
				});
				list7.Sort((BaseNode a, BaseNode b) => a.FullName.CompareTo(b.FullName));
				list7.ForEach(delegate(BaseNode n)
				{
					sortedNodes.Add(n);
				});
				for (int i = 0; i < item.Children.Count; i++)
				{
					item.Children[i] = sortedNodes[i];
				}
			}
		}

		private TreeNode<BaseNode> EnsureGroupExists(string groupPath)
		{
			string[] array = groupPath.Split('.');
			string text = "";
			TreeNode<BaseNode> treeNode = Tree.Root;
			for (int i = 0; i < array.Length; i++)
			{
				text = ((i <= 0) ? (text + array[i]) : (text + "." + array[i]));
				GroupNode groupNode = GetGroupNode(text);
				treeNode = ((groupNode != null) ? groupNode : CreateGroupNode(text, array[i], treeNode));
			}
			return treeNode;
		}

		public GroupNode GetGroupNode(string groupPath)
		{
			GroupNode value = null;
			GroupNodes.TryGetValue(groupPath, out value);
			return value;
		}

		public BaseNode FindNode(string fullName)
		{
			BaseNode baseNode = Tree.Root;
			string text = fullName;
			if (Path.HasExtension(fullName))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullName);
				baseNode = (GetGroupNode(fileNameWithoutExtension) ?? baseNode);
				text = Path.GetExtension(fullName);
				foreach (BaseNode child in baseNode.Children)
				{
					if (child.FullName == text)
					{
						baseNode = child;
					}
				}
			}
			else if (fullName != baseNode.FullName)
			{
				baseNode = (GetGroupNode(fullName) ?? baseNode);
			}
			return baseNode;
		}

		private GroupNode CreateGroupNode(string fullName, string shortName, TreeNode<BaseNode> parent)
		{
			GroupNode groupNode = new GroupNode(fullName, shortName);
			GroupNodes.Add(fullName, groupNode);
			parent.Children.Add(groupNode);
			return groupNode;
		}

		private TreeNode<BaseNode> CreateTweakerNode(TreeNode<BaseNode> parent, ITweakerObject obj)
		{
			BaseNode baseNode = null;
			if (obj is IInvokable)
			{
				baseNode = new InvokableNode(obj as IInvokable);
			}
			else if (obj is ITweakable)
			{
				baseNode = new TweakableNode(obj as ITweakable);
			}
			else if (obj is IWatchable)
			{
				baseNode = new WatchableNode(obj as IWatchable);
			}
			parent.Children.Add(baseNode);
			return baseNode;
		}
	}
}
