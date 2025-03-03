using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class SwitchPresetData
	{
		[SerializeField]
		public string _sourcePreset = "";

		[SerializeField]
		public string _targetPreset = "";
	}
}
