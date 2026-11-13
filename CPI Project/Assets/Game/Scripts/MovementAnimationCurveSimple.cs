using UnityEngine;

public class MovementAnimationCurveSimple : MonoBehaviour
{
	[Tooltip("Animation curve to apply to x-coordinate")]
	public AnimationCurve animCurveX;

	[Tooltip("Value to multiply X curve data by")]
	public float magnitudeX = 1f;

	[Tooltip("Reverses X animation if checked")]
	public bool animReverseX = false;

	[Tooltip("Animation curve to apply to y-coordinate")]
	public AnimationCurve animCurveY;

	[Tooltip("Value to multiply Y curve data by")]
	public float magnitudeY = 1f;

	[Tooltip("Reverses Y animation if checked")]
	public bool animReverseY = false;

	[Tooltip("Amount of seconds to play 1 cycle")]
	public float animSecondsPerCycle = 1f;

	public bool UseLocalSpace = false;

	[Range(0f, 1f)]
	public float startPosition = 0f;

	public GameObject RelativeToObject;

	private Vector3 originalPosition;

	public bool IsActive = false;

	public void Start()
	{
		SetOriginalPosition();
	}

	private void Update()
	{
		if (!IsActive)
		{
			return;
		}
		float curveTime = startPosition + Time.time % (float)(int)(animSecondsPerCycle * 1f) / 1f / animSecondsPerCycle;
		if (RelativeToObject == null)
		{
			if (!UseLocalSpace)
			{
				base.gameObject.transform.position = GetPointAtTime(originalPosition, curveTime);
			}
			else
			{
				base.gameObject.transform.localPosition = GetPointAtTime(originalPosition, curveTime);
			}
		}
		else if (!UseLocalSpace)
		{
			base.gameObject.transform.position = RelativeToObject.transform.position + GetPointAtTime(originalPosition, curveTime);
		}
		else
		{
			base.gameObject.transform.localPosition = RelativeToObject.transform.position + GetPointAtTime(originalPosition, curveTime);
		}
	}

	private Vector3 GetPointAtTime(Vector3 origPos, float curveTime)
	{
		float num = 0f;
		float num2 = 0f;
		if (animCurveX != null && animCurveX.length > 0)
		{
			num = animCurveX.Evaluate(curveTime) * magnitudeX;
			if (animReverseX)
			{
				num *= -1f;
			}
		}
		if (animCurveY != null && animCurveY.length > 0)
		{
			num2 = animCurveY.Evaluate(curveTime) * magnitudeY;
			if (animReverseY)
			{
				num2 *= -1f;
			}
		}
		Vector3 result = origPos;
		result.x += num;
		result.y += num2;
		return result;
	}

	public void SetOriginalPosition()
	{
		if (!UseLocalSpace)
		{
			originalPosition = base.gameObject.transform.position;
		}
		else
		{
			originalPosition = base.gameObject.transform.localPosition;
		}
	}
}
