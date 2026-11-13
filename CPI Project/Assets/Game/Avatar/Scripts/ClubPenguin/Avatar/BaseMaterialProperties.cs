using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Avatar
{
	public abstract class BaseMaterialProperties
	{
		public abstract void Apply(Material mat);

		public abstract List<Object> InternalReferences();
	}
}
