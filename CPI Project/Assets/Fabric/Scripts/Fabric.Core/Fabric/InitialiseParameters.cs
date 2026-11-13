using UnityEngine;

namespace Fabric
{
	public class InitialiseParameters
	{
		public bool _isMutliplier;

		public InitialiseParameter<float> _priority;

		public InitialiseParameter<float> _volume;

		public InitialiseParameter<float> _volumeRandomization;

		public InitialiseParameter<float> _fadeInTime;

		public InitialiseParameter<float> _fadeInCurve;

		public InitialiseParameter<float> _fadeOutTime;

		public InitialiseParameter<float> _fadeOutCurve;

		public InitialiseParameter<float> _pitch;

		public InitialiseParameter<float> _pitchRandomization;

		public InitialiseParameter<float> _panLevel;

		public InitialiseParameter<float> _pan2D;

		public InitialiseParameter<float> _spreadLevel;

		public InitialiseParameter<float> _dopplerLevel;

		public InitialiseParameter<float> _minDistance;

		public InitialiseParameter<float> _maxDistance;

		public InitialiseParameter<int> _delaySamples;

		public InitialiseParameter<AudioRolloffMode> _rolloffMode;
	}
}
