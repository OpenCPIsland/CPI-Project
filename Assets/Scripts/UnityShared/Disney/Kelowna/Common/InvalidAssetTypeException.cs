using System;

namespace Disney.Kelowna.Common
{
	public class InvalidAssetTypeException : Exception
	{
		public InvalidAssetTypeException(Type expectedBase, Type asset)
			: base(asset.FullName + " cannot be assigned to " + expectedBase.FullName)
		{
		}
	}
}
