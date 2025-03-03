using ClubPenguin;
using ClubPenguin.Avatar;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace CpRemixShaders
{
	[ExecuteInEditMode]
	public class DepthColoredShadersController : MonoBehaviour
	{
		private const string SURFACE_Y_POS_PROP = "_SurfaceYCoord";

		private const string DEEPEST_Y_POS_PROP = "_DeepestYCoord";

		private const string DEPTH_COLOR_PROP = "_DepthColor";

		private const string REFLECT_TEX_PROP = "_SurfaceReflectionsRGB";

		private const string REFLECT_TEX_TILE_PROP = "_SurfaceTexTile";

		private const string REFLECT_MULT_PROP = "_SurfaceMultiplier";

		private const string REFLECT_X_VEL_PROP = "_SurfaceVelocityX";

		private const string REFLECT_Z_VEL_PROP = "_SurfaceVelocityZ";

		private const string REFLECT_COL_PROP = "_SurfaceReflectionColor";

		private const string DYN_REFLECT_TEX_TILE_PROP = "_DynSurfaceTexTile";

		private const string DYN_REFLECT_MULT_PROP = "_DynSurfaceMultiplier";

		private const float UPDATE_TIME_SEC = 0.5f;

		public float SurfaceWorldPositionY = 5f;

		public float DeepestWorldPositionY = -100f;

		public Color DepthColor = new Color(0f, 0.1f, 0.3f);

		public Texture2D ReflectionTexture;

		public Color ReflectionColor = new Color(0.5f, 0.8f, 1f);

		public float ReflectionTiling = 1f;

		public float ReflectionBrightness = 2f;

		public float ReflectionXVelocity = 1f;

		public float ReflectionZVelocity = 1f;

		public float DynObjReflcTiling = 1f;

		public float DynObjReflcBrightness = 2f;

		private float prevSurfaceY;

		private float prevDeepestPos;

		private Color prevDepthColor;

		private Texture2D prevReflectionTexture;

		private Color prevReflectionColor;

		private float prevReflectionTiling;

		private float prevReflectionBrightness;

		private float prevReflectionXVelocity;

		private float prevReflectionZVelocity;

		private float prevDynReflectionTiling;

		private float prevDynReflectionBrightness;

		private EventDispatcher eventDispatcher;

		private Shader combinedAvatarDepthShader;

		private float timeElapsedSec;

		private void Awake()
		{
			prevSurfaceY = SurfaceWorldPositionY;
			prevDeepestPos = DeepestWorldPositionY;
			prevDepthColor = DepthColor;
			prevReflectionTexture = ReflectionTexture;
			prevReflectionColor = ReflectionColor;
			prevReflectionTiling = ReflectionTiling;
			prevReflectionBrightness = ReflectionBrightness;
			prevReflectionXVelocity = ReflectionXVelocity;
			prevReflectionZVelocity = ReflectionZVelocity;
			prevDynReflectionTiling = DynObjReflcTiling;
			prevDynReflectionBrightness = DynObjReflcBrightness;
			combinedAvatarDepthShader = Shader.Find("CpRemix/Combined Avatar Depth");
			Shader.SetGlobalFloat("_SurfaceYCoord", SurfaceWorldPositionY);
			Shader.SetGlobalFloat("_DeepestYCoord", DeepestWorldPositionY);
			Shader.SetGlobalColor("_DepthColor", DepthColor);
			Shader.SetGlobalTexture("_SurfaceReflectionsRGB", ReflectionTexture);
			Shader.SetGlobalColor("_SurfaceReflectionColor", ReflectionColor);
			Shader.SetGlobalFloat("_SurfaceTexTile", ReflectionTiling);
			Shader.SetGlobalFloat("_SurfaceMultiplier", ReflectionBrightness);
			Shader.SetGlobalFloat("_SurfaceVelocityX", ReflectionXVelocity);
			Shader.SetGlobalFloat("_SurfaceVelocityZ", ReflectionZVelocity);
			Shader.SetGlobalFloat("_DynSurfaceTexTile", DynObjReflcTiling);
			Shader.SetGlobalFloat("_DynSurfaceMultiplier", DynObjReflcBrightness);
			if (Application.isPlaying)
			{
				eventDispatcher = Service.Get<EventDispatcher>();
				eventDispatcher.AddListener<PlayerSpawnedEvents.RemotePlayerSpawned>(onRemotePlayerSpawned);
				eventDispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			}
		}

		private bool onRemotePlayerSpawned(PlayerSpawnedEvents.RemotePlayerSpawned evt)
		{
			AvatarView component = evt.RemotePlayerGameObject.GetComponent<AvatarView>();
			component.OnReady += onViewReady;
			if (component.IsReady)
			{
				onViewReady(component);
			}
			return false;
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			AvatarView component = evt.LocalPlayerGameObject.GetComponent<AvatarView>();
			component.OnReady += onViewReady;
			if (component.IsReady)
			{
				onViewReady(component);
			}
			return false;
		}

		private void onViewReady(AvatarBaseAsync avatarView)
		{
			SkinnedMeshRenderer component = avatarView.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				component.sharedMaterial.shader = combinedAvatarDepthShader;
			}
		}

		private void Update()
		{
			if (Application.isPlaying)
			{
				timeElapsedSec += Time.deltaTime;
				if (timeElapsedSec < 0.5f)
				{
					return;
				}
				timeElapsedSec = 0f;
			}
			if (prevSurfaceY != SurfaceWorldPositionY)
			{
				Shader.SetGlobalFloat("_SurfaceYCoord", SurfaceWorldPositionY);
				prevSurfaceY = SurfaceWorldPositionY;
			}
			if (prevDeepestPos != DeepestWorldPositionY)
			{
				Shader.SetGlobalFloat("_DeepestYCoord", DeepestWorldPositionY);
				prevDeepestPos = DeepestWorldPositionY;
			}
			if (prevDepthColor != DepthColor)
			{
				Shader.SetGlobalColor("_DepthColor", DepthColor);
				prevDepthColor = DepthColor;
			}
			if (prevReflectionTexture != ReflectionTexture)
			{
				Shader.SetGlobalTexture("_SurfaceReflectionsRGB", ReflectionTexture);
				prevReflectionTexture = ReflectionTexture;
			}
			if (prevReflectionColor != ReflectionColor)
			{
				Shader.SetGlobalColor("_SurfaceReflectionColor", ReflectionColor);
				prevReflectionColor = ReflectionColor;
			}
			if (prevReflectionTiling != ReflectionTiling)
			{
				Shader.SetGlobalFloat("_SurfaceTexTile", ReflectionTiling);
				prevReflectionTiling = ReflectionTiling;
			}
			if (prevReflectionBrightness != ReflectionBrightness)
			{
				Shader.SetGlobalFloat("_SurfaceMultiplier", ReflectionBrightness);
				prevReflectionBrightness = ReflectionBrightness;
			}
			if (prevReflectionXVelocity != ReflectionXVelocity)
			{
				Shader.SetGlobalFloat("_SurfaceVelocityX", ReflectionXVelocity);
				prevReflectionXVelocity = ReflectionXVelocity;
			}
			if (prevReflectionZVelocity != ReflectionZVelocity)
			{
				Shader.SetGlobalFloat("_SurfaceVelocityZ", ReflectionZVelocity);
				prevReflectionZVelocity = ReflectionZVelocity;
			}
			if (prevDynReflectionTiling != DynObjReflcTiling)
			{
				Shader.SetGlobalFloat("_DynSurfaceTexTile", DynObjReflcTiling);
				prevDynReflectionTiling = DynObjReflcTiling;
			}
			if (prevDynReflectionBrightness != DynObjReflcBrightness)
			{
				Shader.SetGlobalFloat("_DynSurfaceMultiplier", DynObjReflcBrightness);
				prevDynReflectionBrightness = DynObjReflcBrightness;
			}
		}
	}
}
