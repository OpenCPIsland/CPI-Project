using Disney.Kelowna.Common;
using System;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class WarpToTransformAction : Action
	{
		public Transform TargetTransform;

		public Vector3 TargetOffset = Vector3.zero;

		public float Duration = 0f;

		public float Smoothness = 0f;

		private Transform thisTransform;

		private Vector3 startPos;

		private Quaternion startRot;

		private float elapsedTime;

		public event Action<WarpToTransformAction> WarpStarted;

		public event Action<WarpToTransformAction> WarpCompleted;

		protected override void CopyTo(Action _destWarper)
		{
			WarpToTransformAction warpToTransformAction = _destWarper as WarpToTransformAction;
			warpToTransformAction.TargetTransform = TargetTransform;
			warpToTransformAction.TargetOffset = TargetOffset;
			warpToTransformAction.Duration = Duration;
			warpToTransformAction.Smoothness = Smoothness;
			warpToTransformAction.WarpStarted = this.WarpStarted;
			warpToTransformAction.WarpCompleted = this.WarpCompleted;
			base.CopyTo(_destWarper);
		}

		protected override void OnEnable()
		{
			thisTransform = GetTarget().transform;
			if (TargetTransform != null)
			{
				startPos = thisTransform.position;
				startRot = thisTransform.rotation;
			}
			base.OnEnable();
		}

		protected override void Update()
		{
			if (TargetTransform != null)
			{
				this.WarpStarted.InvokeSafe(this);
				bool flag = true;
				if (Duration > 0f)
				{
					elapsedTime += Time.deltaTime;
					if (Smoothness > 0f)
					{
						thisTransform.position = Vector3.Lerp(thisTransform.position, TargetTransform.position + TargetOffset, Smoothness * Time.deltaTime);
						thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, TargetTransform.rotation, Smoothness * Time.deltaTime);
					}
					else
					{
						float t = elapsedTime / Duration;
						thisTransform.position = Vector3.Lerp(startPos, TargetTransform.position + TargetOffset, t);
						thisTransform.rotation = Quaternion.Slerp(startRot, TargetTransform.rotation, t);
					}
					if (elapsedTime < Duration)
					{
						flag = false;
					}
				}
				if (flag)
				{
					thisTransform.position = TargetTransform.position + TargetOffset;
					thisTransform.rotation = TargetTransform.rotation;
					Completed();
					this.WarpCompleted.InvokeSafe(this);
				}
			}
			else
			{
				Completed();
			}
		}
	}
}
