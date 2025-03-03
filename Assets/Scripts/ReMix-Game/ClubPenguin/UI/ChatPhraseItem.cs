using ClubPenguin.Analytics;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ChatPhraseItem : MonoBehaviour
	{
		public Text ItemText;

		public Action<string, SizzleClipDefinition> ClickAction;

		[SerializeField]
		private ChatPhraseImage image;

		private SizzleClipDefinition sizzleClip;

		private string token;

		public void LoadText(string token, SizzleClipDefinition sizzleClip)
		{
			this.token = token;
			ItemText.text = Service.Get<Localizer>().GetTokenTranslation(token);
			this.sizzleClip = sizzleClip;
			CoroutineRunner.Start(updateImagePosition(), this, "updateImagePosition");
		}

		private IEnumerator updateImagePosition()
		{
			yield return null;
			image.UpdatePosition();
		}

		public void OnClick()
		{
			if (ClickAction != null)
			{
				ClickAction(ItemText.text, sizzleClip);
			}
			Service.Get<ICPSwrveService>().Action("game.quick_chat", token, ItemText.text);
		}

		public void OnDestroy()
		{
			ClickAction = null;
		}
	}
}
