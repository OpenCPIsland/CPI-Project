using Disney.Kelowna.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ObjectManipulation
{
	[DisallowMultipleComponent]
	public class ObjectManipulator : MonoBehaviour
	{
		private HashSet<Collider> currentColliders;

		[HideInInspector]
		public bool WasReparented = false;

		private bool baseLocationIsValid = true;

		private bool collisionStateIsValid = true;

		public bool BaseLocationIsValid
		{
			set
			{
				bool isAllowed = IsAllowed;
				baseLocationIsValid = value;
				if (isAllowed != IsAllowed && this.IsAllowedChanged != null)
				{
					this.IsAllowedChanged.InvokeSafe();
				}
			}
		}

		public bool CollisionIsValid
		{
			set
			{
				bool isAllowed = IsAllowed;
				collisionStateIsValid = value;
				if (isAllowed != IsAllowed && this.IsAllowedChanged != null)
				{
					this.IsAllowedChanged.InvokeSafe();
				}
			}
		}

		public bool IsAllowed
		{
			get
			{
				return baseLocationIsValid && collisionStateIsValid;
			}
		}

		public float CurrentRotationDegreesAroundUp
		{
			get
			{
				Vector3 vector = Vector3.up;
				if (base.transform.up != vector)
				{
					vector = Vector3.Cross(vector, base.transform.up);
				}
				return Vector3.Dot(base.transform.localEulerAngles, vector);
			}
			private set
			{
			}
		}

		public float Scale
		{
			get
			{
				return GetWorldSpaceScale(base.transform);
			}
			private set
			{
			}
		}

		public HashSet<Collider> CurrentColliders
		{
			get
			{
				if (currentColliders == null)
				{
					currentColliders = new HashSet<Collider>();
				}
				return currentColliders;
			}
			private set
			{
			}
		}

		public event Action<ObjectManipulator> PositionChanged;

		public event Action<ObjectManipulator> RotationChanged;

		public event Action<ObjectManipulator> ScaleChanged;

		public event Action IsAllowedChanged;

		public event Action<Collider> TriggerEnter;

		public event Action<Collider> TriggerExit;

		public static float GetWorldSpaceScale(Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.localScale.x;
			}
			Transform parent = transform.parent;
			transform.SetParent(null, true);
			float x = transform.localScale.x;
			transform.SetParent(parent, true);
			return x;
		}

		private void Awake()
		{
			int num = base.name.IndexOf("_");
			int result;
			if (num < 0 || !int.TryParse(base.name.Substring(0, num), out result))
			{
				SetUniqueGameObjectName(base.gameObject, base.transform.parent);
			}
		}

		public void Start()
		{
			CollidableObjectCollisionOverrides component = GetComponent<CollidableObjectCollisionOverrides>();
			if (component != null)
			{
				component.SetDragging(true);
			}
		}

		public void OnDestroy()
		{
			this.PositionChanged = null;
			this.RotationChanged = null;
			this.ScaleChanged = null;
			this.TriggerEnter = null;
			this.TriggerExit = null;
			this.IsAllowedChanged = null;
			CollidableObjectCollisionOverrides component = GetComponent<CollidableObjectCollisionOverrides>();
			if (component != null)
			{
				component.SetDragging(false);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			trackCollider(other);
			if (this.TriggerEnter != null)
			{
				this.TriggerEnter.InvokeSafe(other);
			}
		}

		public void OnTriggerExit(Collider other)
		{
			removeCollider(other);
			if (this.TriggerExit != null)
			{
				this.TriggerExit.InvokeSafe(other);
			}
		}

		public void OnBeforeTransformParentChanged()
		{
		}

		public override string ToString()
		{
			return base.ToString();
		}

		private void trackCollider(Collider other)
		{
			CurrentColliders.Add(other);
		}

		private void removeCollider(Collider other)
		{
			CurrentColliders.Remove(other);
		}

		public void AlignWith(Vector3 normal, Transform parent)
		{
			if (base.transform.up != normal)
			{
				Vector3 forward = base.transform.forward - Vector3.Dot(base.transform.forward, normal) * normal;
				base.transform.rotation = Quaternion.LookRotation(forward, normal);
				base.transform.up = normal;
			}
			PerserveLocalNonZeroRotation component = GetComponent<PerserveLocalNonZeroRotation>();
			if (component != null)
			{
				component.RestoreNonZeroRotation(parent);
			}
		}

		public void SetPosition(Vector3 newPosition)
		{
			Vector3 vector = new Vector3(newPosition.x, newPosition.y, newPosition.z);
			if (vector != base.transform.position)
			{
				base.transform.position = vector;
				if (this.PositionChanged != null)
				{
					this.PositionChanged.InvokeSafe(this);
				}
			}
		}

		public void SetRotation(Quaternion rotation)
		{
			if (base.transform.rotation != rotation)
			{
				base.transform.rotation = rotation;
				if (this.RotationChanged != null)
				{
					this.RotationChanged.InvokeSafe(this);
				}
			}
		}

		public void RotateBy(float degrees)
		{
			Quaternion lhs = Quaternion.AngleAxis(degrees, base.transform.up);
			base.transform.rotation = lhs * base.transform.rotation;
			if (this.RotationChanged != null)
			{
				this.RotationChanged.InvokeSafe(this);
			}
		}

		public void ScaleTo(float value)
		{
			bool flag = false;
			Transform parent = base.transform.parent;
			base.transform.SetParent(null, true);
			if (base.transform.localScale.x != value)
			{
				Dictionary<ManipulatableObject, GameObject> dictionary = new Dictionary<ManipulatableObject, GameObject>();
				ManipulatableObject[] componentsInChildren = GetComponentsInChildren<ManipulatableObject>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].transform.parent == base.transform)
					{
						GameObject gameObject = null;
						gameObject = new GameObject();
						dictionary.Add(componentsInChildren[i], gameObject);
						gameObject.transform.SetParent(componentsInChildren[i].transform.parent);
						gameObject.transform.position = componentsInChildren[i].transform.position;
						componentsInChildren[i].transform.SetParent(null, true);
					}
				}
				base.transform.localScale = new Vector3(value, value, value);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].transform != base.transform)
					{
						ManipulatableObject manipulatableObject = componentsInChildren[i];
						if (dictionary.ContainsKey(manipulatableObject))
						{
							GameObject gameObject = dictionary[manipulatableObject];
							manipulatableObject.transform.SetParent(gameObject.transform.parent, true);
							manipulatableObject.transform.position = gameObject.transform.position;
						}
					}
				}
				foreach (GameObject value2 in dictionary.Values)
				{
					UnityEngine.Object.Destroy(value2);
				}
				flag = true;
			}
			base.transform.SetParent(parent, true);
			if (flag && this.ScaleChanged != null)
			{
				this.ScaleChanged.InvokeSafe(this);
			}
		}

		public void SetParent(Transform value)
		{
			if (base.transform.parent != value)
			{
				base.transform.SetParent(value, true);
				SetUniqueGameObjectName(base.gameObject, base.transform.parent);
			}
		}

		public static void SetUniqueGameObjectName(GameObject obj, Transform parent)
		{
			bool flag = false;
			int result = -1;
			int num = obj.name.IndexOf("_");
			if (num > -1)
			{
				int.TryParse(obj.name.Substring(0, num), out result);
			}
			int num2 = 0;
			if (parent != null)
			{
				int childCount = parent.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = parent.GetChild(i);
					if (child == obj.transform)
					{
						continue;
					}
					num = child.name.IndexOf("_");
					int result2;
					if (num > -1 && int.TryParse(child.name.Substring(0, num), out result2))
					{
						if (result2 == result)
						{
							flag = true;
						}
						if (result2 > num2)
						{
							num2 = result2;
						}
					}
				}
			}
			if (result == -1 || flag)
			{
				obj.name = num2 + 1 + "_" + obj.name.Substring(obj.name.IndexOf("_") + 1);
			}
		}
	}
}
