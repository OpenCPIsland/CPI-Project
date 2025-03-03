using System;
using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/Components/SamplePlayerComponent")]
	public class SamplePlayerComponent : AudioComponent
	{
		[NonSerialized]
		[HideInInspector]
		public SampleFileInstance sampleFileInstance;

		[SerializeField]
		[HideInInspector]
		public string sampleFileName = "";

		[SerializeField]
		[HideInInspector]
		public float[] _channelGains = new float[8]
		{
			1f,
			1f,
			1f,
			1f,
			1f,
			1f,
			1f,
			1f
		};

		[SerializeField]
		[HideInInspector]
		public int _leftLoopMarker;

		[SerializeField]
		public int _rightLoopMarker;

		private int _prevIndex = -1;

		protected override void OnInitialise(bool inPreviewMode = false)
		{
			AudioSource component = base.gameObject.GetComponent<AudioSource>();
			if ((bool)component)
			{
				Debug.LogWarning("Fabric: Adding an AudioSource and AudioComponent [" + base.name + "] in the same gameObject will impact performance, move AudioSource in a new gameObject underneath");
			}
			int sampleFileIndexByName = SampleManager.Instance.GetSampleFileIndexByName(sampleFileName);
			if (sampleFileIndexByName >= 0 && (sampleFileIndexByName != _prevIndex || sampleFileInstance == null))
			{
				SampleFile sampleFileByIndex = SampleManager.Instance.GetSampleFileByIndex(sampleFileIndexByName);
				if (sampleFileByIndex != null)
				{
					sampleFileInstance = sampleFileByIndex.GetInstance();
					if (sampleFileInstance != null)
					{
						sampleFileInstance._start = _leftLoopMarker;
						sampleFileInstance._end = _rightLoopMarker;
						base.AudioClip = AudioClip.Create(sampleFileName + " (Custom)", _rightLoopMarker, sampleFileByIndex._channels, (int)sampleFileByIndex._sampleRate, true, OnAudioRead);
						sampleFileInstance._loop = base.Loop;
					}
				}
				_prevIndex = sampleFileIndexByName;
			}
			base.OnInitialise(inPreviewMode);
		}

		public void SetLeftLoopMarker(int offset)
		{
			if (sampleFileInstance != null && sampleFileInstance._sampleFile != null)
			{
				offset -= offset % sampleFileInstance._sampleFile._channels;
				sampleFileInstance._start = offset;
			}
			_leftLoopMarker = offset;
			if (_leftLoopMarker > _rightLoopMarker)
			{
				_leftLoopMarker = _rightLoopMarker;
			}
		}

		public void SetRightLoopMarker(int offset)
		{
			if (sampleFileInstance != null && sampleFileInstance._sampleFile != null)
			{
				offset -= offset % sampleFileInstance._sampleFile._channels;
				sampleFileInstance._end = offset;
			}
			_rightLoopMarker = offset;
			if (_rightLoopMarker < _leftLoopMarker)
			{
				_rightLoopMarker = _leftLoopMarker;
			}
		}

		public override void PlayInternal(ComponentInstance zComponentInstance, float target, float curve, bool dontPlayComponents)
		{
			if (sampleFileInstance != null)
			{
				sampleFileInstance._position = 0;
				base.PlayInternal(zComponentInstance, target, curve, dontPlayComponents);
			}
		}

		public void NotifySampleFileRemoved(SampleFile sampleFile)
		{
			if (sampleFileInstance != null && sampleFileInstance._sampleFile == sampleFile)
			{
				sampleFileInstance = null;
				sampleFileName = "";
			}
		}

		private void OnAudioRead(float[] data)
		{
			if (sampleFileInstance != null)
			{
				sampleFileInstance.FillAudioBuffer(data, sampleFileInstance._sampleFile._channels, _channelGains);
			}
		}
	}
}
