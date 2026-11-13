using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class ParameterData
	{
		[SerializeField]
		public float _value;

		[SerializeField]
		public int _parameter;

		[SerializeField]
		public int _index = -1;
	}
}
