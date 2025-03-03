using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Graphic))]
	public class LocalPlayerMembershipTintSelector : AbstractLocalPlayerMembershipControl
	{
		public Color MemberColor;

		private Graphic targetGraphic;

		private Color nonMemberColor;

		private void Awake()
		{
			targetGraphic = GetComponent<Graphic>();
			nonMemberColor = targetGraphic.color;
		}

		protected override void membershipSet(bool isMember)
		{
			if (isMember)
			{
				targetGraphic.color = MemberColor;
			}
			else
			{
				targetGraphic.color = nonMemberColor;
			}
		}
	}
}
