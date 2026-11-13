#define UNITY_ASSERTIONS
using Disney.Kelowna.Common;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin.ObjectManipulation
{
	public class PartneredObject : MonoBehaviour
	{
		private PartneredObject other;

		[HideInInspector]
		public string Guid;

		[HideInInspector]
		public string PartnerGuid;

		[HideInInspector]
		public int Number = 0;

		public PartneredObject Other
		{
			get
			{
				return other;
			}
			set
			{
				if (value != other)
				{
					other = value;
					this.OtherSet.InvokeSafe(this);
				}
			}
		}

		public event Action<PartneredObject> OtherSet;

		public string GetGuid()
		{
			if (string.IsNullOrEmpty(Guid))
			{
				Guid = System.Guid.NewGuid().ToString();
			}
			return Guid;
		}

		public void SetOthers(GameObject[] others)
		{
			Assert.IsTrue(others.Length == 2, "The paired item only works with two items");
			for (int i = 0; i < others.Length; i++)
			{
				if (others[i] != base.gameObject)
				{
					PartneredObject component = others[i].GetComponent<PartneredObject>();
					if (component != null)
					{
						Other = component;
					}
				}
			}
		}

		public void SetNumber(int value)
		{
			Assert.IsTrue(value > 0, "The value must be greater than 0");
			Number = value;
			TextMesh componentInChildren = GetComponentInChildren<TextMesh>();
			if (componentInChildren != null)
			{
				componentInChildren.text = value.ToString();
			}
		}

		public void OnDestroy()
		{
			this.OtherSet = null;
		}
	}
}
