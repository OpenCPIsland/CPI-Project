using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Adventure
{
	public class QuestSubtask : MonoBehaviour
	{
		public Text SubtaskText;

		public Toggle TickToggle;

		public Image Tick;

		public Outline TickOutline;

		public string ID
		{
			get;
			set;
		}

		public void SetTickColor(Color color)
		{
			Tick.color = color;
		}

		public void SetTickOutlineColor(Color color)
		{
			TickOutline.effectColor = color;
		}
	}
}
