using UnityEngine;

public class PenguinSnowballThrowData : ScriptableObject
{
	public string SnowballPoolName = "Pool[Snowball]";

	public int AnimLayerIndex = 2;

	public Transform ThrowDirection;

	public float MinThrowSpeed = 1f;

	public float MaxThrowSpeed = 4f;

	public float MinLiftForce = 2f;

	public float MaxLiftForce = 6f;

	public float MaxChargeTime = 2f;

	public float MinTrailAlpha = 0.1f;

	public float MaxTrailAlpha = 1f;

	public float CharVelocityFactor = 1f;

	public string ChargeAnimTrigger = "ChargeSnowball";

	public string LaunchAnimTrigger = "ThrowSnowball";

	[Header("AimAssist")]
	public bool EnableAimAssist = true;

	public LayerMask AimAssistCollisionLayers;

	public Vector3 AimAssistRaycastError = Vector3.one;

	public float AimAssistRange = 75f;
}
