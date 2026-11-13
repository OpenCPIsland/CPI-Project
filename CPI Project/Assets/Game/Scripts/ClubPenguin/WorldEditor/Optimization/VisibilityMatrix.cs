using System;
using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public class VisibilityMatrix : VisibilityIterator
	{
		[Serializable]
		public struct Voxel
		{
			public bool Active;

			public uint Facings;
		}

		public Vector3 Granularity = Vector3.one;

		[Range(1f, 256f)]
		public int DimensionX = 10;

		[Range(1f, 256f)]
		public int DimensionY = 2;

		[Range(1f, 256f)]
		public int DimensionZ = 10;

		[Range(6f, 32f)]
		public int FacingCount = 6;

		public Voxel[] Voxels;

		public override Visibility Current
		{
			get
			{
				return default(Visibility);
			}
		}

		public void Awake()
		{
			base.hideFlags = (HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
		}

		public override bool MoveNext()
		{
			return false;
		}

		public override void Reset()
		{
		}
	}
}
