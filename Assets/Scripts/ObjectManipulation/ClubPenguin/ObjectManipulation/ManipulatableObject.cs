using ClubPenguin.Core;
using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation
{
	public class ManipulatableObject : MonoBehaviour
	{
		[HideInInspector]
		public string PathId = string.Empty;

		public int DefinitionId = -1;

		public DecorationLayoutData.DefinitionType Type;

		[SerializeField]
		private bool isSquashed;

		public bool IsSquashed
		{
			get
			{
				return isSquashed;
			}
		}

		public event Action<GameObject, bool> OnRemoved;

		public event Action<Transform, GameObject> BeforeParentChanged;

		public event Action<Transform, GameObject> AfterParentChanged;

		public event Action<bool> IsSquashedChanged;

		private void OnDestroy()
		{
			this.OnRemoved = null;
			this.BeforeParentChanged = null;
			this.AfterParentChanged = null;
			this.IsSquashedChanged = null;
		}

		internal void CheckIfSquashedChanged()
		{
			CollidableObject[] componentsInChildren = GetComponentsInChildren<CollidableObject>();
			int num = componentsInChildren.Length;
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				if (componentsInChildren[i].IsSquashed)
				{
					flag = true;
					break;
				}
			}
			if (isSquashed != flag)
			{
				if (this.IsSquashedChanged != null)
				{
					this.IsSquashedChanged.InvokeSafe(flag);
				}
				isSquashed = flag;
			}
		}

		public void RemoveObject(bool deleteChildren)
		{
			if (this.OnRemoved != null)
			{
				this.OnRemoved.InvokeSafe(base.gameObject, deleteChildren);
			}
		}

		public void SetParent(Transform value)
		{
			this.BeforeParentChanged.InvokeSafe(value, base.gameObject);
			if (base.transform.parent != value)
			{
				base.transform.SetParent(value, true);
			}
			this.AfterParentChanged.InvokeSafe(value, base.gameObject);
		}
	}
}
