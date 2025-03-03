using Fabric.TimelineComponent;
using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class RTPParameterToProperty
	{
		[SerializeField]
		public RTPParameter _parameter;

		[SerializeField]
		public RTPProperty _property;

		[NonSerialized]
		public Vector3 _previousPosition = default(Vector3);

		[SerializeField]
		public VolumeMeter _volumeMeter;

		[SerializeField]
		public string _globalParameterName;

		[SerializeField]
		public RTPModulator _rtpModulator;

		[SerializeField]
		public RTPParameterType _type;

		[SerializeField]
		public RTPPropertyType _propertyType;

		[SerializeField]
		public Fabric.TimelineComponent.Envelope _envelope = new Fabric.TimelineComponent.Envelope();

		public static float SetValueByType(float in1, float in2, RTPPropertyType type = RTPPropertyType.Set)
		{
			switch (type)
			{
			case RTPPropertyType.Multiply:
				return in1 * in2;
			case RTPPropertyType.Divide:
				return in1 / in2;
			case RTPPropertyType.Add:
				return in1 + in2;
			case RTPPropertyType.Subtract:
				return in1 - in2;
			default:
				return in2;
			}
		}
	}
}
