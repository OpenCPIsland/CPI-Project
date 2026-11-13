using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class SaveOutfitToWearCMD
	{
		private long[] outfitToWear;

		private DCustomEquipment[] outfitToWearData;

		public SaveOutfitToWearCMD(long[] outfitToWear, DCustomEquipment[] outfitToWearData)
		{
			this.outfitToWear = outfitToWear;
			this.outfitToWearData = outfitToWearData;
		}

		public void Execute()
		{
			PlayerOutfit playerOutfit = new PlayerOutfit();
			playerOutfit.parts = outfitToWear;
			Service.Get<INetworkServicesManager>().PlayerStateService.SetOutfit(playerOutfit);
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			cPDataEntityCollection.GetComponent<AvatarDetailsData>(localPlayerHandle).Outfit = outfitToWearData;
		}
	}
}
