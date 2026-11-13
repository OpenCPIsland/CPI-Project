using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class SharedActionGraphState : MonoBehaviour
	{
		public const int UNLIMITED_PLAYERS = -1;

		[Tooltip("The number of players allowed to execute the action sequence simultaneously. Set to -1 for unlimited.")]
		public int MaxInteractors = -1;

		private HashSet<GameObject> interactors = new HashSet<GameObject>();

		public bool BoolData
		{
			get;
			set;
		}

		public int IntData
		{
			get;
			set;
		}

		public float FloatData
		{
			get;
			set;
		}

		public string StringData
		{
			get;
			set;
		}

		public GameObject GameObjectData
		{
			get;
			set;
		}

		public HashSet<GameObject> Interactors
		{
			get
			{
				return interactors;
			}
		}
	}
}
