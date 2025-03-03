using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(Text))]
	public class FontMaterialLoader : MonoBehaviour
	{
		public MaterialContentKey FontMaterial;

		private Text text;

		private void Awake()
		{
			text = GetComponent<Text>();
			Content.LoadAsync(onFontMaterialLoaded, FontMaterial);
		}

		private void onFontMaterialLoaded(string key, Material fontMaterial)
		{
			if (fontMaterial != null)
			{
				text.material = fontMaterial;
			}
			else
			{
				Log.LogError(this, "Loaded a null material for MaterialContentKey: " + FontMaterial.Key);
			}
		}
	}
}
