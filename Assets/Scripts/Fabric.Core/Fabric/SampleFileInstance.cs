using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class SampleFileInstance
	{
		[SerializeField]
		public int _position;

		[NonSerialized]
		public SampleFile _sampleFile;

		public int _start;

		public int _end;

		public bool _loop;

		private int _channelGainIndex;

		public SampleFileInstance(SampleFile sampleFile)
		{
			_sampleFile = sampleFile;
		}

		public string Name()
		{
			if (_sampleFile != null)
			{
				return _sampleFile.Name();
			}
			return "Invalid SampleFile!!";
		}

		public void Reset(int start = 0)
		{
			_position = start;
			_channelGainIndex = 0;
		}

		public void FillAudioBuffer(float[] buffer, int channels, float[] channelGains)
		{
			if (_sampleFile._data == null || _sampleFile._data.Length <= buffer.Length)
			{
				return;
			}
			for (int i = 0; i < buffer.Length; i++)
			{
				if (_position >= _end)
				{
					if (!_loop)
					{
						break;
					}
					Reset(_start);
				}
				if (_channelGainIndex >= channels)
				{
					_channelGainIndex = 0;
				}
				buffer[i] = _sampleFile._data[_position] * channelGains[_channelGainIndex];
				_position++;
				_channelGainIndex++;
			}
		}
	}
}
