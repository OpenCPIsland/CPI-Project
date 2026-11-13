using System;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	[Serializable]
	public class CellPhoneLocationActivityDefinition : CellPhoneActivityDefinition
	{
		public SceneDefinition Scene;

		public Vector3 LocationInZone;

		public bool IsHiddenAfterAccessed;
	}
}
