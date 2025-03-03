using System;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Mixing/SideChain")]
	public class SideChain : MonoBehaviour
	{
		private Component _component;

		protected InterpolatedParameter _envelope = new InterpolatedParameter(1f);

		[HideInInspector]
		[SerializeField]
		public VolumeMeter _volumeMeter;

		[HideInInspector]
		[SerializeField]
		public Component _componentToListen;

		[HideInInspector]
		[SerializeField]
		public bool _useComponentIsPlayingFlag;

		[NonSerialized]
		[HideInInspector]
		public float _sideChainGain = 1f;

		[HideInInspector]
		[SerializeField]
		public float gain = 1f;

		[HideInInspector]
		[SerializeField]
		public float fadeUpRate = 0.25f;

		[SerializeField]
		[HideInInspector]
		public float fadeDownRate = 0.25f;

		[NonSerialized]
		[HideInInspector]
		public CodeProfiler profiler = new CodeProfiler();

		private void Start()
		{
			_component = base.gameObject.GetComponent<Component>();
		}

		private void Update()
		{
			profiler.Begin();
			if (_useComponentIsPlayingFlag)
			{
				if (_componentToListen != null && _componentToListen.IsComponentActive())
				{
					_sideChainGain = gain;
				}
				else
				{
					_sideChainGain = 1f;
				}
			}
			else
			{
				if (!(_volumeMeter != null))
				{
					_sideChainGain = 1f;
					profiler.End();
					return;
				}
				float mRMS = _volumeMeter.volumeMeterState.mRMS;
				float db = AudioTools.LinearToDB(mRMS);
				_sideChainGain = AudioTools.DBToNormalizedDB(db);
			}
			float currentTimeMS = FabricTimer.Get();
			if (_sideChainGain > _envelope.GetCurrentValue())
			{
				_envelope.SetTarget(currentTimeMS, _sideChainGain, fadeUpRate, 0.5f);
			}
			else
			{
				_envelope.SetTarget(currentTimeMS, _sideChainGain, fadeDownRate, 0.5f);
			}
			if (_useComponentIsPlayingFlag)
			{
				_sideChainGain = _envelope.Get(currentTimeMS);
			}
			else
			{
				_sideChainGain = 1f - _envelope.Get(currentTimeMS) * gain;
			}
			profiler.End();
		}
	}
}
