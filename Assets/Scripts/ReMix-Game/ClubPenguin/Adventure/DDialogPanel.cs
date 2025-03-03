using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	public class DDialogPanel
	{
		[TextArea]
		public DialogList.Entry Dialog;

		[HideInInspector]
		public string[] i18nTextArgs;

		public bool ClickToClose = true;
	}
}
