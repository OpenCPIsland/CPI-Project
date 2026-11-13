using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class CustomCurves
	{
		[SerializeField]
		public string _name;

		[SerializeField]
		public float _minDistance = 1f;

		[SerializeField]
		public float _maxDistance = 500f;

		[SerializeField]
		public bool _enableCustomRolloff;

		[SerializeField]
		public AnimationCurve _customRolloff = new AnimationCurve(default(Keyframe));

		[SerializeField]
		public bool _enableSpatialBlend;

		[SerializeField]
		public AnimationCurve _spatialBlend = new AnimationCurve(default(Keyframe));

		[SerializeField]
		public bool _enableReverbZoneMix;

		[SerializeField]
		public AnimationCurve _reverbZoneMix = new AnimationCurve(default(Keyframe));

		[SerializeField]
		public bool _enableSpread;

		[SerializeField]
		public AnimationCurve _spread = new AnimationCurve(default(Keyframe));
	}
}
