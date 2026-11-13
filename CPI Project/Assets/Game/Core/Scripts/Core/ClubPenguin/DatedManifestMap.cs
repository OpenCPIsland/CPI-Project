using Disney.Kelowna.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class DatedManifestMap : ScriptableObject
	{
		public List<DatedManifest> Manifests = new List<DatedManifest>();

		private Dictionary<DateTime, UnityEngine.Object> map;

		public Dictionary<DateTime, UnityEngine.Object> Map
		{
			get
			{
				if (map == null)
				{
					map = new Dictionary<DateTime, UnityEngine.Object>();
					foreach (DatedManifest manifest in Manifests)
					{
						map.Add(manifest.Date.Date, manifest.Manifest);
					}
				}
				return map;
			}
		}

		public void Add(DateTime date, UnityEngine.Object manifest)
		{
			DatedManifest datedManifest = new DatedManifest();
			datedManifest.Date = new DateUnityWrapper();
			datedManifest.Date.Date = date;
			datedManifest.Manifest = manifest;
			Manifests.Add(datedManifest);
			map = null;
		}

		public void OnValidate()
		{
			HashSet<DateTime> hashSet = new HashSet<DateTime>();
			foreach (DatedManifest manifest in Manifests)
			{
				hashSet.Add(manifest.Date.Date);
			}
		}
	}
}
