using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.Net.Domain.Decoration;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using Tweaker.Core;

namespace ClubPenguin.Igloo
{
	public class RecentDecorationsService
	{
		private const string KEY = "RecentDecorationsServiceList";

		public int MaxCountRecentDecorations = 16;

		public int SelectedCategoryIndex = -1;

		public List<int> DefinitionsIDsList;

		private EventDispatcher eventDispatcher;

		public bool ShouldShowMostRecentPurchase;

		private int mostRecentPurchaseId;

		private DecorationType mostRecentPurchaseType;

		public int MostRecentPurchaseId
		{
			get
			{
				return mostRecentPurchaseId;
			}
		}

		public DecorationType MostRecentPurchaseType
		{
			get
			{
				return mostRecentPurchaseType;
			}
		}

		[Invokable("Igloo.ClearRecentlyUsed")]
		public static void ClearRecent()
		{
			DisplayNamePlayerPrefs.DeleteKey("RecentDecorationsServiceList");
			Service.Get<RecentDecorationsService>().DefinitionsIDsList.Clear();
		}

		public RecentDecorationsService()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventDispatcher.AddListener<IglooUIEvents.AddNewDecoration>(onNewDecorationAdded);
			eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerDataReadyEvent>(onLocalPlayerDataReadyEvent);
		}

		private bool onLocalPlayerDataReadyEvent(NetworkControllerEvents.LocalPlayerDataReadyEvent evt)
		{
			Initialize();
			return false;
		}

		private void Initialize()
		{
			if (DisplayNamePlayerPrefs.HasKey("RecentDecorationsServiceList"))
			{
				List<string> list = DisplayNamePlayerPrefs.GetList<string>("RecentDecorationsServiceList");
				try
				{
					DefinitionsIDsList = list.ConvertAll((string s) => int.Parse(s));
				}
				catch (Exception)
				{
					DefinitionsIDsList = new List<int>();
				}
			}
			else
			{
				DefinitionsIDsList = new List<int>();
			}
			EnforceMaxListSize();
			EnforceMemberOnlyItems();
		}

		private void EnforceMemberOnlyItems()
		{
			bool flag = Service.Get<CPDataEntityCollection>().IsLocalPlayerMember();
			Dictionary<int, DecorationDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, DecorationDefinition>>();
			if (dictionary != null)
			{
				List<int> list = new List<int>(DefinitionsIDsList.Count);
				foreach (int definitionsIDs in DefinitionsIDsList)
				{
					if (dictionary.ContainsKey(definitionsIDs))
					{
						DecorationDefinition decorationDefinition = dictionary[definitionsIDs];
						if (decorationDefinition.IsMemberOnly)
						{
							if (flag)
							{
								list.Add(definitionsIDs);
							}
						}
						else
						{
							list.Add(definitionsIDs);
						}
					}
				}
				DefinitionsIDsList = list;
			}
		}

		private bool onNewDecorationAdded(IglooUIEvents.AddNewDecoration evt)
		{
			if (!DefinitionsIDsList.Contains(evt.Definition.Id))
			{
				EnforceMaxListSize();
				DefinitionsIDsList.Insert(0, evt.Definition.Id);
			}
			return false;
		}

		private void EnforceMaxListSize()
		{
			while (DefinitionsIDsList.Count >= MaxCountRecentDecorations)
			{
				DefinitionsIDsList.RemoveAt(DefinitionsIDsList.Count - 1);
			}
		}

		public void FlushPlayerPrefs()
		{
			DisplayNamePlayerPrefs.SetList("RecentDecorationsServiceList", DefinitionsIDsList.ConvertAll((int i) => i.ToString()));
		}

		public void SetRecentPurchaseData(DecorationType recentPurchaseType, int recentPurchaseId)
		{
			mostRecentPurchaseType = recentPurchaseType;
			mostRecentPurchaseId = recentPurchaseId;
		}
	}
}
