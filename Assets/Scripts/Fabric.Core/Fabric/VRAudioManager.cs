using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class VRAudioManager
	{
		[Serializable]
		public class VRSolution
		{
			[SerializeField]
			public GameObject _audioSourcePrefab;

			[SerializeField]
			public GameObject _audioListenerPrefab;
		}

		[SerializeField]
		public VRPlatformDictionary _vrPlatforms = new VRPlatformDictionary();

		[SerializeField]
		public List<VRSolution> _vrSolutions = new List<VRSolution>();

		[SerializeField]
		public int _defaultVRSolution;

		public bool HasVRSolutions()
		{
			if (_vrSolutions.Count <= 0)
			{
				return false;
			}
			return true;
		}

		public void Initialise()
		{
		}

		private VRSolution GetCurrentVRSolution()
		{
			if (_vrPlatforms.ContainsKey(Application.platform))
			{
				VRSolution vRSolution = _vrPlatforms[Application.platform];
				if (vRSolution != null && vRSolution._audioSourcePrefab != null)
				{
					return vRSolution;
				}
			}
			if (_defaultVRSolution < _vrSolutions.Count)
			{
				return _vrSolutions[_defaultVRSolution];
			}
			return null;
		}

		public GameObject GetAudioSource(bool instantiate = true)
		{
			VRSolution currentVRSolution = GetCurrentVRSolution();
			if (currentVRSolution != null && currentVRSolution._audioSourcePrefab != null)
			{
				if (instantiate)
				{
					return UnityEngine.Object.Instantiate(currentVRSolution._audioSourcePrefab);
				}
				return currentVRSolution._audioSourcePrefab;
			}
			return null;
		}

		public GameObject GetAudioListener(bool instantiate = true)
		{
			VRSolution currentVRSolution = GetCurrentVRSolution();
			if (currentVRSolution != null && currentVRSolution._audioListenerPrefab != null)
			{
				if (instantiate)
				{
					return UnityEngine.Object.Instantiate(currentVRSolution._audioListenerPrefab);
				}
				return currentVRSolution._audioListenerPrefab;
			}
			return null;
		}
	}
}
