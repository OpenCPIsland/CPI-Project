using System;
using Tweaker.AssemblyScanner;

namespace Tweaker.UI.Testbed
{
	public class CustomSerializerProcessor : ITypeScanProcessor<CustomTypeSerializer, CustomSerializerResult>, IScanProcessor<CustomTypeSerializer, CustomSerializerResult>, IScanResultProvider<CustomSerializerResult>
	{
		public event EventHandler<ScanResultArgs<CustomSerializerResult>> ResultProvided;

		public void ProcessType(Type type, IBoundInstance instance = null)
		{
			if (this.ResultProvided != null)
			{
				this.ResultProvided(this, new ScanResultArgs<CustomSerializerResult>(new CustomSerializerResult(type)));
			}
		}
	}
}
