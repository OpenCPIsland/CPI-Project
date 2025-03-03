using System;
using UnityEngine;

namespace Fabric
{
	public class ActiveEvent
	{
		[NonSerialized]
		public string _eventName;

		[NonSerialized]
		public GameObject _parentGameObject;

		[NonSerialized]
		public Component _component;

		[NonSerialized]
		public bool _componentInstanceFoldout = true;

		[NonSerialized]
		public bool _virtualEventsFoldout;

		[NonSerialized]
		public float _time;
	}
}
