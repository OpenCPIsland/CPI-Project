using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Graphic))]
	public class TintSelector : Selector
	{
		public Color[] Colors;

		private Graphic graphic;

		public override void Select(int index)
		{
			SelectColor(index);
		}

		public void SelectColor(int index)
		{
			if (graphic == null)
			{
				graphic = GetComponent<Graphic>();
			}
			if (Colors != null)
			{
				graphic.color = Colors[index];
			}
			else
			{
				Log.LogErrorFormatted(this, "Colors array was null on game object {0}", base.name);
			}
		}
	}
}
