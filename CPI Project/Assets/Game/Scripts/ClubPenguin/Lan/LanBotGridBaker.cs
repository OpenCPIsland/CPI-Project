#if !UNITY_WEBGL
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanBotGridBaker : MonoBehaviour
	{
		private const float CellSize = 0.5f;

		private const int CellsPerFrame = 3000;

		private const float MaxExtent = 520f;

		private static LanBotGridBaker instance;

		private static LanServer server;

		private static readonly Dictionary<string, LanBotGrid> loaded = new Dictionary<string, LanBotGrid>();

		private string zoneSeen;

		private float zoneSince;

		private bool baking;

		private float looksCheckAt;

		public static void Attach(LanServer host)
		{
			server = host;
			if (instance == null)
			{
				GameObject go = new GameObject("LanBotGridBaker");
				UnityEngine.Object.DontDestroyOnLoad(go);
				instance = go.AddComponent<LanBotGridBaker>();
				LoadAll();
			}
			foreach (KeyValuePair<string, LanBotGrid> kv in loaded)
			{
				host.Bots.SetGrid(kv.Key, kv.Value);
			}
		}

		public static void Detach(LanServer host)
		{
			if (server == host)
			{
				server = null;
			}
		}

		private static string Folder()
		{
			return Path.Combine(Application.persistentDataPath, "LanBotGrids");
		}

		private static string FileFor(string zone)
		{
			foreach (char c in Path.GetInvalidFileNameChars())
			{
				zone = zone.Replace(c, '_');
			}
			return Path.Combine(Folder(), zone + ".grid");
		}

		private static void LoadAll()
		{
			try
			{
				string folder = Folder();
				if (!Directory.Exists(folder))
				{
					return;
				}
				foreach (string file in Directory.GetFiles(folder, "*.grid"))
				{
					LanBotGrid grid = LanBotGrid.Deserialize(File.ReadAllBytes(file));
					if (grid != null && !string.IsNullOrEmpty(grid.Zone))
					{
						loaded[grid.Zone] = grid;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void Update()
		{
			if (server == null)
			{
				return;
			}
			if (Time.realtimeSinceStartup >= looksCheckAt)
			{
				looksCheckAt = Time.realtimeSinceStartup + 10f;
				if (!server.Bots.HasTubes)
				{
					LanTweaks.RefreshBotLooks();
				}
			}
			if (baking)
			{
				return;
			}
			ZoneDefinition zone = null;
			try
			{
				if (Service.IsSet<ZoneTransitionService>())
				{
					ZoneTransitionService zts = Service.Get<ZoneTransitionService>();
					if (!zts.IsTransitioning)
					{
						zone = zts.CurrentZone;
					}
				}
			}
			catch (Exception)
			{
				zone = null;
			}
			string name = (zone != null && zone.Type != ZoneDefinition.ZoneType.Igloo) ? zone.ZoneName : null;
			if (name != zoneSeen)
			{
				zoneSeen = name;
				zoneSince = Time.realtimeSinceStartup;
				return;
			}
			if (string.IsNullOrEmpty(name) || loaded.ContainsKey(name) || Time.realtimeSinceStartup - zoneSince < 5f)
			{
				return;
			}
			StartCoroutine(Bake(name));
		}

		private static int Layer(string name)
		{
			return LayerMask.NameToLayer(name);
		}

		private static int Mask(params string[] names)
		{
			int mask = 0;
			foreach (string n in names)
			{
				int l = Layer(n);
				if (l >= 0)
				{
					mask |= 1 << l;
				}
			}
			return mask;
		}

		private IEnumerator Bake(string zone)
		{
			baking = true;
			int barrierMask = Mask("InvisibleBarrier", "TerrainBarrier", "ManipulateObject");
			int waterMask = Mask("Water");
			int groundMask = Mask("Default", "NonPlayer") | barrierMask | waterMask;
			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			bool any = false;
			foreach (Collider c in UnityEngine.Object.FindObjectsOfType<Collider>())
			{
				if (c == null || c.isTrigger || ((1 << c.gameObject.layer) & groundMask) == 0)
				{
					continue;
				}
				Bounds b = c.bounds;
				if (b.size.x > MaxExtent * 4f || b.size.z > MaxExtent * 4f)
				{
					continue;
				}
				if (!any)
				{
					bounds = b;
					any = true;
				}
				else
				{
					bounds.Encapsulate(b);
				}
			}
			if (!any)
			{
				baking = false;
				yield break;
			}
			Vector3 center = bounds.center;
			float sizeX = Mathf.Min(bounds.size.x, MaxExtent);
			float sizeZ = Mathf.Min(bounds.size.z, MaxExtent);
			LanBotGrid grid = new LanBotGrid();
			grid.Zone = zone;
			grid.Cell = CellSize;
			grid.MinX = center.x - sizeX * 0.5f;
			grid.MinZ = center.z - sizeZ * 0.5f;
			grid.W = Mathf.Max(1, Mathf.CeilToInt(sizeX / CellSize));
			grid.H = Mathf.Max(1, Mathf.CeilToInt(sizeZ / CellSize));
			grid.Flags = new byte[grid.W * grid.H];
			grid.Heights = new short[grid.W * grid.H];
			float top = bounds.max.y + 5f;
			float depth = bounds.size.y + 10f;
			int done = 0;
			for (int z = 0; z < grid.H; z++)
			{
				for (int x = 0; x < grid.W; x++)
				{
					int i = z * grid.W + x;
					Vector3 origin = new Vector3(grid.MinX + (x + 0.5f) * CellSize, top, grid.MinZ + (z + 0.5f) * CellSize);
					RaycastHit hit;
					if (Physics.Raycast(origin, Vector3.down, out hit, depth, groundMask, QueryTriggerInteraction.Ignore))
					{
						int bit = 1 << hit.collider.gameObject.layer;
						grid.Heights[i] = (short)Mathf.Clamp(Mathf.RoundToInt(hit.point.y * 20f), short.MinValue, short.MaxValue);
						if ((bit & (barrierMask | waterMask)) != 0 || hit.normal.y < 0.45f)
						{
							grid.Flags[i] = 2;
						}
						else if (Physics.CheckCapsule(hit.point + Vector3.up * 0.45f, hit.point + Vector3.up * 1.4f, 0.3f, barrierMask, QueryTriggerInteraction.Ignore))
						{
							grid.Flags[i] = 2;
						}
						else
						{
							grid.Flags[i] = 1;
						}
					}
					done++;
					if (done >= CellsPerFrame)
					{
						done = 0;
						yield return null;
						if (server == null || zoneSeen != zone)
						{
							baking = false;
							yield break;
						}
					}
				}
			}
			loaded[zone] = grid;
			try
			{
				Directory.CreateDirectory(Folder());
				File.WriteAllBytes(FileFor(zone), grid.Serialize());
			}
			catch (Exception)
			{
			}
			if (server != null)
			{
				server.Bots.SetGrid(zone, grid);
			}
			baking = false;
		}
	}
}
#endif
