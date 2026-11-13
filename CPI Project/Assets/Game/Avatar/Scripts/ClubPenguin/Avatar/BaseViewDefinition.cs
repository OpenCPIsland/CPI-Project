using Disney.Kelowna.Common;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	public abstract class BaseViewDefinition : ScriptableObject, ICacheableContent
	{
		public abstract void ApplyToViewPart(ViewPart partView);

		public abstract List<Object> InternalReferences();
	}
}
