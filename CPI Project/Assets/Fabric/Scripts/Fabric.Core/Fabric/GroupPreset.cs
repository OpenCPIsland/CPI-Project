using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Mixing/DynamicMixer/GroupPreset")]
	public class GroupPreset : MonoBehaviour
	{
		[HideInInspector]
		[SerializeField]
		public GroupComponent _groupComponent;

		[HideInInspector]
		[SerializeField]
		private float _volume;

		[SerializeField]
		[HideInInspector]
		private float _pitch;

		[HideInInspector]
		[SerializeField]
		private float _fadeInTime;

		[HideInInspector]
		[SerializeField]
		private float _fadeInCurve = 0.5f;

		[SerializeField]
		[HideInInspector]
		private float _fadeOutTime;

		[HideInInspector]
		[SerializeField]
		private float _fadeOutCurve = 0.5f;

		private string _name = "";

		private float _gain = 1f;

		private InterpolatedParameter _volumeParameter = new InterpolatedParameter();

		private InterpolatedParameter _pitchParameter = new InterpolatedParameter();

		private InterpolatedParameter _activeParameter = new InterpolatedParameter();

		private bool _isActive;

		public GroupComponent GroupComponent
		{
			get
			{
				return _groupComponent;
			}
			set
			{
				_groupComponent = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public float Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				_volume = value;
			}
		}

		public float VolumeParameter
		{
			set
			{
				_volume = value;
				_volumeParameter.SetTarget(FabricTimer.Get(), _volume, 0f, 0f);
			}
		}

		public float PitchParameter
		{
			set
			{
				_pitch = value;
				_pitchParameter.SetTarget(FabricTimer.Get(), _pitch, 0f, 0f);
			}
		}

		public float Pitch
		{
			get
			{
				return _pitch;
			}
			set
			{
				_pitch = value;
			}
		}

		public float FadeInTime
		{
			get
			{
				return _fadeInTime;
			}
			set
			{
				_fadeInTime = value;
			}
		}

		public float FadeInCurve
		{
			get
			{
				return _fadeInCurve;
			}
			set
			{
				_fadeInCurve = value;
			}
		}

		public float FadeOutTime
		{
			get
			{
				return _fadeOutTime;
			}
			set
			{
				_fadeOutTime = value;
			}
		}

		public float FadeOutCurve
		{
			get
			{
				return _fadeOutCurve;
			}
			set
			{
				_fadeOutCurve = value;
			}
		}

		public float Gain
		{
			get
			{
				return _gain;
			}
			set
			{
				_gain = value;
			}
		}

		public void Init(GroupComponent groupComponent)
		{
			_activeParameter.Reset(0f);
			_volumeParameter.Reset(0f);
			_pitchParameter.Reset(0f);
			_groupComponent = groupComponent;
		}

		private void Start()
		{
			if (_groupComponent != null)
			{
				_name = _groupComponent.name;
			}
			_activeParameter.Reset(0f);
			_volumeParameter.Reset(0f);
			_pitchParameter.Reset(0f);
		}

		public float CalculateVolume()
		{
			float num = AudioTools.DBToLinear(_volumeParameter.Get(FabricTimer.Get()));
			float linear = 1f - (1f - _gain * num);
			return AudioTools.LinearToDB(linear);
		}

		public float CalculatePitch()
		{
			return 1f + _pitchParameter.Get(FabricTimer.Get());
		}

		public void Activate()
		{
			_activeParameter.SetTarget(FabricTimer.Get(), 1f, _fadeInTime, _fadeInCurve);
			_volumeParameter.SetTarget(FabricTimer.Get(), _volume, _fadeInTime, _fadeInCurve);
			_pitchParameter.SetTarget(FabricTimer.Get(), _pitch, _fadeInTime, _fadeInCurve);
			_isActive = true;
		}

		public void Deactivate()
		{
			_activeParameter.SetTarget(FabricTimer.Get(), 0f, _fadeOutTime, _fadeOutCurve);
			_volumeParameter.SetTarget(FabricTimer.Get(), 0f, _fadeOutTime, _fadeOutCurve);
			_pitchParameter.SetTarget(FabricTimer.Get(), 0f, _fadeOutTime, _fadeOutCurve);
			_isActive = false;
		}

		public bool IsActive()
		{
			if (!_isActive)
			{
				return !_activeParameter.HasReachedTarget(FabricTimer.Get());
			}
			return true;
		}

		public float Progress()
		{
			return _activeParameter.GetCurrentValue();
		}

		public void Reset()
		{
			_activeParameter.Reset(0f);
			_volumeParameter.Reset(0f);
			_pitchParameter.Reset(0f);
			_isActive = false;
		}

		public void SwitchFromPreset(GroupPreset sourcePreset)
		{
			_volumeParameter.Reset(sourcePreset._volume);
			_pitchParameter.Reset(sourcePreset._pitch);
			Activate();
		}
	}
}
