using System;

namespace DeviceDB
{
	internal interface IFixedSizeIndexValueType<TEntryValue> : IIndexValueType<TEntryValue>, IDisposable where TEntryValue : IComparable<TEntryValue>
	{
		uint Size
		{
			get;
		}
	}
}
