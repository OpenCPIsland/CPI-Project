using UnityEngine;

namespace ClubPenguin.DCE
{
	public abstract class BasePartDefinition : ScriptableObject
	{
		public abstract void ApplyToViewPart(ViewPart partView);
	}
}
