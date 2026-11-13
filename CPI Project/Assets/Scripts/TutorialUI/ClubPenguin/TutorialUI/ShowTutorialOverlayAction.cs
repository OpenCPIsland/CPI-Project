using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.TutorialUI
{
	[ActionCategory("Tutorial")]
	public class ShowTutorialOverlayAction : FsmStateAction
	{
		public string TargetName;

		public FsmGameObject TargetObject;

		public FsmVector2 HighlightPosition;

		public bool AutoSize = false;

		public Vector2 Size = new Vector2(100f, 100f);

		public TutorialOverlayShape Shape;

		public TutorialOverlayArrowPosition ArrowPosition;

		public Vector2 ArrowOffset;

		public Vector2 TextBoxOffset;

		public Vector2 TextBoxPivot = new Vector2(0.5f, 0f);

		public float MaxTextBoxSize = 300f;

		public bool ShowHighlightOutline;

		public string i18nBubbleText = "";

		public FsmString i18TranslatedArg0;

		public bool ShowArrow = true;

		public float Opacity = 0.6f;

		public bool UseTarget = false;

		public bool DisableUI = true;

		public bool EnableTarget = true;

		public bool BlocksRaycast = false;

		public override void OnEnter()
		{
			DTutorialOverlay dTutorialOverlay = new DTutorialOverlay();
			bool flag = true;
			if (UseTarget)
			{
				if (!string.IsNullOrEmpty(TargetName))
				{
					GameObject gameObject = GameObject.Find(TargetName);
					if (gameObject != null)
					{
						dTutorialOverlay.Target = gameObject;
					}
					else
					{
						Disney.LaunchPadFramework.Log.LogErrorFormatted(this, "Tutorial overlay skipped. Target {0} not found. Tutorial: {1}", TargetName, i18TranslatedArg0.Value);
						flag = false;
					}
				}
				else if (TargetObject.Value == null)
				{
					Disney.LaunchPadFramework.Log.LogErrorFormatted(this, "Tutorial overlay skipped. Target object was null. Tutorial: {0}", i18TranslatedArg0.Value);
					flag = false;
				}
				else
				{
					dTutorialOverlay.Target = TargetObject.Value;
				}
			}
			if (flag)
			{
				dTutorialOverlay.Position = HighlightPosition.Value;
				dTutorialOverlay.AutoSize = AutoSize;
				dTutorialOverlay.Size = Size;
				dTutorialOverlay.Shape = Shape;
				dTutorialOverlay.ArrowPosition = ArrowPosition;
				dTutorialOverlay.ArrowOffset = ArrowOffset;
				dTutorialOverlay.TextBoxOffset = TextBoxOffset;
				dTutorialOverlay.TextBoxPivot = TextBoxPivot;
				dTutorialOverlay.MaxTextBoxSize = MaxTextBoxSize;
				dTutorialOverlay.ShowHighlightOutline = ShowHighlightOutline;
				dTutorialOverlay.Text = Service.Get<Localizer>().GetTokenTranslation(i18nBubbleText);
				if (!string.IsNullOrEmpty(i18TranslatedArg0.Value))
				{
					dTutorialOverlay.Text = string.Format(dTutorialOverlay.Text, i18TranslatedArg0.Value);
				}
				dTutorialOverlay.ShowArrow = ShowArrow;
				dTutorialOverlay.Opacity = Opacity;
				dTutorialOverlay.DisableUI = DisableUI;
				dTutorialOverlay.EnableTarget = EnableTarget;
				dTutorialOverlay.BlocksRaycast = BlocksRaycast;
				Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.ShowHighlightOverlay(dTutorialOverlay));
			}
			Finish();
		}
	}
}
