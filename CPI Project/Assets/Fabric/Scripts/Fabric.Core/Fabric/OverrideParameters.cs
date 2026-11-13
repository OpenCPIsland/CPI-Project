using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class OverrideParameters
	{
		[SerializeField]
		public bool _overrideIncomingEventAction;

		[SerializeField]
		public EventAction _incomingEventAction;

		[SerializeField]
		public EventAction _overrideEventAction;

		[SerializeField]
		public OverrideParameterType _type;

		[SerializeField]
		private float _floatParameter = 1f;

		[SerializeField]
		private string _stringParameter = "";

		[SerializeField]
		private SwitchPresetData _switchPresetData;

		[SerializeField]
		private DSPParameterData _dspParameterData;

		[SerializeField]
		private TransitionToSnapshotData _transitionToSnapshotData;

		public float FloatParameter
		{
			get
			{
				return _floatParameter;
			}
			set
			{
				Reset();
				_floatParameter = value;
				_type = OverrideParameterType.Float;
			}
		}

		public string StringParameter
		{
			get
			{
				return _stringParameter;
			}
			set
			{
				Reset();
				_stringParameter = value;
				_type = OverrideParameterType.String;
			}
		}

		public SwitchPresetData SwitchPresetData
		{
			get
			{
				return _switchPresetData;
			}
			set
			{
				Reset();
				_switchPresetData = value;
				_type = OverrideParameterType.SwitchPresetData;
			}
		}

		public DSPParameterData DSPParameterData
		{
			get
			{
				return _dspParameterData;
			}
			set
			{
				Reset();
				_dspParameterData = value;
				_type = OverrideParameterType.DSPParameterData;
			}
		}

		public TransitionToSnapshotData TransitionToSnapshotData
		{
			get
			{
				return _transitionToSnapshotData;
			}
			set
			{
				Reset();
				_transitionToSnapshotData = value;
				_type = OverrideParameterType.TransitionToSnapshotData;
			}
		}

		public void Reset()
		{
			_type = OverrideParameterType.Float;
			_floatParameter = 1f;
			_stringParameter = "";
			_switchPresetData = null;
			_dspParameterData = null;
			_transitionToSnapshotData = null;
		}
	}
}
