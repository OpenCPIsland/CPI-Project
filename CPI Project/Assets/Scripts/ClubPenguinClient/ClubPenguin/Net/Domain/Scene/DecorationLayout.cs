using ClubPenguin.Net.Domain.Decoration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Net.Domain.Scene
{
	[Serializable]
	public struct DecorationLayout
	{
		public struct Id
		{
			public string name;

			public string parent;
		}

		public Id id;

		public DecorationType type;

		public long definitionId;

		public Vector3 position;

		public Quaternion rotation;

		public float scale;

		public Vector3 normal;

		public Dictionary<string, string> customProperties;
	}
}
