using Disney.LaunchPadFramework;
using Fabric;
using System.Collections.Generic;
using UnityEngine;

public class MeshFXAnimator : MonoBehaviour
{
	public string MeshTag = "_frame";

	[Range(0.01f, float.MaxValue)]
	public float TotalTime = 1f;

	public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public bool PersistAfterLastFrame = false;

	public bool SelfDestruct;

	public float DestroyAfterSeconds = 2f;

	public bool ResetOnEnabled = false;

	public bool PlaySoundOnLoop = false;

	public string SoundEventToPlay = "";

	[Tooltip("Which frame should trigger the audio event? Must be between 0 and total number of frames-1.")]
	public int PlaySoundOnFrame = 0;

	public GameObject OverrideSource = null;

	private List<MeshRenderer> meshesAsFrames;

	private int oldArrayPos;

	private float normalizedTimeStepRecip;

	private float totalTimeRecip;

	private float elapsedTime = 0f;

	private void Start()
	{
		meshesAsFrames = new List<MeshRenderer>();
		base.gameObject.GetComponentsInChildren(meshesAsFrames);
		findMeshRenderers(MeshTag);
		if (meshesAsFrames.Count > 0)
		{
			for (int i = 0; i < meshesAsFrames.Count; i++)
			{
				meshesAsFrames[i].enabled = false;
			}
			normalizedTimeStepRecip = 1f / divideByMeshCnt();
			totalTimeRecip = 1f / TotalTime;
			if (SelfDestruct)
			{
				Object.Destroy(base.gameObject, DestroyAfterSeconds);
			}
			if (OverrideSource == null)
			{
				OverrideSource = base.gameObject;
			}
		}
	}

	private void findMeshRenderers(string tag)
	{
		for (int i = 0; i < meshesAsFrames.Count; i++)
		{
			if (!meshesAsFrames[i].name.Contains(tag))
			{
				meshesAsFrames.RemoveAt(i);
				i--;
			}
		}
	}

	private float divideByMeshCnt()
	{
		int count = meshesAsFrames.Count;
		float result = 1f / (float)count;
		PlaySoundOnFrame = Mathf.Clamp(PlaySoundOnFrame, 0, meshesAsFrames.Count - 1);
		return result;
	}

	private int curveToArrayPos()
	{
		float num = curve.Evaluate(elapsedTime * totalTimeRecip);
		int value = Mathf.FloorToInt(num * normalizedTimeStepRecip);
		return Mathf.Clamp(value, 0, meshesAsFrames.Count - 1);
	}

	private void flipMeshes()
	{
		int num = curveToArrayPos();
		float num2 = curve.Evaluate(elapsedTime * totalTimeRecip);
		if (num2 < 1f)
		{
			if (meshesAsFrames[oldArrayPos] != null)
			{
				meshesAsFrames[oldArrayPos].enabled = false;
			}
			if (meshesAsFrames[num] != null)
			{
				meshesAsFrames[num].enabled = true;
			}
		}
		else if (!PersistAfterLastFrame)
		{
			meshesAsFrames[oldArrayPos].enabled = false;
		}
		if (num != oldArrayPos && num == PlaySoundOnFrame && PlaySoundOnLoop)
		{
			EventManager.Instance.PostEvent(SoundEventToPlay, EventAction.PlaySound, OverrideSource);
		}
		oldArrayPos = num;
	}

	private void OnEnable()
	{
		if (ResetOnEnabled)
		{
			elapsedTime = 0f;
		}
	}

	private void OnDisable()
	{
		if (meshesAsFrames == null)
		{
			return;
		}
		int count = meshesAsFrames.Count;
		for (int i = 0; i < count; i++)
		{
			MeshRenderer meshRenderer = meshesAsFrames[i];
			if (meshRenderer != null)
			{
				meshRenderer.enabled = false;
			}
			else
			{
				Log.LogErrorFormatted(this, "No mesh found at index {0} during OnDisable", i);
			}
		}
	}

	private void Update()
	{
		if (meshesAsFrames != null && meshesAsFrames.Count > 0)
		{
			elapsedTime += Time.deltaTime;
			flipMeshes();
		}
	}
}
