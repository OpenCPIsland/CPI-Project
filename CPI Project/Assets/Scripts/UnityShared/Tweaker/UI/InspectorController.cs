using System;
using System.Collections.Generic;
using Tweaker.Core;

namespace Tweaker.UI
{
	public abstract class InspectorController<TNode> : IInspectorController, IViewController where TNode : BaseNode
	{
		protected InspectorView view;

		protected InspectorContentViewFactory contentFactory;

		protected List<IInspectorContentView> contentViews;

		protected IHexGridController gridController;

		protected ITweakerConsoleController consoleController;

		protected ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		public BaseNode.NodeType NodeType
		{
			get
			{
				return CurrentBaseNode.Type;
			}
		}

		public BaseNode CurrentBaseNode
		{
			get;
			private set;
		}

		public TNode CurrentNode
		{
			get;
			private set;
		}

		public abstract string Title
		{
			get;
		}

		public event Action Closed;

		public InspectorController(InspectorView view, IHexGridController gridController)
		{
			this.view = view;
			this.gridController = gridController;
			consoleController = gridController.Console;
			contentFactory = new InspectorContentViewFactory(view);
			contentViews = new List<IInspectorContentView>();
			ConfigureViews();
		}

		public void Resize()
		{
			view.Resize();
		}

		public void InspectNode(BaseNode node)
		{
			if (!(node is TNode))
			{
				logger.Error("Invalid node type '{0}' passed to controller of type {1}", node.GetType().Name, GetType().Name);
				return;
			}
			CurrentBaseNode = node;
			CurrentNode = (node as TNode);
			ClearContents();
			OnInspectNode();
		}

		protected virtual void ClearContents()
		{
			foreach (IInspectorContentView contentView in contentViews)
			{
				contentView.DestroySelf();
			}
			contentViews.Clear();
		}

		protected virtual void OnInspectNode()
		{
			view.Header.TitleText.text = Title;
			view.Header.TypeText.text = CurrentBaseNode.GetType().FullName;
		}

		public virtual void Destroy()
		{
			if (this.Closed != null)
			{
				this.Closed();
				this.Closed = null;
			}
			view.Footer.CloseButton.onClick.RemoveAllListeners();
			view.DestroySelf();
		}

		protected virtual void ConfigureViews()
		{
			view.Footer.CloseButton.onClick.AddListener(CloseClicked);
		}

		private void CloseClicked()
		{
			Destroy();
		}
	}
}
