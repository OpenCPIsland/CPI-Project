using UnityEngine;

namespace ClubPenguin.UI
{
	public class DCinematicSpeech
	{
		public string Text
		{
			get;
			set;
		}

		public string BubbleContentKey
		{
			get;
			set;
		}

		public DButton[] Buttons
		{
			get;
			set;
		}

		public string BackgroundImageKey
		{
			get;
			set;
		}

		public DTextStyle TextStyle
		{
			get;
			set;
		}

		public bool RichText
		{
			get;
			set;
		}

		public GameObject MascotTarget
		{
			get;
			set;
		}

		public string ContentImageKey
		{
			get;
			set;
		}

		public string MascotName
		{
			get;
			set;
		}

		public float DismissTime
		{
			get;
			set;
		}

		public bool CenterX
		{
			get;
			set;
		}

		public bool CenterY
		{
			get;
			set;
		}

		public float OffsetY
		{
			get;
			set;
		}

		public float OffsetYPercent
		{
			get;
			set;
		}

		public bool HideTail
		{
			get;
			set;
		}

		public bool ClickToClose
		{
			get;
			set;
		}

		public bool KeepTextStyle
		{
			get;
			set;
		}

		public bool ShowContinueImageImmediately
		{
			get;
			set;
		}
	}
}
