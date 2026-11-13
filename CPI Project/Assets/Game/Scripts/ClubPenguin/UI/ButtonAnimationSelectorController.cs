using UnityEngine;

namespace ClubPenguin.UI
{
	public class ButtonAnimationSelectorController : MonoBehaviour
	{
		public Selector[] Selectors;

		public void OnStateChanged(string state)
		{
			if (Selectors != null)
			{
				int index = 1;
				if (state != null && state == "Disabled")
				{
					index = 0;
				}
				for (int i = 0; i < Selectors.Length; i++)
				{
					Selectors[i].Select(index);
				}
			}
		}
	}
}
