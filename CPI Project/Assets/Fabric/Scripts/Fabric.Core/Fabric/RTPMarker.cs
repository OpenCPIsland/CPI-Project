using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class RTPMarker
	{
		[SerializeField]
		public float _value;

		[SerializeField]
		public string _label;

		[SerializeField]
		public bool _keyOffEnabled;

		[NonSerialized]
		public bool _keyOff;

		public RTPMarker(string label, float value)
		{
			_label = label;
			_value = value;
		}
	}
}
