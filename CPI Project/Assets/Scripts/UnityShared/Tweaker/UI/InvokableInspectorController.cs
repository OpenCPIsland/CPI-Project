using System.Collections.Generic;
using Tweaker.Core;

namespace Tweaker.UI
{
	public class InvokableInspectorController : InspectorController<InvokableNode>
	{
		public IInvokable Invokable
		{
			get;
			private set;
		}

		public override string Title
		{
			get
			{
				return Invokable.Name;
			}
		}

		public InvokableInspectorController(InspectorView view, IHexGridController gridController)
			: base(view, gridController)
		{
		}

		protected override void OnInspectNode()
		{
			if (base.CurrentNode.Type != BaseNode.NodeType.Invokable)
			{
				logger.Error("Invalid node type assigned to invokable controller: {0}", base.CurrentNode.Type);
				return;
			}
			Invokable = base.CurrentNode.Invokable;
			base.OnInspectNode();
			view.Header.TypeText.text = Invokable.MethodSignature;
			foreach (IInspectorContentView item in MakeContentViews())
			{
				if (item != null)
				{
					contentViews.Add(item);
				}
				else
				{
					view.Header.TypeText.text = "[Unsupported] " + Invokable.MethodSignature;
				}
			}
		}

		private IEnumerable<IInspectorContentView> MakeContentViews()
		{
			yield return contentFactory.MakeDescriptionView(Invokable.Description);
		}

		public override void Destroy()
		{
			base.Destroy();
		}
	}
}
