using System;
using UnityEngine;

namespace Disney.Kelowna.Common.DataModel
{
	[Serializable]
	public abstract class BaseDataMonoBehaviour<T> : MonoBehaviour where T : BaseData
	{
		public T Data;

		public bool IsDestroyed
		{
			get;
			internal set;
		}
	}
}
