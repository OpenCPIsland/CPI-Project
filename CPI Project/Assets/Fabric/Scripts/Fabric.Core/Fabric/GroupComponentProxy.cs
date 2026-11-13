using System;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("")]
	public class GroupComponentProxy : MonoBehaviour
	{
		[NonSerialized]
		public GroupComponent _groupComponent;

		[NonSerialized]
		public int _registeredWithMainRefCount;
	}
}
