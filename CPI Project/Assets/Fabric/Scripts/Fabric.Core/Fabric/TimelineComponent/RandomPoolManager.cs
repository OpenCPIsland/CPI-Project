using System.Collections.Generic;
using UnityEngine;

namespace Fabric.TimelineComponent
{
	[AddComponentMenu("")]
	public class RandomPoolManager : Component
	{
		[HideInInspector]
		[SerializeField]
		public Dictionary<string, RandomComponent> _definitionsTable = new Dictionary<string, RandomComponent>();

		[HideInInspector]
		[SerializeField]
		public string projectName;
	}
}
