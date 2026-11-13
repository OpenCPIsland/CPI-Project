using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class SampleManager
	{
		private static SampleManager _instance;

		[SerializeField]
		private List<SampleFile> _sampleFileList = new List<SampleFile>();

		public static SampleManager Instance
		{
			get
			{
				return _instance;
			}
		}

		public SampleManager()
		{
			_instance = this;
		}

		public void Destroy()
		{
			for (int i = 0; i < _sampleFileList.Count; i++)
			{
				_sampleFileList[i].Destroy();
			}
		}

		public void AddSampleFile(SampleFile sampleFile)
		{
			_sampleFileList.Add(sampleFile);
		}

		public void RemoveSampleFile(SampleFile sampleFile)
		{
			_sampleFileList.Remove(sampleFile);
		}

		public string[] ToStringArray()
		{
			string[] array = new string[_sampleFileList.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = _sampleFileList[i].Name();
			}
			return array;
		}

		public int GetSampleFileIndexByName(string name)
		{
			for (int i = 0; i < _sampleFileList.Count; i++)
			{
				if (_sampleFileList[i].Name() == name)
				{
					return i;
				}
			}
			return -1;
		}

		public int GetNumSampleFiles()
		{
			return _sampleFileList.Count;
		}

		public SampleFile GetSampleFileByIndex(int index)
		{
			return _sampleFileList[index];
		}
	}
}
