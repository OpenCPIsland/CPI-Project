using System.Collections.Generic;
using Tweaker.Core;

namespace Tweaker.UI
{
	public class TweakableInspectorController : InspectorController<TweakableNode>
	{
		public ITweakable Tweakable
		{
			get;
			private set;
		}

		public override string Title
		{
			get
			{
				return Tweakable.Name;
			}
		}

		public TweakableInspectorController(InspectorView view, IHexGridController gridController)
			: base(view, gridController)
		{
		}

		protected override void OnInspectNode()
		{
			if (base.CurrentNode.Type != BaseNode.NodeType.Tweakable)
			{
				logger.Error("Invalid node type assigned to tweakable controller: {0}", base.CurrentNode.Type);
				return;
			}
			Tweakable = base.CurrentNode.Tweakable;
			base.OnInspectNode();
			view.Header.TypeText.text = Tweakable.TweakableType.FullName;
			foreach (IInspectorContentView item in MakeContentViews(Tweakable))
			{
				if (item != null)
				{
					contentViews.Add(item);
				}
				else
				{
					view.Header.TypeText.text = "[Unsupported] " + Tweakable.TweakableType.FullName;
				}
			}
		}

		private IEnumerable<IInspectorContentView> MakeContentViews(ITweakable tweakable)
		{
			yield return contentFactory.MakeDescriptionView(tweakable.Description);
			if (tweakable.TweakableType == typeof(string))
			{
				yield return contentFactory.MakeEditStringView(tweakable);
			}
			else if (tweakable.TweakableType == typeof(bool))
			{
				yield return contentFactory.MakeEditBoolView(tweakable);
			}
			else if (!tweakable.TweakableType.IsEnum)
			{
				if (tweakable.TweakableType.IsNumericType())
				{
					yield return contentFactory.MakeEditNumericView(tweakable);
					if (tweakable.HasRange && !TweakerFlagsUtil.IsSet(TweakableUIFlags.HideRangeSlider, tweakable))
					{
						yield return contentFactory.MakeSliderView(tweakable);
					}
				}
				else
				{
					yield return contentFactory.MakeEditSerializedStringView(tweakable, gridController.Console.Serializer);
				}
			}
			if (tweakable.HasStep)
			{
				yield return contentFactory.MakeStepperView(tweakable);
			}
			if (tweakable.HasToggle)
			{
				InspectorToggleGroupView groupView = contentFactory.MakeToggleGroupView();
				yield return groupView;
				IToggleTweakable toggle = tweakable.Toggle;
				for (int i = 0; i < toggle.ToggleCount; i++)
				{
					yield return contentFactory.MakeToggleValueView(tweakable, toggle, i, groupView.ToggleGroup);
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
		}
	}
}
