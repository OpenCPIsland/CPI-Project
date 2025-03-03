using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.TutorialUI
{
	[ActionCategory("Tutorial")]
	public class ShowTutorialTooltipAction : FsmStateAction
	{
		public string TargetName;

		public Vector2 Offset;

		public FsmVector2 Position;

		public Color OutlineColor = Color.red;

		public bool FullScreenClose = true;

		public string[] TextStrings = new string[0];

		public DTextStyle[] TextStyles = new DTextStyle[0];

		public bool UseTarget = false;

		public override void OnEnter()
		{
			Service.Get<EventDispatcher>().AddListener<TutorialUIEvents.OnTooltipCreated>(onTooltipCreated);
			if (UseTarget)
			{
				if (!string.IsNullOrEmpty(TargetName))
				{
					GameObject gameObject = GameObject.Find(TargetName);
					if (gameObject != null && gameObject.GetComponent<RectTransform>() != null)
					{
						Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.ShowTooltip(null, gameObject.GetComponent<RectTransform>(), Offset, FullScreenClose));
					}
				}
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.ShowTooltip(null, null, Offset, FullScreenClose));
			}
		}

		private bool onTooltipCreated(TutorialUIEvents.OnTooltipCreated evt)
		{
			TutorialTooltip tooltip = evt.Tooltip;
			tooltip.ClearAllText();
			for (int i = 0; i < TextStrings.Length; i++)
			{
				tooltip.AddText(TextStrings[i], ColorUtils.HexToColor(TextStyles[i].ColorHex), TextStyles[i].FontSize);
			}
			Service.Get<EventDispatcher>().RemoveListener<TutorialUIEvents.OnTooltipCreated>(onTooltipCreated);
			Finish();
			return false;
		}
	}
}
