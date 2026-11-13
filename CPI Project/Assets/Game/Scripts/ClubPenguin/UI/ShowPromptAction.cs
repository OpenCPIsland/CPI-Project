using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;

namespace ClubPenguin.UI
{
	public class ShowPromptAction : ScriptableAction
	{
		[Serializable]
		public struct Option
		{
			public DPrompt.ButtonFlags Button;

			public ScriptableAction NextStep;
		}

		public string i18nTitleText;

		public string i18nBodyText;

		public Option[] Options;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			DPrompt.ButtonFlags buttonFlags = DPrompt.ButtonFlags.None;
			for (int i = 0; i < Options.Length; i++)
			{
				buttonFlags = (DPrompt.ButtonFlags)((int)buttonFlags | (1 << (int)Options[i].Button));
			}
			DPrompt data = new DPrompt(i18nTitleText, i18nBodyText, buttonFlags);
			Action<DPrompt.ButtonFlags> clickHandler = delegate(DPrompt.ButtonFlags pressed)
			{
				onClick(pressed, player);
			};
			Service.Get<PromptManager>().ShowPrompt(data, clickHandler);
			while (!player.ActionIsFinished)
			{
				yield return null;
			}
		}

		private void onClick(DPrompt.ButtonFlags pressed, ScriptableActionPlayer player)
		{
			int num = 0;
			while (true)
			{
				if (num < Options.Length)
				{
					if (pressed == Options[num].Button)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			player.NextAction = Options[num].NextStep;
			player.ActionIsFinished = true;
		}
	}
}
