using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Tags;
using Disney.MobileNetwork;

namespace ClubPenguin.Switches
{
	public class EquippedOutfitSwitch : Switch
	{
		public OutfitTagMatcher TagMatcher;

		private CPDataEntityCollection dataEntityCollection;

		private AvatarDetailsData avatarDetailsData;

		public void Start()
		{
			if (dataEntityCollection == null)
			{
				dataEntityCollection = Service.Get<CPDataEntityCollection>();
			}
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out avatarDetailsData))
			{
				onOutfitChanged(avatarDetailsData.Outfit);
				avatarDetailsData.PlayerOutfitChanged += onOutfitChanged;
			}
		}

		private void onOutfitChanged(DCustomEquipment[] outfit)
		{
			Change(TagMatcher.isMatch(outfit));
		}

		public void OnDestroy()
		{
			if (avatarDetailsData != null)
			{
				avatarDetailsData.PlayerOutfitChanged -= onOutfitChanged;
			}
		}

		public override object GetSwitchParameters()
		{
			return TagMatcher.GetExportParameters();
		}

		public override string GetSwitchType()
		{
			return "equippedOutfit";
		}
	}
}
