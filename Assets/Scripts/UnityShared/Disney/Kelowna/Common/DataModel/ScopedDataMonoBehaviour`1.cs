using System;

namespace Disney.Kelowna.Common.DataModel
{
	[Serializable]
	public class ScopedDataMonoBehaviour<T> : BaseDataMonoBehaviour<T> where T : ScopedData
	{
	}
}
