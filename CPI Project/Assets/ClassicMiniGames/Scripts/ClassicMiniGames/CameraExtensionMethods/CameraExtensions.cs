using NUnit.Framework;
using UnityEngine;

namespace CameraExtensionMethods
{
	public static class CameraExtensions
	{
		public static float LeftEdgeInWorld(this Camera _cam)
		{
			Assert.IsTrue(_cam.orthographic, "This method only makes sense on ortho camera");
			return _cam.ViewportToWorldPoint(Vector3.zero).x;
		}

		public static float RightEdgeInWorld(this Camera _cam)
		{
			Assert.IsTrue(_cam.orthographic, "This method only makes sense on ortho camera");
			return _cam.ViewportToWorldPoint(Vector3.right).x;
		}

		public static float BottomEdgeInWorld(this Camera _cam)
		{
			Assert.IsTrue(_cam.orthographic, "This method only makes sense on ortho camera");
			return _cam.ViewportToWorldPoint(Vector3.zero).y;
		}

		public static float TopEdgeInWorld(this Camera _cam)
		{
			Assert.IsTrue(_cam.orthographic, "This method only makes sense on ortho camera");
			return _cam.ViewportToWorldPoint(Vector3.up).y;
		}
	}
}
