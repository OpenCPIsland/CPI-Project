using ClubPenguin.Avatar;
using Disney.LaunchPadFramework;
using Disney.LaunchPadFramework.PoolStrategies;
using Disney.LaunchPadFramework.Utility.DesignPatterns;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	[RequireComponent(typeof(MotionTracker))]
	[RequireComponent(typeof(Rig))]
	public class SurfaceEffects : MonoBehaviour
	{
		[Serializable]
		public struct Effector
		{
			public string Alias;

			public Transform EmitterPoint;
		}

		public Effector[] Effectors;

		public SurfaceEffectsData MasterData;

		private readonly Dictionary<string, Transform> emitterPoints = new Dictionary<string, Transform>();

		private GameObjectPool[] system;

		private int prevSurfaceTypeIndex;

		private ParticleSystem prevEffect;

		private void OnEnable()
		{
			system = new GameObjectPool[MasterData.Effects.Length];
			for (int i = 0; i < MasterData.Effects.Length; i++)
			{
				if (MasterData.Effects[i].System != null)
				{
					GameObjectPool foundPool = null;
					if (!Singleton<GameObjectPoolManager>.Instance.TryGetPool(MasterData.Effects[i].System.name, out foundPool))
					{
						foundPool = Singleton<GameObjectPoolManager>.Instance.AddPoolForObject(MasterData.Effects[i].System.gameObject, ScriptableObject.CreateInstance<GrowPool>());
						foundPool.Capacity = 4;
					}
					system[i] = foundPool;
				}
			}
			for (int i = 0; i < Effectors.Length; i++)
			{
				emitterPoints.Add(Effectors[i].Alias, Effectors[i].EmitterPoint);
			}
		}

		private void OnDisable()
		{
			if (prevEffect != null)
			{
				prevEffect.Stop();
				if (system[prevSurfaceTypeIndex] != null)
				{
					system[prevSurfaceTypeIndex].Unspawn(prevEffect.gameObject);
				}
			}
			emitterPoints.Clear();
			system = null;
			prevEffect = null;
			prevSurfaceTypeIndex = -1;
		}

		public void SurfaceFX(string alias)
		{
			if (!base.enabled)
			{
				return;
			}
			Vector3 hitPoint;
			int num = LocomotionUtils.SampleSurface(base.transform, MasterData, out hitPoint);
			if (num < 0)
			{
				return;
			}
			if (prevEffect != null)
			{
				prevEffect.Stop();
				if (system[prevSurfaceTypeIndex] != null)
				{
					system[prevSurfaceTypeIndex].Unspawn(prevEffect.gameObject);
				}
			}
			Vector3 position = Vector3.zero;
			bool flag = false;
			Transform value;
			if (emitterPoints.TryGetValue(alias, out value))
			{
				position = value.position;
				flag = true;
			}
			if (flag)
			{
				if (MasterData.Effects[num].UseCollisionHeight)
				{
					position.y = hitPoint.y;
				}
				if (system[num] != null)
				{
					ParticleSystem component = system[num].Spawn().GetComponent<ParticleSystem>();
					component.transform.position = position;
					component.Play();
					prevEffect = component;
					prevSurfaceTypeIndex = num;
				}
			}
		}
	}
}
