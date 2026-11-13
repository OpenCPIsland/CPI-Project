using System;

namespace UnityTest
{
	[Flags]
	public enum CheckMethod
	{
		AfterPeriodOfTime = 0x1,
		Start = 0x2,
		Update = 0x4,
		FixedUpdate = 0x8,
		LateUpdate = 0x10,
		OnDestroy = 0x20,
		OnEnable = 0x40,
		OnDisable = 0x80,
		OnControllerColliderHit = 0x100,
		OnParticleCollision = 0x200,
		OnJointBreak = 0x400,
		OnBecameInvisible = 0x800,
		OnBecameVisible = 0x1000,
		OnTriggerEnter = 0x2000,
		OnTriggerExit = 0x4000,
		OnTriggerStay = 0x8000,
		OnCollisionEnter = 0x10000,
		OnCollisionExit = 0x20000,
		OnCollisionStay = 0x40000,
		OnTriggerEnter2D = 0x80000,
		OnTriggerExit2D = 0x100000,
		OnTriggerStay2D = 0x200000,
		OnCollisionEnter2D = 0x400000,
		OnCollisionExit2D = 0x800000,
		OnCollisionStay2D = 0x1000000
	}
}
