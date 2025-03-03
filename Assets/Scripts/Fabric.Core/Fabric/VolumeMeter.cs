using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	[AddComponentMenu("Fabric/Mixing/VolumeMeter")]
	public class VolumeMeter : MonoBehaviour
	{
		[HideInInspector]
		private AudioComponent[] audioComponents;

		[HideInInspector]
		public VolumeMeterState volumeMeterState = new VolumeMeterState();

		[SerializeField]
		[HideInInspector]
		public bool _is3D = true;

		[SerializeField]
		[HideInInspector]
		public string _globalParameterName;

		[NonSerialized]
		[HideInInspector]
		public CodeProfiler profiler = new CodeProfiler();

		private float[,] samples;

		private float[] tempSamples;

		private AudioListener listener;

		public void OnInitialise()
		{
			for (int i = 0; i < 5; i++)
			{
				volumeMeterState.mHistory[i] = new VolumeMeterState.stSpeakers();
			}
			samples = new float[2, 256];
			tempSamples = new float[256];
			listener = (AudioListener)UnityEngine.Object.FindObjectOfType(typeof(AudioListener));
			CollectAudioComponents();
		}

		public void CollectAudioComponents()
		{
			AudioComponent component = base.gameObject.GetComponent<AudioComponent>();
			if (component != null)
			{
				audioComponents = new AudioComponent[1];
				audioComponents[0] = component;
				return;
			}
			List<AudioComponent> list = new List<AudioComponent>();
			AudioComponent[] componentsInChildren = base.gameObject.GetComponentsInChildren<AudioComponent>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				list.Add(componentsInChildren[i]);
			}
			GroupComponentProxy[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<GroupComponentProxy>(true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				AudioComponent[] componentsInChildren3 = componentsInChildren2[j]._groupComponent.GetComponentsInChildren<AudioComponent>();
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					list.Add(componentsInChildren3[k]);
				}
			}
			audioComponents = list.ToArray();
		}

		private float distanceAttenuation(float distance, float minDistance, float maxDistance, AudioRolloffMode rolloffMode)
		{
			if (distance <= minDistance)
			{
				return 1f;
			}
			if (distance > maxDistance)
			{
				distance = maxDistance;
			}
			switch (rolloffMode)
			{
			case AudioRolloffMode.Custom:
				return 1f;
			case AudioRolloffMode.Linear:
			{
				float num = (minDistance < maxDistance) ? ((maxDistance - distance) / (maxDistance - minDistance)) : 1f;
				if (rolloffMode != AudioRolloffMode.Linear)
				{
					return num * num;
				}
				return num;
			}
			default:
				if (!(distance > 0f))
				{
					return 1f;
				}
				return minDistance / scaledRolloffDistance(distance, minDistance, maxDistance);
			}
		}

		private float scaledRolloffDistance(float distance, float minDistance, float maxDistance)
		{
			float num = 1f;
			if (!(distance > minDistance) || num == 1f)
			{
				return distance;
			}
			return (distance - minDistance) * num + minDistance;
		}

		private void VolumeMeterProcess(ref VolumeMeterState outState)
		{
			VolumeMeterState.stSpeakers stSpeakers = outState.mHistory[outState.mHistoryIndex];
			outState.mHistoryIndex++;
			outState.mHistoryIndex %= 5;
			stSpeakers.Clear();
			int length = samples.Length;
			float num = 0f;
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 256; j++)
				{
					float num2 = samples[i, j];
					float b = (num2 < 0f) ? (0f - num2) : num2;
					float a = stSpeakers.mChannels[i];
					stSpeakers.mChannels[i] = Mathf.Max(a, b);
					num += num2 * num2;
				}
			}
			if (length > 0)
			{
				num /= 256f;
				num = Mathf.Sqrt(num);
				num = Mathf.Max(num, 0f);
				num = Mathf.Min(num, 1f);
			}
			stSpeakers.mRMS = num;
			outState.mPeaks.Clear();
			outState.mRMS = 0f;
			for (int k = 0; k < 5; k++)
			{
				VolumeMeterState.stSpeakers stSpeakers2 = outState.mHistory[k];
				outState.mRMS += stSpeakers2.mRMS;
				for (int l = 0; l < 2; l++)
				{
					outState.mPeaks.mChannels[l] += stSpeakers2.mChannels[l];
				}
			}
			float num3 = 0.2f;
			outState.mRMS *= num3;
			for (int m = 0; m < 2; m++)
			{
				outState.mPeaks.mChannels[m] *= num3;
			}
		}

		public void Update()
		{
			profiler.Begin();
			if (samples != null && audioComponents != null)
			{
				Array.Clear(samples, 0, samples.Length);
				for (int i = 0; i < audioComponents.Length; i++)
				{
					AudioComponent audioComponent = audioComponents[i];
					if (!(audioComponent != null) || !audioComponent.IsPlaying() || !(audioComponent.AudioSource != null))
					{
						continue;
					}
					AudioSource audioSource = audioComponent.AudioSource;
					if (!audioSource.isPlaying || !(audioComponent.ParentGameObject != null))
					{
						continue;
					}
					float num = 0f;
					if (_is3D)
					{
						float distance = 0f;
						if (listener != null)
						{
							distance = (listener.transform.position - audioComponent.ParentGameObject.transform.position).magnitude;
						}
						num = distanceAttenuation(distance, audioSource.minDistance, audioSource.maxDistance, audioSource.rolloffMode) * audioSource.volume;
					}
					else
					{
						num = audioSource.volume;
					}
					for (int j = 0; j < 2; j++)
					{
						audioSource.GetOutputData(tempSamples, j);
						for (int k = 0; k < 256; k++)
						{
							samples[j, k] += tempSamples[k] * num;
						}
					}
				}
				VolumeMeterProcess(ref volumeMeterState);
			}
			if (_globalParameterName != null)
			{
				float db = AudioTools.LinearToDB(volumeMeterState.mRMS);
				EventManager.Instance._globalParameterManager.SetGlobalParameter(_globalParameterName, AudioTools.DBToNormalizedDB(db));
			}
			profiler.End();
		}
	}
}
