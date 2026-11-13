using UnityEngine;

namespace ClubPenguin.Avatar
{
	[RequireComponent(typeof(AvatarOutfit))]
	public class AvatarControllerStaticOutfit : AvatarController
	{
		public void Start()
		{
			AvatarOutfit component = GetComponent<AvatarOutfit>();
			Model.BeakColor = component.BeakColor;
			Model.BellyColor = component.BellyColor;
			Model.BodyColor = component.BodyColor;
			Model.ClearAllEquipment();
			Model.ApplyOutfit(component.Outfit);
		}
	}
}
