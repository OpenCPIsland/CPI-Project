using Disney.MobileNetwork;
using System;
using System.Collections;
using Tweaker.Core;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class CoroutineRunner : MonoBehaviour
	{
		protected const string PERSISTENT_DEBUG_PREFIX = "P";

		protected const string TRANSIENT_DEBUG_PREFIX = "T";

		protected readonly CoroutineOwnerMap<RadicalCoroutine> persistentOwnerMap = new CoroutineOwnerMap<RadicalCoroutine>();

		protected readonly CoroutineOwnerMap<RadicalCoroutine> transientOwnerMap = new CoroutineOwnerMap<RadicalCoroutine>();

		private static CoroutineRunner instance
		{
			get
			{
				return Service.Get<CoroutineRunner>();
			}
		}

		[Tweakable("Debug.Monitor.ActiveCoroutineCount", Description = "[READ ONLY] The active number of coroutines tracked by CoroutineRunner.")]
		private static int activeCoroutineCount
		{
			get
			{
				return instance.transientOwnerMap.GetCountForAllOwners() + instance.persistentOwnerMap.GetCountForAllOwners();
			}
			set
			{
			}
		}

		[Tweakable("Debug.Monitor.ActiveCoroutineOwnerCount", Description = "[READ ONLY] The active number of coroutine owners tracked by CoroutineRunner.")]
		private static int activeCoroutineOwnerCount
		{
			get
			{
				return instance.transientOwnerMap.GetOwnerCount() + instance.persistentOwnerMap.GetOwnerCount();
			}
			set
			{
			}
		}

		public static ICoroutine Start(IEnumerator enumerator, object owner, string debugName)
		{
			return instance.start(enumerator, owner, debugName, "T", instance.transientOwnerMap);
		}

		public static ICoroutine StartPersistent(IEnumerator enumerator, object owner, string debugName)
		{
			return instance.start(enumerator, owner, debugName, "P", instance.persistentOwnerMap);
		}

		protected ICoroutine start(IEnumerator enumerator, object owner, string debugName, string prefix, CoroutineOwnerMap<RadicalCoroutine> map)
		{
			RadicalCoroutine radicalCoroutine = RadicalCoroutine.Create(enumerator, getDebugName(owner, debugName, prefix));
			map.Add(owner, radicalCoroutine);
			radicalCoroutine.unityOwner = this;
			radicalCoroutine.unityCoroutine = StartCoroutine(radicalCoroutine.Enumerator);
			return radicalCoroutine;
		}

		public static void StopAllForOwner(object owner)
		{
			instance.transientOwnerMap.StopAllForOwner(owner);
			instance.persistentOwnerMap.StopAllForOwner(owner);
		}

		public static int GetCountForOwner(object owner)
		{
			return instance.transientOwnerMap.GetCountForOwner(owner) + instance.persistentOwnerMap.GetCountForOwner(owner);
		}

		[Invokable("Debug.Monitor.StopTransientCoroutines", Description = "Stops all transient coroutines. Normally called during scene/zone transitions.")]
		public static void StopTransientCoroutines()
		{
			instance.transientOwnerMap.StopAll();
			instance.transientOwnerMap.Clear();
		}

		private static string getDebugName(object owner, string debugName, string prefix)
		{
			if (owner is Type)
			{
				return string.Format("({0}:{1}){2}:{3}", Time.frameCount, prefix, ((Type)owner).Name, debugName);
			}
			return string.Format("({0}:{1}){2}:{3}", Time.frameCount, prefix, owner.GetType().Name, debugName);
		}
	}
}
