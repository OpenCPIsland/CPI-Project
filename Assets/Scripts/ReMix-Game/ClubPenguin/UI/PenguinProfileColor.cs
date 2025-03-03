using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PenguinProfileColor : MonoBehaviour
	{
		public Transform ColorGrid;

		private AvatarDetailsData avatarDetailsData;

		private Dictionary<int, AvatarColorDefinition> avatarColors;

		private int playerColorId = -1;

		private Color initialColor;

		private PrefabContentKey colorSwatchContentKey = new PrefabContentKey("ScreenPenguinProfilePrefabs/PenguinColorSwatch");

		private void Start()
		{
			Content.LoadAsync(onColorSwatchLoaded, colorSwatchContentKey);
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				avatarDetailsData = Service.Get<CPDataEntityCollection>().GetComponent<AvatarDetailsData>(localPlayerHandle);
				if (avatarDetailsData != null)
				{
					initialColor = avatarDetailsData.BodyColor;
				}
			}
			avatarColors = new Dictionary<int, AvatarColorDefinition>();
		}

		private void onColorSwatchLoaded(string path, GameObject colorSwatchPrefab)
		{
			populateColors(colorSwatchPrefab);
		}

		public void OnDestroy()
		{
			if (playerColorId <= -1 || !avatarColors.ContainsKey(playerColorId))
			{
				return;
			}
			Color color;
			if (ColorUtility.TryParseHtmlString("#" + avatarColors[playerColorId].Color, out color))
			{
				if (color != initialColor)
				{
					Profile profile = new Profile();
					profile.colour = playerColorId;
					Service.Get<INetworkServicesManager>().PlayerStateService.SetProfile(profile);
				}
			}
			else
			{
				Log.LogError(this, "AvatarColorDefinition did not contain a valid color");
			}
		}

		private void populateColors(GameObject colorSwatchPrefab)
		{
			List<AvatarColorDefinition> list = new List<AvatarColorDefinition>();
			foreach (KeyValuePair<int, AvatarColorDefinition> item in Service.Get<GameData>().Get<Dictionary<int, AvatarColorDefinition>>())
			{
				list.Add(item.Value);
			}
			list.Sort((AvatarColorDefinition a, AvatarColorDefinition b) => a.ViewOrder.CompareTo(b.ViewOrder));
			for (int i = 0; i < list.Count; i++)
			{
				loadColorSwatch(list[i], colorSwatchPrefab);
			}
		}

		private void loadColorSwatch(AvatarColorDefinition avatarColorDefinition, GameObject colorSwatchPrefab)
		{
			Color color;
			if (avatarColorDefinition != null && ColorUtility.TryParseHtmlString("#" + avatarColorDefinition.Color, out color))
			{
				avatarColors[avatarColorDefinition.ColorId] = avatarColorDefinition;
				GameObject gameObject = UnityEngine.Object.Instantiate(colorSwatchPrefab);
				PenguinProfileColorSwatch component = gameObject.GetComponent<PenguinProfileColorSwatch>();
				component.OnClicked = (Action<int>)Delegate.Combine(component.OnClicked, new Action<int>(onColorSwatchClicked));
				component.SetColor(avatarColorDefinition.ColorId, color, avatarColorDefinition.ColorName);
				gameObject.transform.SetParent(ColorGrid, false);
			}
			else
			{
				Log.LogError(this, "AvatarColorDefinition did not contain a valid color");
			}
		}

		private void onColorSwatchClicked(int colorId)
		{
			if (avatarDetailsData != null)
			{
				Color color;
				if (ColorUtility.TryParseHtmlString("#" + avatarColors[colorId].Color, out color))
				{
					Service.Get<ICPSwrveService>().Action("game.colour_change", avatarColors[colorId].ColorName);
					avatarDetailsData.BodyColor = color;
					playerColorId = colorId;
				}
				else
				{
					Log.LogError(this, "AvatarColorDefinition did not contain a valid color");
				}
			}
		}
	}
}
