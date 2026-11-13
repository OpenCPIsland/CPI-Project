using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class CinematicSpeechState
	{
		public static void HideChatUI()
		{
			Canvas[] componentsInChildren = GameObject.FindWithTag(UIConstants.Tags.UI_Chat).GetComponentsInChildren<Canvas>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].GetComponent<CinematicSpeechController>() == null)
				{
					componentsInChildren[i].enabled = false;
				}
			}
		}

		public static void ShowChatUI()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Chat);
			if (!(gameObject != null))
			{
				return;
			}
			Canvas[] componentsInChildren = gameObject.GetComponentsInChildren<Canvas>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].GetComponent<CinematicSpeechController>() == null)
				{
					componentsInChildren[i].enabled = true;
				}
			}
		}
	}
}
