using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class DanceBattleFloorController : MonoBehaviour
	{
		private const string COLOR_PROPERTY = "_TintColor";

		public UVRandomTileOffsetter RedUVOffsetter;

		public UVRandomTileOffsetter BlueUVOffsetter;

		public Color RedHighlightColor;

		public Color BlueHighlightColor;

		[Range(0.001f, 100f)]
		public float HighlightAnimSpeed = 1f;

		public Texture2D HighlightAnimTexture;

		private Material redMaterial;

		private Material blueMaterial;

		private Color redDefaultColor;

		private Color blueDefaultColor;

		private float defaultAnimSpeed;

		private Texture defaultTexture;

		private void Start()
		{
			RedUVOffsetter.enabled = false;
			BlueUVOffsetter.enabled = false;
			redMaterial = RedUVOffsetter.GetComponent<Renderer>().material;
			blueMaterial = BlueUVOffsetter.GetComponent<Renderer>().material;
			redDefaultColor = redMaterial.GetColor("_TintColor");
			blueDefaultColor = blueMaterial.GetColor("_TintColor");
			defaultAnimSpeed = RedUVOffsetter.SecondsPerChange;
			defaultTexture = redMaterial.mainTexture;
		}

		public void SetFloorAnim(bool redAnimOn, bool blueAnimOn)
		{
			if (redAnimOn)
			{
				RedUVOffsetter.enabled = true;
				redMaterial.mainTexture = HighlightAnimTexture;
			}
			else
			{
				RedUVOffsetter.enabled = false;
				redMaterial.mainTexture = defaultTexture;
			}
			if (blueAnimOn)
			{
				BlueUVOffsetter.enabled = true;
				blueMaterial.mainTexture = HighlightAnimTexture;
			}
			else
			{
				BlueUVOffsetter.enabled = false;
				blueMaterial.mainTexture = defaultTexture;
			}
		}

		public void SetFloorHighlight(bool redHighlightOn, bool blueHighlightOn)
		{
			if (redHighlightOn)
			{
				redMaterial.SetColor("_TintColor", RedHighlightColor);
				RedUVOffsetter.SecondsPerChange = HighlightAnimSpeed;
			}
			else
			{
				redMaterial.SetColor("_TintColor", redDefaultColor);
				RedUVOffsetter.SecondsPerChange = defaultAnimSpeed;
			}
			if (blueHighlightOn)
			{
				blueMaterial.SetColor("_TintColor", BlueHighlightColor);
				BlueUVOffsetter.SecondsPerChange = HighlightAnimSpeed;
			}
			else
			{
				blueMaterial.SetColor("_TintColor", blueDefaultColor);
				BlueUVOffsetter.SecondsPerChange = defaultAnimSpeed;
			}
		}
	}
}
