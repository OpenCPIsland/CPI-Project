using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Locomotion.Primitives
{
	public class WarpAlongPathPrimitive : LocomotionPrimitive
	{
		public struct KeyFrame
		{
			public Vector3 pos
			{
				get;
				private set;
			}

			public float dist
			{
				get;
				private set;
			}

			public void Set(Vector3 _pos, float _dist)
			{
				pos = _pos;
				dist = _dist;
			}

			public void Set(Vector3 _pos, Vector3 prevPos)
			{
				pos = _pos;
				dist = (pos - prevPos).magnitude;
			}
		}

		private WarpAlongPathPrimitiveData mutableData;

		private static readonly float closeEnoughDistSq = 1f;

		private static readonly float timeToAlign = 1f;

		private List<KeyFrame> keyFrames = new List<KeyFrame>();

		private Vector3[] waypoints;

		private int curKeyFrameIndex;

		private float distConsumedFromLastFrame;

		private Animator anim;

		private int animBoolParameter;

		private bool isWarpingToStart;

		private float curWarpSpeed;

		private Vector3 curVel;

		private float elapsedWarpTime;

		private void Awake()
		{
			anim = GetComponent<Animator>();
			isWarpingToStart = true;
			base.enabled = false;
		}

		private void OnEnable()
		{
			calculateSplines();
		}

		private void OnDestroy()
		{
			if (mutableData != null && !string.IsNullOrEmpty(mutableData.AnimBool))
			{
				anim.SetBool(animBoolParameter, false);
			}
		}

		public void SetData(WarpAlongPathPrimitiveData data)
		{
			if (data != null)
			{
				mutableData = Object.Instantiate(data);
				if (!string.IsNullOrEmpty(mutableData.AnimBool))
				{
					animBoolParameter = Animator.StringToHash(mutableData.AnimBool);
					anim.SetBool(animBoolParameter, true);
				}
			}
		}

		public override void ResetState()
		{
			distConsumedFromLastFrame = 0f;
			curKeyFrameIndex = 0;
			curWarpSpeed = 0f;
			curVel = Vector3.zero;
			elapsedWarpTime = 0f;
			base.ResetState();
		}

		public void SetVelocity(Vector3 vel)
		{
			curVel = vel;
			curWarpSpeed = curVel.magnitude;
		}

		public void SetWaypoints(ref Vector3[] points)
		{
			waypoints = points;
		}

		private void OnDrawGizmos()
		{
			calculateSplines();
			for (int i = 0; i < keyFrames.Count; i++)
			{
				Gizmos.DrawWireSphere(keyFrames[i].pos, 0.07f);
			}
		}

		private void calculateSplines()
		{
			keyFrames.Clear();
			if (mutableData == null || ((waypoints == null || waypoints.Length < 2) && mutableData.Waypoints == null))
			{
				return;
			}
			float num = 1f / (float)mutableData.Steps;
			if (waypoints == null)
			{
				Transform[] componentsInChildren = mutableData.Waypoints.GetComponentsInChildren<Transform>(true);
				if (componentsInChildren.Length < 3)
				{
					return;
				}
				waypoints = new Vector3[componentsInChildren.Length - 1];
				for (int i = 0; i < waypoints.Length; i++)
				{
					waypoints[i] = componentsInChildren[i + 1].position;
				}
			}
			for (int j = 1; j < waypoints.Length; j++)
			{
				Vector3 vector = waypoints[j - 1];
				Vector3 vector2 = waypoints[j];
				Vector3 vector3;
				Vector3 a;
				if (j < waypoints.Length - 1)
				{
					if (j > 2)
					{
						vector3 = mutableData.Curvature * (vector - waypoints[j - 2]);
						a = mutableData.Curvature * (vector2 - vector);
					}
					else
					{
						vector3 = mutableData.Curvature * (vector2 - vector);
						a = mutableData.Curvature * (vector2 - vector);
					}
				}
				else if (j > 2)
				{
					vector3 = mutableData.Curvature * (vector - waypoints[j - 2]);
					a = mutableData.Curvature * (vector2 - vector);
				}
				else
				{
					vector3 = mutableData.Curvature * (vector2 - vector);
					a = vector3;
				}
				for (int k = 0; k < mutableData.Steps; k++)
				{
					float num2 = (float)k * num;
					float num3 = num2 * num2;
					float num4 = num3 * num2;
					float num5 = 2f * num4;
					float num6 = 3f * num3;
					float d = num5 - num6 + 1f;
					float d2 = 0f - num5 + num6;
					float d3 = num4 - 2f * num3 + num2;
					float d4 = num4 - num3;
					Vector3 pos = d * vector + d2 * vector2 + d3 * vector3 + d4 * a;
					KeyFrame item = default(KeyFrame);
					if (keyFrames.Count == 0)
					{
						item.Set(pos, 0f);
						keyFrames.Add(item);
					}
					else
					{
						item.Set(pos, keyFrames[keyFrames.Count - 1].pos);
						keyFrames.Add(item);
					}
				}
				if (j == waypoints.Length - 1)
				{
					KeyFrame item = default(KeyFrame);
					item.Set(vector2, keyFrames[keyFrames.Count - 1].pos);
					keyFrames.Add(item);
				}
			}
		}

		private Vector3 GetNextPos(float deltaDist)
		{
			Vector3 result = base.transform.position;
			for (int i = curKeyFrameIndex + 1; i < keyFrames.Count; i++)
			{
				float num;
				if (i == 1)
				{
					num = (keyFrames[i].pos - base.transform.position).magnitude;
					if (deltaDist <= num)
					{
						distConsumedFromLastFrame = 0f;
						result = base.transform.position + (keyFrames[i].pos - base.transform.position).normalized * deltaDist;
						curKeyFrameIndex = i - 1;
						break;
					}
				}
				else
				{
					num = keyFrames[i].dist - distConsumedFromLastFrame;
				}
				deltaDist -= num;
				if (deltaDist <= 0f)
				{
					distConsumedFromLastFrame = keyFrames[i].dist + deltaDist;
					result = keyFrames[i - 1].pos + (keyFrames[i].pos - keyFrames[i - 1].pos).normalized * distConsumedFromLastFrame;
					curKeyFrameIndex = i - 1;
					break;
				}
				curKeyFrameIndex++;
				distConsumedFromLastFrame = 0f;
				result = keyFrames[i].pos;
			}
			return result;
		}

		private void Update()
		{
			if (keyFrames.Count > 1)
			{
				if (isWarpingToStart)
				{
					Vector3 rhs = keyFrames[0].pos - base.transform.position;
					Vector3 normalized = rhs.normalized;
					curWarpSpeed += mutableData.StartAccel * Time.deltaTime;
					curWarpSpeed = Mathf.Clamp(curWarpSpeed, 0f, mutableData.WarpSpeed);
					elapsedWarpTime += Time.deltaTime;
					float t = Mathf.Clamp01(elapsedWarpTime / timeToAlign);
					Vector3 b = normalized * curWarpSpeed;
					curVel = Vector3.Lerp(curVel, b, t);
					Vector3 vector = base.transform.position + curVel * Time.deltaTime;
					Output.wsDeltaPos = vector - base.transform.position;
					Vector3 lhs = keyFrames[0].pos - vector;
					float sqrMagnitude = lhs.sqrMagnitude;
					if (sqrMagnitude < closeEnoughDistSq || Vector3.Dot(lhs, rhs) <= 0f)
					{
						isWarpingToStart = false;
					}
				}
				else
				{
					float deltaDist = mutableData.WarpSpeed * Time.deltaTime;
					Vector3 vector = GetNextPos(deltaDist);
					if (vector != base.transform.position)
					{
						curVel = (vector - base.transform.position) / Time.deltaTime;
						base.transform.position = vector;
					}
					else
					{
						if (mutableData.StopAtEndPoint)
						{
							base.transform.position = vector;
							Output.wsVelocity = Vector3.zero;
						}
						else
						{
							base.transform.position += curVel * Time.deltaTime;
							Output.wsVelocity = curVel;
						}
						base.IsFinished = true;
					}
				}
				Vector3 normalized2 = curVel.normalized;
				Quaternion b2 = Quaternion.LookRotation(upwards: new Vector3(0f - normalized2.y, normalized2.x, 0f), forward: normalized2);
				Output.wsRotation = Quaternion.Slerp(base.transform.rotation, b2, mutableData.TurnSmoothing * Time.deltaTime);
			}
			else
			{
				base.IsFinished = true;
			}
		}
	}
}
