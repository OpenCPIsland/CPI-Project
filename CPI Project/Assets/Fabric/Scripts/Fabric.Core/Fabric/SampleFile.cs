using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class SampleFile
	{
		[SerializeField]
		private List<SampleFileInstance> _sampleFileInstanceList = new List<SampleFileInstance>();

		[SerializeField]
		public string _name = "";

		[SerializeField]
		public string _audioClipPath = "";

		[SerializeField]
		public List<Marker> _markers = new List<Marker>();

		[SerializeField]
		public float _sampleRate;

		[SerializeField]
		public int _channels;

		[SerializeField]
		public int _samples;

		[SerializeField]
		public bool _threeD;

		public float[] _data;

		private int _refCount;

		private void InitData()
		{
			if (_audioClipPath != null)
			{
				AudioClip audioClip = Resources.Load(_audioClipPath) as AudioClip;
				if (audioClip != null)
				{
					int num = audioClip.samples * audioClip.channels;
					_data = new float[num];
					audioClip.GetData(_data, 0);
				}
				else
				{
					DebugLog.Print(DebugLevel.Error, "Sample File [", _name, "] Not loaded");
				}
			}
		}

		private void IncRef()
		{
			_refCount++;
		}

		private void DecRef()
		{
			_refCount--;
		}

		public void Destroy()
		{
			DecRef();
			if (_refCount == 0)
			{
				_data = null;
			}
		}

		public string Name()
		{
			return _name;
		}

		public SampleFileInstance GetInstance()
		{
			if (_refCount == 0)
			{
				InitData();
			}
			IncRef();
			return new SampleFileInstance(this);
		}
	}
}
