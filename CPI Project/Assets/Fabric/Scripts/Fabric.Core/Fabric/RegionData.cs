using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class RegionData
	{
		[SerializeField]
		public string _regionName = "";

		[SerializeField]
		public int _endOffset;

		[SerializeField]
		public string _endMarker;
	}
}
