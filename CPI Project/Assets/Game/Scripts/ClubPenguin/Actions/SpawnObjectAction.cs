using ClubPenguin.Locomotion;
using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class SpawnObjectAction : Action
	{
		public GameObjectPool ObjectPool;

		public GameObject ObjectPrefab;

		public Transform SpawnTransform;

		public bool SpawnAtOwnerTransform;

		public bool ParentToOwner = false;

		public bool TransferOwnerMomentum;

		public Vector3 ImpulseDirection;

		public float DirNoise;

		public float ImpulseMagnitude;

		public float MagNoise;

		private Vector3 GetMorphedDirection(ref Vector3 dir, float noiseInDegrees, float distanceT, float circleT)
		{
			noiseInDegrees = Mathf.Clamp(noiseInDegrees, 0f, 90f);
			float num = Mathf.Clamp01(noiseInDegrees / 90f) * distanceT;
			num = 1f - num * num;
			float f = circleT * 2f * (float)Math.PI;
			return new Vector3(Mathf.Sqrt(1f - num * num) * Mathf.Cos(f), Mathf.Sqrt(1f - num * num) * Mathf.Sin(f), num);
		}

		protected override void CopyTo(Action _destWarper)
		{
			SpawnObjectAction spawnObjectAction = _destWarper as SpawnObjectAction;
			spawnObjectAction.ObjectPool = ObjectPool;
			spawnObjectAction.ObjectPrefab = ObjectPrefab;
			spawnObjectAction.SpawnTransform = SpawnTransform;
			spawnObjectAction.SpawnAtOwnerTransform = SpawnAtOwnerTransform;
			spawnObjectAction.ParentToOwner = ParentToOwner;
			spawnObjectAction.TransferOwnerMomentum = TransferOwnerMomentum;
			spawnObjectAction.ImpulseDirection = ImpulseDirection;
			spawnObjectAction.ImpulseMagnitude = ImpulseMagnitude;
			spawnObjectAction.DirNoise = DirNoise;
			spawnObjectAction.MagNoise = MagNoise;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			if (base.Complete)
			{
				return;
			}
			GameObject gameObject = null;
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			bool flag = false;
			if (SpawnTransform == null)
			{
				if (SpawnAtOwnerTransform)
				{
					vector = GetTarget().transform.position;
					quaternion = GetTarget().transform.rotation;
					flag = true;
				}
			}
			else
			{
				vector = SpawnTransform.position;
				quaternion = SpawnTransform.rotation;
				flag = true;
			}
			if (ObjectPool != null)
			{
				gameObject = ((!flag) ? ObjectPool.Spawn() : ObjectPool.Spawn(vector, quaternion));
			}
			else if (ObjectPrefab != null)
			{
				if (flag)
				{
					gameObject = UnityEngine.Object.Instantiate(ObjectPrefab);
					gameObject.transform.position = quaternion * ObjectPrefab.transform.position + vector;
					gameObject.transform.rotation = quaternion * ObjectPrefab.transform.rotation;
				}
				else
				{
					GameObject target = GetTarget();
					Transform transform = target.transform;
					gameObject = UnityEngine.Object.Instantiate(ObjectPrefab);
					gameObject.transform.position = transform.rotation * gameObject.transform.position + transform.position;
					gameObject.transform.rotation *= transform.rotation;
					gameObject.transform.parent = transform;
				}
				if (ParentToOwner)
				{
					GameObject target = GetTarget();
					Transform transform = target.transform;
					gameObject.transform.parent = transform;
				}
			}
			if (gameObject != null)
			{
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					if (TransferOwnerMomentum)
					{
						MotionTracker component2 = GetComponent<MotionTracker>();
						if (component2 != null)
						{
							component.AddForce(component2.Velocity, ForceMode.VelocityChange);
						}
					}
					else if (ImpulseMagnitude != 0f)
					{
						float d = ImpulseMagnitude + (MagNoise - 2f * MagNoise * UnityEngine.Random.value);
						Vector3 dir = ImpulseDirection;
						if (IncomingUserData != null && IncomingUserData.GetType() == typeof(Vector3))
						{
							dir = ((Vector3)IncomingUserData).normalized;
						}
						if (dir == Vector3.zero)
						{
							dir = gameObject.transform.forward;
						}
						if (DirNoise != 0f)
						{
							Vector3 morphedDirection = GetMorphedDirection(ref dir, DirNoise, UnityEngine.Random.value, UnityEngine.Random.value);
							Quaternion rotation = Quaternion.LookRotation(dir);
							dir = rotation * morphedDirection;
						}
						dir.Normalize();
						component.AddForce(dir * d, ForceMode.VelocityChange);
					}
				}
			}
			if (Owner != null)
			{
				Completed(gameObject);
				return;
			}
			Log.LogError(this, "Owner was null. Cannot remove from Sequencer. Action will now be ignored.");
			base.Complete = true;
		}
	}
}
