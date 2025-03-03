using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MulticoloredList : MonoBehaviour
	{
		public Color[] Colors;

		private void Start()
		{
			setColors();
		}

		public void Refresh()
		{
			setColors();
		}

		public Color GetColorForIndex(int index)
		{
			int num = index % Colors.Length;
			return Colors[num];
		}

		private void setColors()
		{
			MulticoloredListElement[] componentsInChildren = base.transform.GetComponentsInChildren<MulticoloredListElement>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Image component = componentsInChildren[i].GetComponent<Image>();
				component.color = GetColorForIndex(i);
			}
		}
	}
}
