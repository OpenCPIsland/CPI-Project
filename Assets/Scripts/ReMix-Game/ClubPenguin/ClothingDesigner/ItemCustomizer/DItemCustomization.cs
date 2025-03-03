using ClubPenguin.Avatar;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class DItemCustomization
	{
		public DCustomizationChannel[] customizationChannelList;

		private DCustomizationChannel currentChannel;

		private Sprite templateSprite;

		private Texture2D[] _defaultTextures = null;

		private Dictionary<string, bool> nameToInteractable;

		public DCustomEquipment CustomEquipmentModel
		{
			get;
			set;
		}

		public bool WearNowSet
		{
			get;
			set;
		}

		public bool IsChanged
		{
			get;
			private set;
		}

		public TemplateDefinition TemplateDefinition
		{
			get;
			private set;
		}

		public Sprite TemplateSprite
		{
			get
			{
				return templateSprite;
			}
			private set
			{
				if (value == null)
				{
					throw new NullReferenceException(string.Format("Attempting to set a null value for {0} sprite.", TemplateDefinition.AssetName));
				}
				templateSprite = value;
			}
		}

		public DCustomizationChannel CurrentChannel
		{
			get
			{
				return currentChannel;
			}
		}

		public DCustomizationChannel RedChannel
		{
			get
			{
				return customizationChannelList[0];
			}
		}

		public DCustomizationChannel GreenChannel
		{
			get
			{
				return customizationChannelList[1];
			}
		}

		public DCustomizationChannel BlueChannel
		{
			get
			{
				return customizationChannelList[2];
			}
		}

		public DItemCustomization()
		{
			Dictionary<int, FabricDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
			nameToInteractable = new Dictionary<string, bool>();
			foreach (KeyValuePair<int, FabricDefinition> item in dictionary)
			{
				if (!nameToInteractable.ContainsKey(item.Value.AssetName))
				{
					nameToInteractable.Add(item.Value.AssetName, item.Value.allowRotationAndScale);
				}
				else
				{
					Log.LogErrorFormatted(this, "The key {0} already exists in nameToInteractable.", item.Value.AssetName);
				}
			}
			ResetChannels();
		}

		public void Reset()
		{
			ResetChannels();
			TemplateDefinition = null;
			IsChanged = false;
			templateSprite = null;
			ApplyDefaultTextures();
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerModelEvents.ResetItemModelEvent));
		}

		public void ResetChannels()
		{
			customizationChannelList = new DCustomizationChannel[3];
			customizationChannelList[0] = new DCustomizationChannel(DCustomizationChannel.ChannelMask.RED);
			customizationChannelList[1] = new DCustomizationChannel(DCustomizationChannel.ChannelMask.GREEN);
			customizationChannelList[2] = new DCustomizationChannel(DCustomizationChannel.ChannelMask.BLUE);
			currentChannel = customizationChannelList[0];
			for (int i = 0; i < customizationChannelList.Length; i++)
			{
				customizationChannelList[i].SetFabricNameToInteractable(nameToInteractable);
			}
			ApplyDefaultTextures();
		}

		public void SetTemplate(TemplateDefinition templateDefinition, Sprite templateSprite)
		{
			TemplateDefinition = templateDefinition;
			TemplateSprite = templateSprite;
			CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.TemplateChangedEvent(TemplateDefinition));
		}

		public void SetChannelFabric(DCustomizationChannel channelToAlter, Texture2D fabricTexture, Vector2 position)
		{
			SelectChannel(channelToAlter);
			channelToAlter.SetFabric(fabricTexture);
			channelToAlter.FabricUVOffset = position;
			IsChanged = true;
		}

		public void SetChannelDecal(DCustomizationChannel channelToAlter, Texture2D decalTexture, Vector2 position, Renderer chosenRenderer)
		{
			SelectChannel(channelToAlter);
			channelToAlter.SetDecal(decalTexture, chosenRenderer);
			channelToAlter.DecalUVOffset = position;
			IsChanged = true;
		}

		public void SelectChannel(DCustomizationChannel newChannel)
		{
			for (int i = 0; i < customizationChannelList.Length; i++)
			{
				if (customizationChannelList[i] == newChannel)
				{
					customizationChannelList[i].Select();
					currentChannel = customizationChannelList[i];
				}
				else
				{
					customizationChannelList[i].Deselect();
				}
			}
			CustomizationContext.EventBus.DispatchEvent(new CustomizerModelEvents.ChannelChangedEvent(newChannel));
		}

		public void SetSelectionColors(Color darkenedColor)
		{
			for (int i = 0; i < customizationChannelList.Length; i++)
			{
				customizationChannelList[i].SetSelectionColors(darkenedColor);
			}
		}

		public void SetCanFade(bool canFade)
		{
			for (int i = 0; i < customizationChannelList.Length; i++)
			{
				customizationChannelList[i].SetCanFade(canFade);
			}
		}

		public void SetDefaultTextures(Texture2D[] textures)
		{
			_defaultTextures = textures;
			ApplyDefaultTextures();
		}

		private void ApplyDefaultTextures()
		{
			if (_defaultTextures != null)
			{
				RedChannel.SetDefaultTexture(_defaultTextures[0]);
				GreenChannel.SetDefaultTexture(_defaultTextures[1]);
				BlueChannel.SetDefaultTexture(_defaultTextures[2]);
			}
		}
	}
}
