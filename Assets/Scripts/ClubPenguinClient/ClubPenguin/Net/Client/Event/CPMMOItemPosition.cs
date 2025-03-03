using ClubPenguin.Net.Domain;
using System;
using UnityEngine;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct CPMMOItemPosition
	{
		public CPMMOItemId Id;

		public Vector3 Position;
	}
}
