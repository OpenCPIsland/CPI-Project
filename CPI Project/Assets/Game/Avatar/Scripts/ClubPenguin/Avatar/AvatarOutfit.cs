using UnityEngine;

namespace ClubPenguin.Avatar
{
	[DisallowMultipleComponent]
	public class AvatarOutfit : MonoBehaviour
	{
		public Color BeakColor = new Color(1f, 0.78f, 0f);

		public Color BellyColor = Color.white;

		public Color BodyColor = new Color(0.09f, 0.1f, 0.84f);

		public DCustomOutfit Outfit;
	}
}
