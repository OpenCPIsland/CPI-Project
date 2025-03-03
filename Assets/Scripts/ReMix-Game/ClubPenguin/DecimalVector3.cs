using System;
using UnityEngine;

namespace ClubPenguin
{
	public class DecimalVector3
	{
		public readonly decimal x;

		public readonly decimal y;

		public readonly decimal z;

		public DecimalVector3(Vector3 vector)
		{
			if (!ExportUtils.TryConvert(vector.x, out x) || !ExportUtils.TryConvert(vector.y, out y) || !ExportUtils.TryConvert(vector.z, out z))
			{
				throw new InvalidOperationException("Couldn't convert Vector3 to DecimalVector3");
			}
		}
	}
}
