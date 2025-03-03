using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class CustomCurvesManager
	{
		[SerializeField]
		public List<CustomCurves> _customCurveList = new List<CustomCurves>();

		public void CreateCustomCurves(string name)
		{
			CustomCurves customCurves = new CustomCurves();
			customCurves._name = name;
			_customCurveList.Add(customCurves);
		}

		public string[] GetNames()
		{
			if (_customCurveList.Count == 0)
			{
				return new string[1]
				{
					"None"
				};
			}
			string[] array = new string[_customCurveList.Count];
			for (int i = 0; i < _customCurveList.Count; i++)
			{
				array[i] = _customCurveList[i]._name;
			}
			return array;
		}

		public string GetCurveNameByIndex(int index)
		{
			if (index < _customCurveList.Count)
			{
				return _customCurveList[index]._name;
			}
			return null;
		}

		public int GetCurveIndexByName(string name)
		{
			for (int i = 0; i < _customCurveList.Count; i++)
			{
				if (name == _customCurveList[i]._name)
				{
					return i;
				}
			}
			return 0;
		}

		public CustomCurves GetCustomCurvesByName(string name)
		{
			for (int i = 0; i < _customCurveList.Count; i++)
			{
				if (name == _customCurveList[i]._name)
				{
					return _customCurveList[i];
				}
			}
			return null;
		}

		public CustomCurves GetCustomCurvesByIndex(int index)
		{
			if (index < _customCurveList.Count)
			{
				return _customCurveList[index];
			}
			return null;
		}
	}
}
