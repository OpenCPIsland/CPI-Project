using System;
using UnityEngine;

namespace ClubPenguin
{
	public class InWorldExportPosition
	{
		public readonly decimal x;

		public readonly decimal y;

		public readonly decimal z;

		public InWorldExportPosition(Vector3 vector)
		{
			if (!InWorldExportValue.TryConvert(vector.x, out x) || !InWorldExportValue.TryConvert(vector.y, out y) || !InWorldExportValue.TryConvert(vector.z, out z))
			{
				throw new InvalidOperationException("Couldn't convert Vector3 to InWorldExportPosition");
			}
		}
	}
}
