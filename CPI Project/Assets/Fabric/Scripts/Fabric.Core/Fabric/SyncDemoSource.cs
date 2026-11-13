using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[RequireComponent(typeof(AudioComponent))]
	public class SyncDemoSource : MonoBehaviour
	{
		private Serialization.CachedFieldScan _fieldScan;

		private AudioComponent _target;

		private Dictionary<string, Serialization.IField> _targetFields;

		private float _lastCheckForChangesTime;

		public void Start()
		{
			_fieldScan = new Serialization.CachedFieldScan(GetComponent<AudioComponent>());
			GameObject gameObject = new GameObject("target", typeof(AudioComponent));
			_target = gameObject.GetComponent<AudioComponent>();
			_targetFields = new Dictionary<string, Serialization.IField>();
			ScanTarget();
			_lastCheckForChangesTime = Time.time;
		}

		private void ScanTarget()
		{
			_targetFields.Clear();
			foreach (Serialization.IField item in Serialization.EnumerateFields(_target))
			{
				_targetFields[item.FieldName] = item;
			}
		}

		public void Update()
		{
			if (Time.time - _lastCheckForChangesTime > 1f)
			{
				_lastCheckForChangesTime = Time.time;
				foreach (KeyValuePair<string, object> item in _fieldScan.Update())
				{
					_targetFields[item.Key].SetValue(item.Value);
					if (item.Key.EndsWith("#"))
					{
						ScanTarget();
					}
				}
			}
		}
	}
}
