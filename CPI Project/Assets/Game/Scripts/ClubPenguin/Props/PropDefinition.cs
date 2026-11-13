using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Props
{
	[Serializable]
	[CreateAssetMenu]
	public class PropDefinition : StaticGameDataDefinition, IMemberLocked
	{
		public enum PropTypes
		{
			Consumable,
			InteractiveObject,
			Durable,
			PartyGame
		}

		public enum ConsumableExperience
		{
			OneShot,
			PlayerHeld,
			World,
			PartyGameLobby
		}

		public enum ConsumableSortCategory
		{
			Solo,
			Share,
			Deploy,
			InteractDeploy
		}

		[StaticGameDataDefinitionId]
		public int Id;

		[LocalizationToken]
		public string Name;

		[LocalizationToken]
		public string Description;

		public PropTypes PropType;

		public PrefabContentKey PropAssetContentKey;

		public PrefabContentKey ExperienceContentKey;

		public SpriteContentKey IconContentKey;

		private SpriteContentKey iconFallback = new SpriteContentKey("missing_icon");

		[Tooltip("Name on the server for the type.")]
		public string NameOnServer;

		[Header("Consumable Settings")]
		public ConsumableExperience ExperienceType;

		[Tooltip("This consumable is part of quest objective and can only be used within that quest objective")]
		public bool QuestOnly;

		[Tooltip("The number of coins required to purchase this consumable, 0 for those which can't be purchased")]
		public int Cost;

		[Tooltip("Category used for sorting items in the marketplace")]
		public ConsumableSortCategory SortCategory;

		[Header("World Experience Consumable Settings")]
		[Tooltip("How long this object lives in the world before it is destroyed (0 if there is no destruction timer)")]
		public float TimeToLive;

		[Tooltip("How many interactions it takes to destroy this object in the world (0 if it isn't interactable in that manor)")]
		public int ActionCount;

		[Tooltip("The radius forwhich user must be in to earn the reward when this object is destroyed")]
		public float RewardRadius;

		[Tooltip("The target total of all rewards granted when this object is destroyed, to be split amougst all the players in the reward radius")]
		public RewardDefinition TotalReward;

		[Tooltip("The minimum reward a player is guaranteed to receive if the user count in the reward radius would dictate fewer rewards")]
		public RewardDefinition MinPerPlayerReward;

		[Header("Player Held Experience Consumable Settings")]
		[Tooltip("Do other user's interact with the user holding the item in order to advance the experience")]
		public bool Shareable;

		[Tooltip("The optional consumable (identified by NameOnServer) that the other user is holding after interacting with the user holding this consumable")]
		public string RecipientConsumable;

		[Tooltip("Whether the Server automatically adds the item")]
		public bool ServerAddedItem;

		[Tooltip("Retrieve Animation Type")]
		public RetrieveType RetrieveAnimType;

		[Tooltip("The number of interactions initially available for this object")]
		public int TotalItemQuantity;

		[Tooltip("True if the locomotion controls should be reset ( enabled and disabled ) while the prop is stored.")]
		public bool IsControlsResetOnStore = true;

		[Tooltip("The tags that describe this prop")]
		public TagDefinition[] Tags = new TagDefinition[0];

		[SerializeField]
		[JsonProperty]
		private bool isMemberOnly;

		[Tooltip("The shard a non-member has to be in to access this item. Note that Swrve uses a shard of -1 if there's no A/B test.")]
		public int ShardForNonMemberAccess = -2;

		[JsonIgnore]
		[Tooltip("If checked, the item will now show up by default in marketplaces")]
		public bool HasSpecialMarket;

		[SerializeField]
		[Tooltip("Reference to a ScheduledEventDate that specifies when this prop may be used in game.")]
		public ScheduledEventDateDefinitionKey AvailabilitySchedule;

		[Tooltip("The Names of the quests that non-members can use this item")]
		[JsonProperty]
		public string[] QuestNameExclusions = new string[0];

		[JsonProperty]
		[Tooltip("The Names of the objectives within a quest that non-members can use this item")]
		public string[] QuestObjectiveExclusions = new string[0];

		public bool IsMemberOnly
		{
			get
			{
				return isMemberOnly;
			}
		}

		public SpriteContentKey GetIconContentKey()
		{
			if (IconContentKey != null && !string.IsNullOrEmpty(IconContentKey.Key))
			{
				return IconContentKey;
			}
			return iconFallback;
		}

		public string GetNameOnServer()
		{
			string text = NameOnServer;
			if (string.IsNullOrEmpty(text))
			{
				text = Service.Get<Localizer>().GetTokenTranslation(Name);
			}
			return text;
		}

		public void OnValidate()
		{
			HashSet<string> hashSet = new HashSet<string>();
			int num = Tags.Length;
			for (int i = 0; i < num; i++)
			{
				if (Tags[i] == null)
				{
					Log.LogError(this, string.Format("O_o\t Found NULL tag in element {0} of Prop {1}, Id {2}", i, Name, Id));
					continue;
				}
				string tag = Tags[i].Tag;
				if (hashSet.Contains(tag))
				{
					if (Tags[i].name != tag)
					{
						Log.LogError(this, string.Format("O_o\t Found bad tag in element {0} of Prop {1}, Id {2}. It's name field '{3}' doesn't match it's tag field '{4}'", i, Name, Id, Tags[i].name, Tags[i].Tag));
					}
					else
					{
						Log.LogError(this, string.Format("O_o\t Found duplicate tag '{0}' in element {1} of Prop {2}, Id {3}", tag, i, Name, Id));
					}
				}
				else
				{
					hashSet.Add(tag);
				}
			}
		}

		public bool isDefinition(string name)
		{
			if (name == NameOnServer)
			{
				return true;
			}
			return name == Service.Get<Localizer>().GetTokenTranslation(Name);
		}
	}
}
