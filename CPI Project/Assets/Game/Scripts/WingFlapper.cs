using System;
using UnityEngine;

public class WingFlapper : MonoBehaviour
{
	[Range(0f, 360f)]
	[Tooltip("Set the range of motion for the wings")]
	public float RangeOfMotionInDegrees = 45f;

	[Tooltip("Set the speed of the flapping motion")]
	public float FlapsPerSecond = 2f;

	[Tooltip("If checked, wings will start in a random position")]
	public bool RandomStartPosition = true;

	public bool RotateOnZAxis = true;

	public bool RotateOnXAxis = false;

	public bool RotateOnYAxis = false;

	[Tooltip("Which objects are wings?")]
	public GameObject[] Wings;

	private float index;

	private float frequency;

	private float seed;

	private int ZRotMult = 0;

	private int XRotMult = 0;

	private int YRotMult = 0;

	private void Start()
	{
		if (RandomStartPosition)
		{
			seed = (float)UnityEngine.Random.Range(0, 19) / (float)Math.PI;
		}
		if (RotateOnZAxis)
		{
			ZRotMult = 1;
		}
		if (RotateOnXAxis)
		{
			XRotMult = 1;
		}
		if (RotateOnYAxis)
		{
			YRotMult = 1;
		}
	}

	private void Update()
	{
		float num = SmoothSineWave();
		Vector3 vector = new Vector3((float)XRotMult * num, (float)YRotMult * num, (float)ZRotMult * num);
		GameObject[] wings = Wings;
		foreach (GameObject gameObject in wings)
		{
			if (gameObject.name.Contains("R"))
			{
				gameObject.transform.localEulerAngles = vector;
			}
			else
			{
				gameObject.transform.localEulerAngles = -vector;
			}
		}
		index += Time.deltaTime;
	}

	public float SmoothSineWave()
	{
		return RangeOfMotionInDegrees * Mathf.Sin(FlapsPerSecond * (float)Math.PI * index + seed);
	}
}
