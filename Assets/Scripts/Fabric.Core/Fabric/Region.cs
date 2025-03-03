using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class Region
	{
		[SerializeField]
		public string name = "";

		[SerializeField]
		public float offsetTime;

		[SerializeField]
		public float lengthTime;
	}
}
