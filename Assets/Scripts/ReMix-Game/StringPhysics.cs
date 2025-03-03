using UnityEngine;

public class StringPhysics : MonoBehaviour
{
	public float Gravity = -9.8f;

	public Transform EndEffector = null;

	public float Length = 0f;

	public float Stiffness = 0.995f;

	public float MinImpulse = 0.05f;

	public float MaxImpulse = 0.075f;

	public float TwistSmoothness = 1f;

	public float MaxStartTwistVel = 20f;

	public float MinTwistImpulse = 200f;

	public float MaxTwistImpulse = 400f;

	public int Iterations = 3;

	public float SimDelay = 0f;

	private Vector3 prevEndEffectorPos = Vector3.zero;

	private Vector3 anchorPos;

	private float desiredLen;

	private Vector3 impulse;

	private float curTwist;

	private float twistVel;

	private float elapsedTime;

	public void Awake()
	{
		twistVel = Random.value * (MaxStartTwistVel * 2f) - MaxStartTwistVel;
	}

	public void FixedUpdate()
	{
		anchorPos = base.transform.position;
		desiredLen = ((Length > 0f) ? Length : (EndEffector.position - base.transform.position).magnitude);
		if (elapsedTime <= SimDelay)
		{
			prevEndEffectorPos = EndEffector.position;
			curTwist = EndEffector.rotation.eulerAngles.y;
		}
		elapsedTime += Time.fixedDeltaTime;
		Vector3 a = EndEffector.position - prevEndEffectorPos + impulse;
		Vector3 vector = EndEffector.position + a * Stiffness;
		impulse = Vector3.zero;
		vector.y += Gravity * (Time.fixedDeltaTime * Time.fixedDeltaTime);
		for (int i = 0; i < Iterations; i++)
		{
			Vector3 vector2 = vector - anchorPos;
			float magnitude = vector2.magnitude;
			float d = desiredLen - magnitude;
			vector += vector2.normalized * d;
			if (vector.y >= anchorPos.y)
			{
				vector.y = anchorPos.y;
			}
		}
		prevEndEffectorPos = EndEffector.position;
		EndEffector.position = vector;
		curTwist += twistVel * Time.fixedDeltaTime;
		twistVel = Mathf.Lerp(twistVel, 0f, TwistSmoothness * Time.fixedDeltaTime);
		if (Mathf.Abs(curTwist) >= 360f)
		{
			curTwist -= (float)(int)(curTwist / 360f) * 360f;
		}
		EndEffector.rotation = Quaternion.identity;
		EndEffector.Rotate(0f, curTwist, 0f, Space.Self);
		Vector3 normalized = (anchorPos - vector).normalized;
		Vector3 forward = Vector3.Cross(EndEffector.right, normalized);
		EndEffector.rotation = Quaternion.LookRotation(forward, normalized);
	}

	public void ApplyImpulse(ref Vector3 impulseDir, float normalizedImpulseMag, float twistDir)
	{
		twistVel = twistDir * (MinTwistImpulse + normalizedImpulseMag * (MaxTwistImpulse - MinTwistImpulse));
		curTwist += twistVel;
		if (Mathf.Abs(curTwist) >= 360f)
		{
			curTwist -= (float)(int)(curTwist / 360f) * 360f;
		}
		impulse = impulseDir.normalized * (MinImpulse + normalizedImpulseMag * (MaxImpulse - MinImpulse));
	}
}
