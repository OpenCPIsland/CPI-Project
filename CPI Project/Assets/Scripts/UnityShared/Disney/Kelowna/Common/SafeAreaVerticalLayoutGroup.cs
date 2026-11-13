using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(VerticalLayoutGroup))]
	public class SafeAreaVerticalLayoutGroup : MonoBehaviour
	{
		public SafeArea SafeArea;

		public bool UseFillColor;

		public Color FillColor;

		public Image FillColorSourceImage;

		private void Start()
		{
			if (SafeArea == SafeArea.Top || SafeArea == SafeArea.Bottom)
			{
				GameObject gameObject = new GameObject("SafeArea" + SafeArea);
				if (UseFillColor)
				{
					SolidColorFiller solidColorFiller = gameObject.AddComponent<SolidColorFiller>();
					solidColorFiller.FillColor = FillColor;
					solidColorFiller.FillColorSourceImage = FillColorSourceImage;
				}
				SafeAreaLayoutElementSetter safeAreaLayoutElementSetter = gameObject.AddComponent<SafeAreaLayoutElementSetter>();
				safeAreaLayoutElementSetter.SafeArea = SafeArea;
				LayoutElement component = gameObject.GetComponent<LayoutElement>();
				component.flexibleWidth = 1f;
				gameObject.transform.SetParent(base.transform, false);
				if (SafeArea == SafeArea.Top)
				{
					gameObject.transform.SetSiblingIndex(0);
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Component on GameObject {0} needs SafeArea to be set to Top or Bottom. Current value is {1}", base.name, SafeArea);
			}
		}
	}
}
