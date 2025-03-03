using UnityEngine;

namespace ClubPenguin.Core
{
	public class LayerConstants
	{
		public static readonly string InvisibleBarrierLayer = "InvisibleBarrier";

		public static readonly string DefaultLayer = "Default";

		public static readonly string TerrainBarrierLayer = "TerrainBarrier";

		public static readonly string NonPlayerLayer = "NonPlayer";

		public static readonly string WaterLayer = "Water";

		public static readonly string TubeLayer = "Tube";

		public static readonly string LocalPlayer = "LocalPlayer";

		public static readonly string RemotePlayer = "RemotePlayer";

		public static int GetPlayerLayerCollisionMask()
		{
			return (1 << LayerMask.NameToLayer(InvisibleBarrierLayer)) | (1 << LayerMask.NameToLayer(DefaultLayer)) | (1 << LayerMask.NameToLayer(TerrainBarrierLayer)) | (1 << LayerMask.NameToLayer(NonPlayerLayer));
		}

		public static int GetTubeLayerCollisionMask()
		{
			return GetPlayerLayerCollisionMask() | (1 << LayerMask.NameToLayer(WaterLayer));
		}

		public static int GetSurfaceSamplerLayerCollisionMask()
		{
			return (1 << LayerMask.NameToLayer(DefaultLayer)) | (1 << LayerMask.NameToLayer(InvisibleBarrierLayer)) | (1 << LayerMask.NameToLayer(TerrainBarrierLayer)) | (1 << LayerMask.NameToLayer(WaterLayer));
		}

		public static int GetAllPlayersLayerCollisionMask()
		{
			return (1 << LayerMask.NameToLayer(LocalPlayer)) | (1 << LayerMask.NameToLayer(RemotePlayer));
		}
	}
}
