using System;
using UnityEngine;

namespace Fabric.TimelineComponent
{
	[Serializable]
	public class ParameterToProperty
	{
		[SerializeField]
		public TimelineParameter _parameter;

		[SerializeField]
		public Property _property;

		[SerializeField]
		public Envelope _envelope = new Envelope();
	}
}
