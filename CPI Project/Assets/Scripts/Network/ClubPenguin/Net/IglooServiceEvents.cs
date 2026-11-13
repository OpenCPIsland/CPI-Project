using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using Disney.Kelowna.Common.DataModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class IglooServiceEvents
	{
		public struct IglooLayoutCreated
		{
			public SavedSceneLayout SavedSceneLayout;

			public IglooLayoutCreated(SavedSceneLayout savedSceneLayout)
			{
				SavedSceneLayout = savedSceneLayout;
			}
		}

		public struct IglooLayoutUpdated
		{
			public SavedSceneLayout SavedSceneLayout;

			public IglooLayoutUpdated(SavedSceneLayout savedSceneLayout)
			{
				SavedSceneLayout = savedSceneLayout;
			}
		}

		public struct IglooLayoutDeleted
		{
			public long LayoutId;

			public IglooLayoutDeleted(long layoutId)
			{
				LayoutId = layoutId;
			}
		}

		public struct IglooLayoutLoaded
		{
			public SavedSceneLayout SavedSceneLayout;

			public IglooLayoutLoaded(SavedSceneLayout savedSceneLayout)
			{
				SavedSceneLayout = savedSceneLayout;
			}
		}

		public struct IgloosFromZoneIdsLoaded
		{
			public readonly List<RoomPopulation> RoomPopulations;

			public readonly List<DataEntityHandle> Handles;

			public IgloosFromZoneIdsLoaded(List<RoomPopulation> roomPopulations, List<DataEntityHandle> handles)
			{
				RoomPopulations = roomPopulations;
				Handles = handles;
			}
		}

		public struct PopularIgloosListLoaded
		{
			public readonly List<IglooListItem> IglooListItems;

			public PopularIgloosListLoaded(List<IglooListItem> iglooListItems)
			{
				IglooListItems = iglooListItems;
			}
		}

		public struct FriendIgloosListLoaded
		{
			public readonly List<IglooListItem> IglooListItems;

			public FriendIgloosListLoaded(List<IglooListItem> iglooListItems)
			{
				IglooListItems = iglooListItems;
			}
		}

		public struct IglooDataLoaded
		{
			public SavedIglooLayoutsSummary IglooData;

			public IglooDataLoaded(SavedIglooLayoutsSummary iglooData)
			{
				IglooData = iglooData;
			}
		}

		public struct IglooDataUpdated
		{
			public SignedResponse<IglooData> SignedIglooData;

			public IglooDataUpdated(SignedResponse<IglooData> signedIglooData)
			{
				SignedIglooData = signedIglooData;
			}
		}

		public struct IglooPublished
		{
			public readonly SavedSceneLayout SavedSceneLayout;

			public IglooPublished(SavedSceneLayout savedSceneLayout)
			{
				SavedSceneLayout = savedSceneLayout;
			}
		}

		public struct IglooUpdated
		{
			public string IglooId;

			public IglooUpdated(string iglooId)
			{
				IglooId = iglooId;
			}
		}

		public struct ForceLeave
		{
			public ZoneId ZoneId;

			public ForceLeave(ZoneId zoneId)
			{
				ZoneId = zoneId;
			}
		}

		public struct DecorationCreated
		{
			public readonly DecorationId DecorationId;

			public DecorationCreated(DecorationId decorationId)
			{
				DecorationId = decorationId;
			}
		}

		public struct DecorationUpdated
		{
			public readonly DecorationId DecorationId;

			public readonly int Count;

			public DecorationUpdated(DecorationId decorationId, int count)
			{
				DecorationId = decorationId;
				Count = count;
			}
		}

		public struct DecorationsLoaded
		{
			public readonly DecorationInventory DecorationInventory;

			public DecorationsLoaded(DecorationInventory decorationInventory)
			{
				DecorationInventory = decorationInventory;
			}
		}

		public struct DecorationDeleted
		{
			public readonly DecorationId DecorationId;

			public DecorationDeleted(DecorationId decorationId)
			{
				DecorationId = decorationId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DecorationPurchaseFailed
		{
		}

		public struct IglooActiveLayoutLoaded
		{
			public readonly SceneLayout Layout;

			public IglooActiveLayoutLoaded(SceneLayout layout)
			{
				Layout = layout;
			}
		}
	}
}
