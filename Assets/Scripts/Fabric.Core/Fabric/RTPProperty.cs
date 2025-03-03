using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class RTPProperty
	{
		[SerializeField]
		public int _property;

		[SerializeField]
		public string _name = "Volume";

		[SerializeField]
		public float _min;

		[SerializeField]
		public float _max = 1f;

		[NonSerialized]
		public string _propertyName = "";

		[NonSerialized]
		public string _componentName = "";

		[NonSerialized]
		public float _value;

		public RTPProperty(int property, string name, float min, float max)
		{
			_property = property;
			_name = name;
			_min = min;
			_max = max;
		}

		public RTPProperty()
		{
		}
	}
}
