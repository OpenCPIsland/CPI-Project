using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SetTextAction : ScriptableAction
	{
		public string Name;

		public string Content;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			Transform transform = player.transform.Find(Name);
			if (transform == null)
			{
				Log.LogErrorFormatted(player, "SetTextAction {0}: Could not find text object {1}", base.name, Name);
			}
			Text component = transform.GetComponent<Text>();
			component.text = Content;
			yield break;
		}
	}
}
