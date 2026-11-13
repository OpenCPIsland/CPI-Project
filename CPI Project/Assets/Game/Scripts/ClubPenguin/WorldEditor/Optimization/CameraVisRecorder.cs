using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.WorldEditor.Optimization
{
	public class CameraVisRecorder : MonoBehaviour
	{
		public int MaxEntries;

		public float MinAngleVariance;

		public float MinDistanceVariance;

		public float CamFov;

		public float RecordDelaySec;

		private CameraVisibility[] camVisibilities;

		private int camVisIndex = 0;

		private float timeElapsedSec;

		private CameraVisibility lastCamVisibility;

		private bool recording = false;

		public void Init(int maxEntries, float minAngleVariance, float minDistanceVariance, int recordDelayMS, float camFov)
		{
			MaxEntries = maxEntries;
			MinAngleVariance = minAngleVariance;
			MinDistanceVariance = minDistanceVariance;
			RecordDelaySec = (float)recordDelayMS / 1000f;
			CamFov = camFov;
		}

		private void Start()
		{
			camVisibilities = new CameraVisibility[MaxEntries];
			recording = true;
		}

		private void Update()
		{
			if (!recording)
			{
				return;
			}
			timeElapsedSec += Time.deltaTime;
			if (timeElapsedSec < RecordDelaySec)
			{
				return;
			}
			timeElapsedSec = 0f;
			if (Camera.main == null)
			{
				return;
			}
			Transform transform = Camera.main.transform;
			Vector3 position = Camera.main.transform.position;
			Vector3 forward = Camera.main.transform.forward;
			if (!(lastCamVisibility.Position == position) || !(lastCamVisibility.Forward == forward))
			{
				CameraVisibility cameraVisibility = default(CameraVisibility);
				cameraVisibility.Position = position;
				cameraVisibility.Forward = forward.normalized;
				cameraVisibility.Up = transform.up;
				cameraVisibility.Right = transform.right;
				camVisibilities[camVisIndex] = cameraVisibility;
				camVisIndex++;
				lastCamVisibility = cameraVisibility;
				if (camVisIndex == MaxEntries)
				{
					StopRecording();
				}
			}
		}

		public CameraVisibility[] StopRecording()
		{
			recording = false;
			return collapseVisibilities().ToArray();
		}

		private List<CameraVisibility> collapseVisibilities()
		{
			int num = int.MaxValue;
			int num2 = int.MinValue;
			int num3 = int.MaxValue;
			int num4 = int.MinValue;
			int num5 = int.MaxValue;
			int num6 = int.MinValue;
			for (int i = 0; i < camVisIndex; i++)
			{
				int num7 = (int)Mathf.Round(camVisibilities[i].Position.x);
				if (num7 < num)
				{
					num = num7;
				}
				if (num7 > num2)
				{
					num2 = num7;
				}
				int num8 = (int)Mathf.Round(camVisibilities[i].Position.y);
				if (num8 < num3)
				{
					num3 = num8;
				}
				if (num8 > num4)
				{
					num4 = num8;
				}
				int num9 = (int)Mathf.Round(camVisibilities[i].Position.z);
				if (num9 < num5)
				{
					num5 = num9;
				}
				if (num9 > num6)
				{
					num6 = num9;
				}
			}
			num--;
			num2++;
			num3--;
			num4++;
			num5--;
			num6++;
			int num10 = -num;
			int num11 = Mathf.Abs(num2 - num) + 1;
			int num12 = -num3;
			int num13 = Mathf.Abs(num4 - num3) + 1;
			int num14 = -num5;
			int num15 = Mathf.Abs(num6 - num5) + 1;
			List<CameraVisibility>[,,] array = new List<CameraVisibility>[num11, num13, num15];
			for (int i = 0; i < camVisIndex; i++)
			{
				int num16 = (int)Mathf.Round(camVisibilities[i].Position.x) + num10;
				int num17 = (int)Mathf.Round(camVisibilities[i].Position.y) + num12;
				int num18 = (int)Mathf.Round(camVisibilities[i].Position.z) + num14;
				bool flag = false;
				List<CameraVisibility> list = array[num16, num17, num18];
				if (list == null)
				{
					list = (array[num16, num17, num18] = new List<CameraVisibility>());
				}
				else
				{
					for (int j = 0; j < list.Count; j++)
					{
						float magnitude = (camVisibilities[i].Position - list[j].Position).magnitude;
						if (magnitude <= MinDistanceVariance && Vector3.Angle(list[j].Forward, camVisibilities[i].Forward) <= MinAngleVariance)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					CameraVisibility item = default(CameraVisibility);
					item.Forward = camVisibilities[i].Forward;
					item.Up = camVisibilities[i].Up;
					item.Right = camVisibilities[i].Right;
					item.Position = camVisibilities[i].Position;
					item.Fov = CamFov;
					list.Add(item);
				}
			}
			deriveAdditionalVisibilities(array, num10, num12, num14);
			return getUniqueVisibilities(array);
		}

		private void deriveAdditionalVisibilities(List<CameraVisibility>[,,] camVisibilitiesAtPos, int xIndexOffset, int yIndexOffset, int zIndexOffset)
		{
			int length = camVisibilitiesAtPos.GetLength(0);
			int length2 = camVisibilitiesAtPos.GetLength(1);
			int length3 = camVisibilitiesAtPos.GetLength(2);
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						List<CameraVisibility> list = camVisibilitiesAtPos[i, j, k];
						if (list != null)
						{
							continue;
						}
						Vector3 vector = default(Vector3);
						Vector3 vector2 = default(Vector3);
						float num2 = 0f;
						int num3 = -1;
						int num4 = 1;
						for (int l = num3; l <= num4; l++)
						{
							int num5 = i + l;
							if (num5 < 0 || num5 >= length)
							{
								continue;
							}
							for (int m = num3; m <= num4; m++)
							{
								int num6 = j + m;
								if (num6 < 0 || num6 >= length2)
								{
									continue;
								}
								for (int n = num3; n <= num4; n++)
								{
									int num7 = k + n;
									if (num7 < 0 || num7 >= length3)
									{
										continue;
									}
									List<CameraVisibility> list2 = camVisibilitiesAtPos[num5, num6, num7];
									if (list2 == null)
									{
										continue;
									}
									for (int num8 = 0; num8 < list2.Count; num8++)
									{
										if (!list2[num8].IsDerived)
										{
											vector += list2[num8].Forward;
											vector2 += list2[num8].Up;
											num2 = ((list2[num8].Fov > num2) ? list2[num8].Fov : num2);
										}
									}
								}
							}
						}
						if (vector.magnitude != 0f)
						{
							CameraVisibility item = default(CameraVisibility);
							item.Forward = vector.normalized;
							item.Up = vector2.normalized;
							item.Right = Vector3.Cross(item.Up, item.Forward).normalized;
							item.Fov = num2;
							item.IsDerived = true;
							item.Position = new Vector3(i - xIndexOffset, j - yIndexOffset, k - zIndexOffset);
							num++;
							List<CameraVisibility> list3 = new List<CameraVisibility>();
							list3.Add(item);
							camVisibilitiesAtPos[i, j, k] = list3;
						}
					}
				}
			}
		}

		private List<CameraVisibility> getUniqueVisibilities(List<CameraVisibility>[,,] camVisibilitiesAtPos)
		{
			List<CameraVisibility> list = new List<CameraVisibility>();
			int length = camVisibilitiesAtPos.GetLength(0);
			int length2 = camVisibilitiesAtPos.GetLength(1);
			int length3 = camVisibilitiesAtPos.GetLength(2);
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					for (int k = 0; k < length3; k++)
					{
						List<CameraVisibility> list2 = camVisibilitiesAtPos[i, j, k];
						if (list2 != null)
						{
							for (int l = 0; l < list2.Count; l++)
							{
								list.Add(list2[l]);
								num++;
							}
						}
					}
				}
			}
			return list;
		}
	}
}
