using ClubPenguin.Adventure;
using ClubPenguin.Progression;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MascotProgressDisplayItem : MonoBehaviour
	{
		public Image MascotIconImage;

		public RadialProgressBar progressBar;

		public Text LevelText;

		public string MascotName;

		private static SpriteContentKey mascotImageContentKey = new SpriteContentKey("Sprites/Quests_XPDails_FG_*");

		private void Start()
		{
			if (!string.IsNullOrEmpty(MascotName))
			{
				Mascot mascotFromString = getMascotFromString(MascotName);
				if (mascotFromString != null)
				{
					LoadMascot(mascotFromString);
				}
				else
				{
					Log.LogError(this, "Could not find mascot of name " + MascotName + " in mascot service.");
				}
			}
		}

		public void LoadMascot(Mascot mascot)
		{
			progressBar.Pause();
			progressBar.SetProgress(0f);
			progressBar.AnimateProgress(Service.Get<ProgressionService>().MascotLevelPercent(mascot.Name));
			LevelText.text = Service.Get<ProgressionService>().MascotLevel(mascot.Name).ToString();
			CoroutineRunner.Start(loadMascotIcon(mascotImageContentKey, mascot.Name), this, "MascotProgressDisplayItem.loadMascotIcon");
		}

		private IEnumerator loadMascotIcon(SpriteContentKey iconKey, string mascotName)
		{
			UILoadingController.RegisterLoad(base.gameObject);
			AssetRequest<Sprite> assetRequest = Content.LoadAsync(iconKey, mascotName);
			yield return assetRequest;
			if (assetRequest.Asset == null)
			{
				Log.LogError(this, "Missing mascot icon at " + iconKey);
			}
			MascotIconImage.sprite = assetRequest.Asset;
			UILoadingController.RegisterLoadComplete(base.gameObject);
		}

		private Mascot getMascotFromString(string mascotName)
		{
			return Service.Get<MascotService>().GetMascot(mascotName);
		}
	}
}
