using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner
{
	public class ClothingDesignerTheme : MonoBehaviour
	{
		private const string TINT_COLOR_PROPERTY = "_TintColor";

		[Header("UI")]
		public GameObject BrickBackground;

		public GameObject FishImage;

		public GameObject CatalogThemeBackground;

		[Header("Scene Objects")]
		public Renderer CustomizerPlatform;

		public GameObject CatalogChallengeObjects;

		public Renderer CatalogChallengeCurtain;

		public Color DefaultColor;

		public Vector3 platformPositionCustomizer;

		public Vector3 platformPositionCatalog;

		private EventChannel eventChannel;

		private int shaderTintProperty;

		private void Start()
		{
			shaderTintProperty = Shader.PropertyToID("_TintColor");
			setupListeners();
			setThemeToDefault();
		}

		private void setupListeners()
		{
			eventChannel = new EventChannel(ClothingDesignerContext.EventBus);
			eventChannel.AddListener<ClothingDesignerEvents.ResetClothingDesignerTheme>(onResetClothingDesignerTheme);
			eventChannel.AddListener<ClothingDesignerEvents.UpdateClothingDesignerTheme>(onUpdateClothingDesignerTheme);
		}

		private bool onResetClothingDesignerTheme(ClothingDesignerEvents.ResetClothingDesignerTheme evt)
		{
			setThemeToDefault();
			return false;
		}

		private bool onUpdateClothingDesignerTheme(ClothingDesignerEvents.UpdateClothingDesignerTheme evt)
		{
			setTheme(evt.ThemeColors);
			return false;
		}

		private void setThemeToDefault()
		{
			BrickBackground.SetActive(true);
			FishImage.SetActive(true);
			CatalogThemeBackground.SetActive(false);
			CustomizerPlatform.sharedMaterial.SetColor(shaderTintProperty, DefaultColor);
			CatalogChallengeObjects.SetActive(false);
			CustomizerPlatform.transform.position = platformPositionCustomizer;
		}

		private void setTheme(Color[] themeColors)
		{
			CatalogThemeBackground.SetActive(true);
			BrickBackground.SetActive(false);
			FishImage.SetActive(false);
			CatalogChallengeObjects.SetActive(true);
			CatalogChallengeCurtain.sharedMaterial.SetColor(shaderTintProperty, themeColors[0]);
			CustomizerPlatform.sharedMaterial.SetColor(shaderTintProperty, themeColors[0]);
			CustomizerPlatform.transform.position = platformPositionCatalog;
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}
	}
}
