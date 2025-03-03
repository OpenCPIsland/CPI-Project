using UnityEngine;
using UnityEngine.UI;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(Image))]
	public class SolidColorFiller : MonoBehaviour
	{
		public Color FillColor;

		public Image FillColorSourceImage;

		private void Start()
		{
			Image component = GetComponent<Image>();
			Color color2 = component.color = ((FillColorSourceImage != null) ? FillColorSourceImage.color : FillColor);
		}
	}
}
