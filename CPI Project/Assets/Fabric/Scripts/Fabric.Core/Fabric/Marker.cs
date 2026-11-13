using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class Marker
	{
		[SerializeField]
		public string name = "";

		[SerializeField]
		public int offsetSamples;

		[SerializeField]
		public float offsetTime;

		[SerializeField]
		public MarkerType type = MarkerType.Notify;

		[NonSerialized]
		public float frequency;
	}
}
