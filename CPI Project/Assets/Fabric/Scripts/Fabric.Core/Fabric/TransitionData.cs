using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class TransitionData
	{
		[SerializeField]
		public string _startEvent = "";

		[SerializeField]
		public double _scheduleStart;

		[SerializeField]
		public string _stopEvent = "";

		[SerializeField]
		public double _scheduleStop;
	}
}
