using Disney.LaunchPadFramework;
using System;
using UnityEngine;

namespace ClubPenguin
{
	public class ExpandParticlesToViewport : MonoBehaviour
	{
		private static int UPDATE_ON_FRAME = 3;

		public float EffectDepth = 5f;

		public Vector3 EffectPercentAdjustment = Vector3.one;

		public Vector3 ColliderPercentAdjustment = Vector3.one;

		private Camera targetCamera;

		private ParticleSystem partSys;

		private ParticleSystem.ShapeModule partSysShape;

		private BoxCollider partSysCollider;

		private void Start()
		{
			targetCamera = GetComponentInParent<Camera>();
			if (targetCamera == null)
			{
				Log.LogError(this, string.Format("O_o\t Error: Can't find camera in parent, please add one"));
			}
			partSys = base.transform.GetComponentInChildren<ParticleSystem>();
			if (partSys == null)
			{
				Log.LogError(this, string.Format("O_o\t Error: Can't find particle system in children, please add one"));
			}
			else
			{
				partSysShape = partSys.shape;
			}
			partSysCollider = base.transform.GetComponentInChildren<BoxCollider>();
			if (partSysCollider == null)
			{
				Log.LogError(this, string.Format("O_o\t Error: Can't find BoxCollider in children, please add one"));
			}
			if (EffectPercentAdjustment == Vector3.zero)
			{
				EffectPercentAdjustment = Vector3.one;
			}
			if (ColliderPercentAdjustment == Vector3.zero)
			{
				ColliderPercentAdjustment = Vector3.one;
			}
			if (targetCamera != null && partSys != null && partSysCollider != null)
			{
				ResizeTarget();
			}
		}

		private void ResizeTarget()
		{
			float num = Vector3.Distance(targetCamera.gameObject.transform.position, base.transform.position);
			float num2 = 2f * num * Mathf.Tan(targetCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f));
			float num3 = num2 * targetCamera.aspect;
			Vector3 scale = partSysShape.scale;
			scale.x = num3 * EffectPercentAdjustment.x;
			scale.y = EffectDepth * EffectPercentAdjustment.y;
			scale.z = num2 * EffectPercentAdjustment.z;
			partSysShape.scale = scale;
			partSysCollider.size = new Vector3(scale.x * ColliderPercentAdjustment.x, scale.y * ColliderPercentAdjustment.y, scale.z * ColliderPercentAdjustment.z);
		}
	}
}
