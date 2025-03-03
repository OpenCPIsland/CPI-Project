using System;
using UnityEngine;

namespace UnityTest
{
	public class IsRenderedByCamera : ComparerBaseGeneric<Renderer, Camera>
	{
		public enum CompareType
		{
			IsVisible,
			IsNotVisible
		}

		public CompareType compareType;

		protected override bool Compare(Renderer renderer, Camera camera)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			bool flag = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
			switch (compareType)
			{
			case CompareType.IsVisible:
				return flag;
			case CompareType.IsNotVisible:
				return !flag;
			default:
				throw new Exception();
			}
		}
	}
}
