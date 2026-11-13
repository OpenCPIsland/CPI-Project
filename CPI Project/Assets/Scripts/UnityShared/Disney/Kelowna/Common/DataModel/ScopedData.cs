using System;
using UnityEngine;

namespace Disney.Kelowna.Common.DataModel
{
	[Serializable]
	public abstract class ScopedData : BaseData
	{
		[SerializeField]
		private string _scopeID;

		protected abstract string scopeID
		{
			get;
		}

		public string ScopeID
		{
			get
			{
				if (string.IsNullOrEmpty(_scopeID))
				{
					_scopeID = scopeID;
				}
				return _scopeID;
			}
		}
	}
}
