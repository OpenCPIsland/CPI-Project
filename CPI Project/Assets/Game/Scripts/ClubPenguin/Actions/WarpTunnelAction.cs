using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class WarpTunnelAction : Action
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

		private static readonly float minAccelCurveValue = 0.02f;

		private static readonly float closeEnoughDistSq = 1f;

		private static readonly float timeToAlign = 1f;

		public bool IsChatEnabled = false;

		public GameObject Waypoints;

		public string EnterAnimTrigger;

		public string ExitAnimTrigger;

		public bool StopAtEndPoint;

		public float TurnSmoothing = 30f;

		public float StartAccel = 1f;

		public bool SnapToStart = false;

		public AnimationCurve TravelAccelCurve = new AnimationCurve();

		public float TravelDelay = 0f;

		public float TravelStartSpeed = -1f;

		public float TravelSpeed = -1f;

		public bool UseWorldUp = false;

		public bool Is3DSpace = false;

		public GameObject FakeVehicle = null;

		public GameObject VehiclePrefab = null;

		public Transform VehicleSpawnTransform = null;

		public string SeatName;

		[Range(0f, 1.5f)]
		public float Curvature = 1f;

		[Range(2f, 100f)]
		public int Steps = 10;

		public Color drawColor = Color.blue;

		private List<KeyFrame> keyFrames = new List<KeyFrame>();

		private int curKeyFrameIndex;

		private float distConsumedFromLastFrame;

		private Transform seat = null;

		private GameObject vehicleInstance;

		private Transform vehicleTransform;

		private Animator anim;

		private bool isWarpingToStart;

		private float curWarpSpeed;

		private Vector3 curVel;

		private float elapsedWarpTime;

		private Vector3 vehicleStartPos;

		private Quaternion vehicleStartRot;

		private float elapsedTravelTime;

		protected override void CopyTo(Action _destAction)
		{
			WarpTunnelAction warpTunnelAction = _destAction as WarpTunnelAction;
			warpTunnelAction.Waypoints = Waypoints;
			warpTunnelAction.EnterAnimTrigger = EnterAnimTrigger;
			warpTunnelAction.ExitAnimTrigger = ExitAnimTrigger;
			warpTunnelAction.StopAtEndPoint = StopAtEndPoint;
			warpTunnelAction.SnapToStart = SnapToStart;
			warpTunnelAction.StartAccel = StartAccel;
			warpTunnelAction.TravelDelay = TravelDelay;
			warpTunnelAction.TravelStartSpeed = TravelStartSpeed;
			warpTunnelAction.TravelSpeed = TravelSpeed;
			warpTunnelAction.TravelAccelCurve = TravelAccelCurve;
			warpTunnelAction.TurnSmoothing = TurnSmoothing;
			warpTunnelAction.Curvature = Curvature;
			warpTunnelAction.Steps = Steps;
			warpTunnelAction.UseWorldUp = UseWorldUp;
			warpTunnelAction.Is3DSpace = Is3DSpace;
			warpTunnelAction.FakeVehicle = FakeVehicle;
			warpTunnelAction.VehiclePrefab = VehiclePrefab;
			warpTunnelAction.VehicleSpawnTransform = VehicleSpawnTransform;
			warpTunnelAction.SeatName = SeatName;
			warpTunnelAction.IsChatEnabled = IsChatEnabled;
			base.CopyTo(_destAction);
		}

		protected override void OnEnable()
		{
			curKeyFrameIndex = 0;
			distConsumedFromLastFrame = 0f;
			elapsedWarpTime = 0f;
			elapsedTravelTime = 0f;
			isWarpingToStart = true;
			calculateSplines();
			if (VehiclePrefab != null)
			{
				vehicleInstance = Object.Instantiate(VehiclePrefab);
				vehicleInstance.transform.position = VehicleSpawnTransform.position;
				vehicleInstance.transform.rotation = VehicleSpawnTransform.rotation;
			}
			else
			{
				vehicleInstance = GetTarget();
			}
			vehicleTransform = vehicleInstance.transform;
			vehicleStartPos = vehicleTransform.position;
			vehicleStartRot = vehicleTransform.rotation;
			if (FakeVehicle != null)
			{
				FakeVehicle.SetActive(false);
			}
			if (!string.IsNullOrEmpty(SeatName))
			{
				seat = vehicleInstance.transform.Find(SeatName);
			}
			anim = GetTarget().GetComponent<Animator>();
			if (!string.IsNullOrEmpty(EnterAnimTrigger))
			{
				int trigger = Animator.StringToHash(EnterAnimTrigger);
				anim.SetTrigger(trigger);
			}
			MotionTracker component = GetTarget().GetComponent<MotionTracker>();
			if (component != null)
			{
				curVel = component.Velocity;
				curWarpSpeed = curVel.magnitude;
			}
			else
			{
				curVel = Vector3.zero;
				curWarpSpeed = 0f;
			}
			if (base.gameObject.CompareTag("Player"))
			{
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				if (gameObject != null && !IsChatEnabled)
				{
					gameObject.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "minnpc"));
				}
				Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
				if (Service.Get<QuestService>().ActiveQuest != null)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.ShowHideQuestNotifier(false));
				}
				else
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.HideCellPhoneHud));
				}
			}
		}

		private void calculateSplines()
		{
			keyFrames.Clear();
			if (Waypoints == null)
			{
				return;
			}
			float num = 1f / (float)Steps;
			Transform[] componentsInChildren = Waypoints.GetComponentsInChildren<Transform>(true);
			for (int i = 2; i < componentsInChildren.Length; i++)
			{
				Transform transform = componentsInChildren[i - 1];
				Transform transform2 = componentsInChildren[i];
				Vector3 vector;
				Vector3 a;
				if (i < componentsInChildren.Length - 1)
				{
					if (i > 2)
					{
						vector = Curvature * (transform.position - componentsInChildren[i - 2].position);
						a = Curvature * (transform2.position - transform.position);
					}
					else
					{
						vector = Curvature * (transform2.position - transform.position);
						a = Curvature * (transform2.position - transform.position);
					}
				}
				else if (i > 2)
				{
					vector = Curvature * (transform.position - componentsInChildren[i - 2].position);
					a = Curvature * (transform2.position - transform.position);
				}
				else
				{
					vector = Curvature * (transform2.position - transform.position);
					a = vector;
				}
				for (int j = 0; j < Steps; j++)
				{
					float num2 = (float)j * num;
					float num3 = num2 * num2;
					float num4 = num3 * num2;
					float num5 = 2f * num4;
					float num6 = 3f * num3;
					float d = num5 - num6 + 1f;
					float d2 = 0f - num5 + num6;
					float d3 = num4 - 2f * num3 + num2;
					float d4 = num4 - num3;
					Vector3 pos = d * transform.position + d2 * transform2.position + d3 * vector + d4 * a;
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
				if (i == componentsInChildren.Length - 1)
				{
					KeyFrame item = default(KeyFrame);
					item.Set(transform2.position, keyFrames[keyFrames.Count - 1].pos);
					keyFrames.Add(item);
				}
			}
		}

		private Vector3 GetNextPos(float deltaDist)
		{
			Vector3 position = vehicleTransform.position;
			Vector3 result = position;
			for (int i = curKeyFrameIndex + 1; i < keyFrames.Count; i++)
			{
				float num;
				if (i == 1)
				{
					num = (keyFrames[i].pos - position).magnitude;
					if (deltaDist <= num)
					{
						distConsumedFromLastFrame = 0f;
						result = position + (keyFrames[i].pos - position).normalized * deltaDist;
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

		protected override void OnDisable()
		{
			if (vehicleInstance != null && vehicleInstance != Owner)
			{
				Object.Destroy(vehicleInstance);
			}
			if (FakeVehicle != null)
			{
				FakeVehicle.SetActive(true);
			}
			if (base.gameObject.CompareTag("Player"))
			{
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				if (gameObject != null && !IsChatEnabled)
				{
					gameObject.GetComponent<StateMachineContext>().SendEvent(new ExternalEvent("Root", "exit_cinematic"));
				}
				Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
				if (Service.Get<QuestService>().ActiveQuest != null)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.ShowHideQuestNotifier(true));
				}
				else
				{
					Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.ShowCellPhoneHud));
				}
			}
			base.OnDisable();
		}

		protected override void Update()
		{
			if (keyFrames.Count > 1)
			{
				if (isWarpingToStart)
				{
					if (SnapToStart)
					{
						vehicleTransform.position = keyFrames[0].pos;
						isWarpingToStart = false;
					}
					else
					{
						Vector3 position = vehicleTransform.position;
						Vector3 rhs = keyFrames[0].pos - position;
						Vector3 normalized = rhs.normalized;
						curWarpSpeed += StartAccel * Time.deltaTime;
						curWarpSpeed = Mathf.Clamp(curWarpSpeed, 0f, TravelSpeed);
						elapsedWarpTime += Time.deltaTime;
						float t = Mathf.Clamp01(elapsedWarpTime / timeToAlign);
						Vector3 b = normalized * curWarpSpeed;
						curVel = Vector3.Lerp(curVel, b, t);
						Vector3 vector = position + curVel * Time.deltaTime;
						vehicleTransform.position = vector;
						Vector3 lhs = keyFrames[0].pos - vector;
						float sqrMagnitude = lhs.sqrMagnitude;
						if (sqrMagnitude < closeEnoughDistSq || Vector3.Dot(lhs, rhs) <= 0f)
						{
							isWarpingToStart = false;
						}
					}
					if (!isWarpingToStart && TravelStartSpeed >= 0f)
					{
						curWarpSpeed = TravelStartSpeed;
					}
					updateSeat();
				}
				else
				{
					elapsedTravelTime += Time.deltaTime;
					if (elapsedTravelTime >= TravelDelay)
					{
						if (TravelAccelCurve.length > 1)
						{
							curWarpSpeed = TravelAccelCurve.Evaluate(elapsedTravelTime) * TravelSpeed;
							curWarpSpeed = Mathf.Clamp(curWarpSpeed, minAccelCurveValue, TravelSpeed);
						}
						else
						{
							curWarpSpeed = TravelSpeed;
						}
						float deltaDist = curWarpSpeed * Time.deltaTime;
						Vector3 vector = GetNextPos(deltaDist);
						if (vector != vehicleTransform.position)
						{
							curVel = (vector - vehicleTransform.position) / Time.deltaTime;
							vehicleTransform.position = vector;
							updateSeat();
						}
						else
						{
							if (!string.IsNullOrEmpty(ExitAnimTrigger))
							{
								int trigger = Animator.StringToHash(ExitAnimTrigger);
								anim.SetTrigger(trigger);
							}
							if (StopAtEndPoint)
							{
								vehicleTransform.position = vehicleStartPos;
								vehicleTransform.rotation = vehicleStartRot;
							}
							else
							{
								vehicleTransform.position += curVel * Time.deltaTime;
								updateSeat();
								LocomotionTracker component = GetTarget().GetComponent<LocomotionTracker>();
								if (component != null)
								{
									LocomotionController currentController = component.GetCurrentController();
									if (currentController != null)
									{
										currentController.SetForce(curVel);
									}
								}
							}
							Completed(curVel);
						}
					}
				}
				if (!(elapsedTravelTime >= TravelDelay) || !(curVel.sqrMagnitude > 0f))
				{
					return;
				}
				Vector3 normalized2 = curVel.normalized;
				Vector3 upwards = Vector3.up;
				if (!UseWorldUp)
				{
					if (!Is3DSpace)
					{
						upwards = new Vector3(0f - normalized2.y, normalized2.x, 0f);
					}
					else
					{
						Vector3 lhs2 = Vector3.Cross(normalized2, Vector3.up);
						upwards = Vector3.Cross(lhs2, normalized2);
					}
				}
				Quaternion b2 = Quaternion.LookRotation(normalized2, upwards);
				vehicleTransform.rotation = Quaternion.Slerp(vehicleTransform.rotation, b2, TurnSmoothing * Time.deltaTime);
			}
			else
			{
				Completed(Vector3.zero);
			}
		}

		private void updateSeat()
		{
			if (seat != null)
			{
				GetTarget().transform.position = seat.position;
				GetTarget().transform.rotation = seat.rotation;
			}
		}
	}
}
