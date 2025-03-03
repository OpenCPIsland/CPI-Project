using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	internal class SavedOutfitService : BaseNetworkService, ISavedOutfitService, INetworkService
	{
		protected override void setupListeners()
		{
		}

		public void SaveSavedOutfit(SavedOutfit savedOutfit)
		{
			APICall<SaveSavedOutfitOperation> aPICall = clubPenguinClient.SavedOutfitApi.SaveSavedOutfit(savedOutfit);
			aPICall.OnResponse += savedOutfitUpdated;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void GetSavedOutfits()
		{
			APICall<GetSavedOutfitsOperation> savedOutfits = clubPenguinClient.SavedOutfitApi.GetSavedOutfits();
			savedOutfits.OnResponse += savedOutfitsLoaded;
			savedOutfits.OnError += handleCPResponseError;
			savedOutfits.Execute();
		}

		public void DeleteSavedOutfit(int outfitSlot)
		{
			APICall<DeleteSavedOutfitOperation> aPICall = clubPenguinClient.SavedOutfitApi.DeleteSavedOutfit(outfitSlot);
			aPICall.OnResponse += savedOutfitDeleted;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void savedOutfitUpdated(SaveSavedOutfitOperation operation, HttpResponse httpResponse)
		{
			SavedOutfitResponse responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new SavedOutfitServiceEvents.SavedOutfitUpdated(responseBody.outfitSlot));
			handleCPResponse(responseBody);
		}

		private void savedOutfitsLoaded(GetSavedOutfitsOperation operation, HttpResponse httpResponse)
		{
			List<SavedOutfit> responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new SavedOutfitServiceEvents.SavedOutfitsLoaded(responseBody));
		}

		private void savedOutfitDeleted(DeleteSavedOutfitOperation operation, HttpResponse httpResponse)
		{
			SavedOutfitResponse responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new SavedOutfitServiceEvents.SavedOutfitDeleted(responseBody.outfitSlot));
		}
	}
}
